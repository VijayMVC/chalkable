using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Model;
using Person = Chalkable.Data.School.Model.Person;
using Room = Chalkable.Data.School.Model.Room;

namespace Chalkable.StiConnector.Services
{
    public class ImportService
    {
        private StiEntities stiEntities;
        private const string NO_ROOM_NUMBER = "No Room";
        private const string SIS_DATA_BASE_CONNECTION_ERROR_FMT = "Invalid sis database connection data {0}, {1}, {2}, {3}";
        private const string CONNECTION_STRING_FMT =
            "metadata=res://*/Model.StiEntities.csdl|res://*/Model.StiEntities.ssdl|res://*/Model.StiEntities.msl;provider=System.Data.SqlClient;provider connection string=\"data source={0};initial catalog={1};persist security info=True;user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework\"";

        protected IServiceLocatorMaster ServiceLocatorMaster { get; set; }
        protected IServiceLocatorSchool ServiceLocatorSchool { get; set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; private set; }
        protected SisConnectionInfo ConnectionInfo { get; private set; }

        public ImportService(Guid districtId, SisConnectionInfo connectionInfo, BackgroundTaskService.BackgroundTaskLog log)
        {
            ConnectionInfo = connectionInfo;
            Log = log;
            ServiceLocatorMaster = ServiceLocatorFactory.CreateMasterSysAdmin();
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);    

            string dbUrl = connectionInfo.SisUrl,
                    dbName = connectionInfo.DbName,
                    userName = connectionInfo.SisUserName,
                    pwd = connectionInfo.SisPassword;
            if (dbUrl == null || dbName == null || userName == null || pwd == null)
                throw new Exception(string.Format(SIS_DATA_BASE_CONNECTION_ERROR_FMT, dbUrl, dbName, userName, pwd));
            var connectionString = string.Format(CONNECTION_STRING_FMT, dbUrl, dbName, userName, pwd);
            stiEntities = new StiEntities(connectionString);
        }
        
        public void ImportSchedule(DateTime? lastUpdate)
        {
            /*ImportSchools();
            ImportGradeLevels();
            Log.LogInfo(ChlkResources.SCHOOL_YEAR_IMPORT_START);
            ImportSchoolYears(SisSchoolYearIds);
            Log.LogInfo(ChlkResources.IMPORT_ROOMS_START);
            ImportRooms(SisSchoolId);


            Log.LogInfo(ChlkResources.IMPORT_COURSES_START);
            ImportCourses(SisSchoolId);

            Log.LogInfo(ChlkResources.IMPORT_CLASSES_START);
            ImportClasses();

            Log.LogInfo(ChlkResources.IMPORT_CLASS_SCHOOL_PERSONS_START);
            ImportClassSchoolPersons(SisSchoolYearIds);*/
        }

        private void ImportSchools()
        {
            throw new NotImplementedException();
        }

        public void ImportPeople(DateTime? lastUpdate)
        {
            Log.LogInfo(ChlkResources.IMPORT_PERSONS_START);
            ImportGradeLevels();
            ImportSchoolPersons();
        }


        private void ImportClassSchoolPersons(IList<int> sessionIds)
        {
            var persons = ServiceLocatorSchool.PersonService.GetPersons();
            var classes = ServiceLocatorSchool.ClassService.GetClasses(null, null, null);
            var studentSchedules = stiEntities.StudentSchedules.Where(x => x.Course1.AcadSessionID.HasValue && sessionIds.Contains(x.Course1.AcadSessionID.Value)).ToList();
            int counter = 0, success = 0;
            var added = new HashSet<Pair<Guid, Guid>>();
            foreach (var studentSchedule in studentSchedules)
            {
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.ROSTER_RECORDS_PROCESSED_ADDED, counter, success));
                counter++;
                var person = persons.FirstOrDefault(x => x.SisId == studentSchedule.StudentID);
                if (person == null)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_IMPORT_SCHOOL_PERSON_NOT_FOUND_IN_CHALKABLE_DB, studentSchedule.StudentID));
                    continue;
                }
                var clazz = classes.FirstOrDefault(x => x.SisId == studentSchedule.SectionID);
                if (clazz == null)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_IMPORT_CLASS_WAS_NOT_FOUND_CHALKABLE_DB, studentSchedule.SectionID));
                    continue;
                }
                var p = new Pair<Guid, Guid>(clazz.Id, person.Id);
                if (!added.Contains(p))
                {
                    added.Add(p);
                    ServiceLocatorSchool.ClassService.AddStudent(clazz.Id, person.Id);
                }
                success++;
            }
        }

        private void ImportCourses(int stiSchoolId)
        {
            var departments = ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
            var sep = new[] { ',' };
            var departmenPairs = new List<Pair<string, Guid>>();

            foreach (var chalkableDepartment in departments)
            {
                departmenPairs.AddRange(chalkableDepartment.Keywords.ToLower().Split(sep, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Pair<string, Guid>(x, chalkableDepartment.Id)));
            }

            var courses = stiEntities.Courses.Where(x => x.AcadSession.SchoolID == stiSchoolId).ToList();
            foreach (var course in courses)
            {
                string name = course.CourseType.Name.ToLower();
                int minDist = int.MaxValue;
                Pair<string, Guid> closestDep = null;
                for (int i = 0; i < departmenPairs.Count; i++)
                {
                    var d = StringTools.LevenshteinDistance(name, departmenPairs[i].First);
                    if (d < minDist && (d <= 4 || d <= 0.3 * name.Length + 2))
                    {
                        minDist = d;
                        closestDep = departmenPairs[i];
                    }
                }

                ServiceLocatorSchool.CourseService.Add(course.ShortName, course.FullName, null,
                                                           closestDep != null ? closestDep.Second : (Guid?) null,
                                                           course.CourseID);
            }
        }

        private void ImportClasses()
        {
            var markingPeriods = ServiceLocatorSchool.MarkingPeriodService.GetMarkingPeriods(null).Where(x => x.SisId.HasValue).ToList();
            var termIds = markingPeriods.Select(x => x.SisId).ToList();
            var stiCourses = stiEntities.Courses.Where(x => x.SectionTerms.Any(y => termIds.Contains(y.TermID)) && x.Active).ToList();
            var noRoom = ServiceLocatorSchool.RoomService.GetRooms().First(x => x.RoomNumber == NO_ROOM_NUMBER);
            int counter = 0;
            var courses = ServiceLocatorSchool.CourseService.GetCourses().Where(x => x.SisId.HasValue).ToDictionary(x => x.SisId.Value, x => x);
            var schoolYears = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().Where(x => x.SisId.HasValue).ToDictionary(x => x.SisId.Value, x => x);
            var persons = ServiceLocatorSchool.PersonService.GetPersons();
            var teachers = persons.Where(x => x.SisId.HasValue && x.RoleRef == CoreRoles.TEACHER_ROLE.Id).ToDictionary(x => x.SisId.Value, x => x);
            var gradeLevels = ServiceLocatorSchool.GradeLevelService.GetGradeLevels();

            foreach (var stiCourse in stiCourses)
            {
                var course = courses[stiCourse.CourseID];
                if (!stiCourse.AcadSessionID.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_COURSE_DOESNT_HAVE_SESSION_ID, stiCourse.CourseID));
                    continue;
                }
                if (!stiCourse.PrimaryTeacherID.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_COURSE_DOESNT_HAVE_PRIMARY_TEACHER_ID, stiCourse.CourseID));
                    continue;
                }
                var schoolYear = schoolYears[stiCourse.AcadSessionID.Value];
                if (!teachers.ContainsKey(stiCourse.PrimaryTeacherID.Value))
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_TEACHER_WITH_SIS_ID_NOT_FOUND, stiCourse.PrimaryTeacherID.Value));
                    continue;
                }
                var teacher = teachers[stiCourse.PrimaryTeacherID.Value];
                if (teacher == null)
                {
                    continue;
                }

                var glName = gradeLevels.First().Name;
                if (stiCourse.GradeLevel != null)
                    glName = stiCourse.GradeLevel.Name;
                if (stiCourse.GradeLevel1 != null)
                    glName = stiCourse.GradeLevel1.Name;

                var terms = stiCourse.SectionTerms.Select(x => x.TermID).Distinct().ToList();
                var mps = new List<Guid>();
                foreach (var t in terms)
                {
                    var mp = markingPeriods.First(x => x.SisId == t);
                    mps.Add(mp.Id);
                }

                var c = ServiceLocatorSchool.ClassService.Add(schoolYear.Id, course.Id, stiCourse.FullName, stiCourse.FullName, teacher.Id
                    , gradeLevels.First(x => x.Name == glName).Id, mps, stiCourse.CourseID);

                var scheduledSections = stiCourse.ScheduledSections.ToList();
                foreach (var scheduledSection in scheduledSections)
                {
                    var generalPeriods = ServiceLocatorSchool.PeriodService.GetPeriods(null, null).Where(x => x.SisId2 == scheduledSection.TimeSlotID
                        && x.Section.SisId == scheduledSection.DayTypeID && mps.Contains(x.MarkingPeriodRef)).ToList();
                    foreach (var generalPeriod in generalPeriods)
                    {
                        Room room;
                        if (stiCourse.RoomID.HasValue)
                            room = ServiceLocatorSchool.RoomService.GetRooms().First(x => x.SisId == stiCourse.RoomID.Value);
                        else
                            room = noRoom;
                        try
                        {
                            ServiceLocatorSchool.ClassPeriodService.Add(generalPeriod.Id, c.Id, room.Id);
                        }
                        catch (Exception ex)
                        {
                            //TODO: don't use exceptions in logic. need implement canadd
                            Log.LogWarning(string.Format("class {0} wasn't added to room {1} and general period {2} because of {3}", c.Id, room.Id, generalPeriod.Id, ex.Message));
                        }
                    }
                }
                counter++;
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.CLASS_RECORDS_PROCESSED, counter));
            }
        }

        private void ImportGradeLevels()
        {
            var existing = ServiceLocatorSchool.GradeLevelService.GetGradeLevels();
            var sisGradeLevels = stiEntities.GradeLevels.ToList();
            foreach (var gradeLevel in sisGradeLevels)
                if (!existing.Any(x => x.Name == gradeLevel.Name))
                {
                    ServiceLocatorSchool.GradeLevelService.AddGradeLevel(gradeLevel.Name, gradeLevel.Sequence);
                }
        }


        private const string userEmailFmt = "user{0}_{1}@chalkable.com";
        private const string defUserPass = "tester";
        private const string descrWork = "Work";
        private const string descrCell = "cell";
        private const string img = "image";

        private void ImportSchoolPersons()
        {
            /*int counter = 0;
            
            var now = DateTime.UtcNow;
            var persons =
                stiEntities.People.Where(x => 
                    stiEntities.StaffSchools.Any(y=>y.SchoolID == SisSchoolId && y.StaffID == x.PersonID)
                    ||stiEntities.StudentSchools.Any(y=>y.SchoolID == SisSchoolId && y.StudentID == x.PersonID)
                    );
            var gradeLevels = ServiceLocatorSchool.GradeLevelService.GetGradeLevels();

            foreach (var person in persons)
            {
                counter++;
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.USERS_PROCESSED, counter));
                string email = string.Format(userEmailFmt, person.PersonID, SchoolId);


                
                string roleId = null;
                if (person.Student != null)
                    roleId = CoreRoles.STUDENT_ROLE.Name;
                else if (person.Staff != null)
                    roleId = CoreRoles.TEACHER_ROLE.Name;
                else
                    roleId = CoreRoles.ADMIN_VIEW_ROLE.Name;



                Person pr;
                if (person.Student != null)
                {
                    var gl = person.Student.StudentGradeLevels.OrderByDescending(x => x.GradeLevelID).FirstOrDefault(x => x.EndTime == null || x.EndTime < now);
                    if (gl == null)
                    {
                        Log.LogWarning(string.Format(ChlkResources.ERR_STI_NO_GRADE_LEVEL_FOR_STUDENT, person.PersonID));
                    }
                    pr = ServiceLocatorSchool.PersonService.Add(email, defUserPass, person.FirstName, person.LastName, roleId, person.Gender.Code, null, person.DateOfBirth
                        ,gradeLevels.First(x=>x.Name == gl.GradeLevel.Name).Id, person.PersonID);
                }
                else
                    pr = ServiceLocatorSchool.PersonService.Add(email, defUserPass, person.FirstName, person.LastName, roleId, person.Gender.Code, null, person.DateOfBirth, null, person.PersonID);
                
                foreach (var personTelephone in person.PersonTelephones)
                {
                    var type = PhoneType.Home;
                    if (personTelephone.Description == descrWork)
                        type = PhoneType.Work;
                    if (personTelephone.Description == descrCell)
                        type = PhoneType.Mobile;
                    if (!string.IsNullOrEmpty(personTelephone.FormattedTelephoneNumber))
                    {
                        ServiceLocatorSchool.PhoneService.Add(pr.Id, personTelephone.FormattedTelephoneNumber, type, personTelephone.IsPrimary);
                    }
                    
                }

                var addr = person.Address ?? person.Address1;
                if (addr != null)
                {
                    ServiceLocatorSchool.AddressService.Add(pr.Id, addr.AddressLine1 + "," + addr.City + "," + addr.PostalCode + "," + addr.State, null, AddressType.Home);
                }
                else
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_NO_ADDRESS_FOR_PERSON, person.PersonID));

                if (person.Document != null)
                {
                    if (person.Document.MIMEType.ToLower().StartsWith(img))//TODO: other picture formats
                    {
                        ServiceLocatorMaster.PersonPictureService.UploadPicture(pr.Id, person.Document.Data);    
                    }
                }
            }*/
        }
        
        private void ImportSchoolYears()
        {
            /*for (int i = 0; i < sessionIds.Count; i++)
            {
                int sessionId = sessionIds[i];
                var calendar = stiEntities.AcadSessions.First(x => x.AcadSessionID == sessionId);
                if (!calendar.StartDate.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_SCHEDULE_IMPORT_START_DATE_IS_NULL, sessionId));
                    continue;
                }
                if (!calendar.EndDate.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_SCHEDULE_IMPORT_END_DATE_IS_NULL, sessionId));
                    continue;
                }
                var schoolYear = ServiceLocatorSchool.SchoolYearService.Add(calendar.AcadYear.ToString(CultureInfo.InvariantCulture),
                                                           calendar.Name, calendar.StartDate.Value, calendar.EndDate.Value, sessionId);
                
                Log.LogInfo(string.Format(ChlkResources.MARKING_PERIODS_FOR_SCHOOL_YEAR_IMPORT_START, schoolYear.Id));
                ImportMarkingPeriods(schoolYear.Id, sessionId);
                ImportDays(schoolYear.Id, sessionId);
            }*/

        }

        private void ImportDays(Guid schoolYearId, int sessionId)
        {
            var days = stiEntities.CalendarDays.Where(x => x.AcadSessionID == sessionId).ToList();

            var markingPeriods = ServiceLocatorSchool.MarkingPeriodService.GetMarkingPeriods(schoolYearId);
            var scheduleSections = new List<DateType>();
            var mpIds = markingPeriods.Select(x => x.Id).ToList();
            foreach (var id in mpIds)
            {
                scheduleSections.AddRange(ServiceLocatorSchool.ScheduleSectionService.GetSections(id));
            }
            var added = new HashSet<DateTime>();
            foreach (var day in days)
            {
                if (added.Contains(day.Date))
                    continue;
                added.Add(day.Date);
                Guid? mpId = null;
                Guid? ssId = null;
                var mp = markingPeriods.FirstOrDefault(x => x.StartDate <= day.Date && x.EndDate >= day.Date);
                if (mp != null)
                {
                    mpId = mp.Id;
                    var ss = scheduleSections.FirstOrDefault(x => x.SisId == day.DayTypeID && x.MarkingPeriodRef == mpId);
                    if (ss != null)
                        ssId = ss.Id;
                }
                ServiceLocatorSchool.CalendarDateService.Add(day.Date, day.InSchool, mpId, ssId, day.BellScheduleID);//TODO: need 2 sis id
            }
            Log.LogInfo(string.Format(ChlkResources.DAYS_FOR_SCHOOL_YEAR_IMPORTED, schoolYearId));
        }

        private void ImportMarkingPeriods(Guid schoolYearId, int sessionId)
        {
            var terms = stiEntities.Terms.Where(x => x.AcadSessionID == sessionId).ToList();
            foreach (var term in terms)
            {
                if (!term.StartDate.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_SCHEDULE_IMPORT_TERM_HAS_START_DATE_NULL, term.TermID));
                    continue;
                }
                if (!term.EndDate.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_SCHEDULE_IMPORT_TERM_HAS_END_DATE_NULL, term.TermID));
                    continue;
                }
                var markingPeriod = ServiceLocatorSchool.MarkingPeriodService.Add(schoolYearId, term.StartDate.Value,
                                                                                               term.EndDate.Value, term.Name, term.Description, 62, false, term.TermID);

                Log.LogInfo(string.Format(ChlkResources.IMPORT_SCHEDULE_SECTION_START, markingPeriod.Id));
                ImportScheduleSections(markingPeriod.Id, sessionId);
            }
        }

        private const string UNKNOWN_ROOM_NUMBER = "Unknown number";
        private void ImportRooms(int icSchoolId)
        {
            var rooms = stiEntities.Rooms.Where(x => x.SchoolID == icSchoolId).ToList();
            foreach (var room in rooms)
            {
                ServiceLocatorSchool.RoomService.AddRoom(room.RoomNumber ?? UNKNOWN_ROOM_NUMBER, room.Description, null, room.StudentCapacity ?? 0, null, room.RoomID);
            }
            ServiceLocatorSchool.RoomService.AddRoom(NO_ROOM_NUMBER, NO_ROOM_NUMBER, null, 0, null);
        }

        private void ImportScheduleSections(Guid markingPeriodId, int sessionId)
        {
            var dayTypes = stiEntities.DayTypes.Where(x => x.AcadSessionID == sessionId).ToList();
            foreach (var dayType in dayTypes)
            {
                DateType scheduleSection = ServiceLocatorSchool.ScheduleSectionService.Add(dayType.Sequence, dayType.Name, markingPeriodId, dayType.DayTypeID);
                Log.LogInfo(string.Format(ChlkResources.IMPORT_GENERAL_PERIODS_FOR_SCHEDULE_SECTION_START, scheduleSection.Id));
                ImportGeneralperiods(scheduleSection.Id, dayType.DayTypeID, dayType.AcadSessionID);
            }
        }

        private void ImportGeneralperiods(Guid scheduleSectionId, int dayTypeId, int sessionId)
        {
            //TODO: this is heuristic
            var bs = stiEntities.BellSchedules.Where(x => x.AcadSessionID == sessionId).OrderByDescending(x => x.CalendarDays.Count).FirstOrDefault();
            if (bs == null)
            {
                Log.LogWarning(string.Format(ChlkResources.ERR_STI_NO_BELL_SCHEDULE, sessionId));
                return;
            }
            var scheduledTimeSlots = stiEntities.ScheduledTimeSlots.Where(x => x.BellScheduleID == bs.BellScheduleID).ToList();
            var scheduleSection = ServiceLocatorSchool.ScheduleSectionService.GetSectionById(scheduleSectionId);
            foreach (var scheduledTimeSlot in scheduledTimeSlots)
            {
                var sisId = bs.BellScheduleID;
                var sisId2 = scheduledTimeSlot.TimeSlotID;
                if (!scheduledTimeSlot.StartTime.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_SCHEDULE_TIME_SLOT_START_TIME_NULL, sisId, sisId2));
                    continue;
                }
                if (!scheduledTimeSlot.EndTime.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_SCHEDULE_TIME_SLOT_END_TIME_NULL, sisId, sisId2));
                    continue;
                }
                ServiceLocatorSchool.PeriodService.Add(scheduleSection.MarkingPeriodRef, scheduledTimeSlot.StartTime.Value,
                                                       scheduledTimeSlot.EndTime.Value, scheduleSectionId , scheduledTimeSlot.TimeSlot.Sequence, sisId, sisId2);
            }
        }
    }

    public class SisConnectionInfo
    {
        public string DbName { get; set; }
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
    }
}
