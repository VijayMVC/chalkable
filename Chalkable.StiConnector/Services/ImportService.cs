using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.SisConnector.PublicModel;
using Chalkable.SisConnector.Services;
using Chalkable.StiConnector.Model;
using Person = Chalkable.Data.School.Model.Person;
using Room = Chalkable.Data.School.Model.Room;
using SchoolInfo = Chalkable.SisConnector.PublicModel.SchoolInfo;

namespace Chalkable.StiConnector.Services
{
    public class ImportService : SisImportService
    {
        private IServiceLocatorSchool schoolServiceLocator;
        private IServiceLocatorMaster serviceLocatorMaster;
        private StiEntities stiEntities;
        private const string NO_ROOM_NUMBER = "No Room";
        private const string SIS_DATA_BASE_CONNECTION_ERROR_FMT = "Invalid sis database connection data {0}, {1}, {2}, {3}";
        private const string CONNECTION_STRING_FMT =
            "metadata=res://*/Model.StiEntities.csdl|res://*/Model.StiEntities.ssdl|res://*/Model.StiEntities.msl;provider=System.Data.SqlClient;provider connection string=\"data source={0};initial catalog={1};persist security info=True;user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework\"";
        
        public ImportService(Guid schoolId, int sisSchoolId, IList<int> schoolYearIds, BackgroundTaskService.BackgroundTaskLog log) : base(schoolId, sisSchoolId, schoolYearIds, log)
        {
            serviceLocatorMaster = ServiceLocatorFactory.CreateMasterSysAdmin();
            schoolServiceLocator = serviceLocatorMaster.SchoolServiceLocator(schoolId);
            var sisSync = serviceLocatorMaster.SchoolService.GetSyncData(schoolId);

            string dbUrl = sisSync.SisDatabaseUrl,
                    dbName = sisSync.SisDatabaseName,
                    userName = sisSync.SisDatabaseUserName,
                    pwd = sisSync.SisDatabasePassword;
            if (dbUrl == null || dbName == null || userName == null || pwd == null)
                throw new Exception(string.Format(SIS_DATA_BASE_CONNECTION_ERROR_FMT, dbUrl, dbName, userName, pwd));
            var connectionString = string.Format(CONNECTION_STRING_FMT, dbUrl, dbName, userName, pwd);
            stiEntities = new StiEntities(connectionString);
        }

        public override IList<SchoolInfo> GetSchools()
        {
            var res = stiEntities.Schools.ToList().Select(x => new SchoolInfo
                {
                    Address = x.OrganizationAddress.AddressLine1,
                    City = x.OrganizationAddress.City,
                    Email = x.EmailAddress,
                    Name = x.Name,
                    Phone = x.TelephoneNumber,
                    PrincipalEmail = "",
                    PrincipalName = "",
                    PrincipalTitle = "",
                    SchoolId = x.SchoolID,
                    State = x.OrganizationAddress.State,
                    SchoolYears = x.AcadSessions.Select(y => new SchoolYearInfo
                    {
                        CalendarId = y.AcadSessionID,
                        EndDate = y.EndDate,
                        Name = y.Name,
                        SchoolId = y.SchoolID,
                        StartDate = y.StartDate
                    }).ToList()
                }).ToList();
            return res;
        }

        public override void ImportAttendances(DateTime? lastUpdate)
        {

            Log.LogInfo(ChlkResources.SYNC_ATTENDANCE_REASONS);
            SyncAttendanceReasons();
            Log.LogInfo(ChlkResources.IMPORT_ATTENDANCES);
            ImportAttendances(lastUpdate, SisSchoolId, null);
        }

        public override void ImportSchedule(DateTime? lastUpdate)
        {
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
            ImportClassSchoolPersons(SisSchoolYearIds);
        }

        public override void ImportPeople(DateTime? lastUpdate)
        {
            Log.LogInfo(ChlkResources.IMPORT_PERSONS_START);
            ImportGradeLevels();
            ImportSchoolPersons();
        }

        public void ImportAttendances(DateTime? lastUpdate, int stiSchoolId, IList<int> sessionIds)
        {
            /*var currentTime = DateTime.Now;
            var attendancesQuery = stiEntities.StudentPeriodAbsences.Where(x => x.AcadSession.SchoolID == stiSchoolId);
            if (lastUpdate.HasValue)
                attendancesQuery = attendancesQuery.Where(x => x.Date > lastUpdate);//TODO: need use modified date
            if (sessionIds != null)
                attendancesQuery = attendancesQuery.Where(x => sessionIds.Contains(x.AcadSessionID));
            var attendances = attendancesQuery.ToList();

            var generalPeriods = 
                schoolServiceLocator.PeriodService.GetPeriods(null, null).Where(x => x.SisId2.HasValue).ToDictionary(x => new Pair<int, Guid>(x.SisId2.Value, x.SectionRef), x => x);
            var persons = schoolServiceLocator.PersonService.GetPersons().Where(x=>x.SisId.HasValue).ToDictionary(x => x.SisId.Value, x => x);
            
            
            var reasons = ChalkableEntities.AttendanceReasons.Where(x => x.SchoolInfoRef == SchoolId).ToDictionary(x => x.Code, x => x);
            int counter = 0, success = 0;
            var days = ChalkableEntities.Dates.Where(x => x.SchoolInfoRef == SchoolId).ToDictionary(x => x.Date1, x => x);
            Log.LogWarning(string.Format(ChlkResources.ATTENDANCE_RECORDS_TO_PROCESS, attendances.Count));
            foreach (var attendance in attendances)
            {
                if (counter % 200 == 0)
                {
                    Log.LogWarning(string.Format(ChlkResources.ATTENDANCE_RECORDS_PROCESSED_ADDED, counter, success));
                    ChalkableEntities.SaveChanges();
                }
                counter++;
                if (!days.ContainsKey(attendance.Date))
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_DAY_INFO_FOR_DATE_NOT_FOUND, attendance.Date));
                    continue;
                }
                var day = days[attendance.Date];

                if (!day.MarkingPeriodRef.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_DAY_DOESNT_BELONG_TO_MARKING_PERIOD, day.Date1));
                    continue;
                }
                if (!day.ScheduleSectionRef.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_DAY_DAY_DOESNT_HAVE_SCHEDULE_SECTIONS, day.Date1));
                    continue;
                }

                var p = new Pair<int, int>(attendance.TimeSlotID, day.ScheduleSectionRef.Value);
                if (!generalPeriods.ContainsKey(p))
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_GENERAL_PERIOD_NOT_FOUND, attendance.TimeSlotID));
                    continue;
                }
                var gp = generalPeriods[p];

                if (gp.SectionRef != day.ScheduleSectionRef)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_GENERAL_PERIOD_REFERS_TO_DIFFERENT_SCHEDULE, gp.Id, day.Date1));
                    continue;
                }
                if (!schoolPersons.ContainsKey(attendance.StudentID))
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_SCHOOL_PERSON_NOT_FOUND, attendance.StudentID));
                    continue;
                }
                var schoolPerson = schoolPersons[attendance.StudentID];
                var csp = ChalkableEntities.ClassSchoolPersons.FirstOrDefault(x => x.Class.ClassGeneralPeriods.Any(y => y.GeneralPeriodRef == gp.Id) && x.SchoolPersonRef == schoolPerson.Id);
                if (csp == null)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_CANT_FIND_CLASS_SCHOOL_PERSON, gp.Id, schoolPerson.Id));
                    continue;
                }
                if (!reasons.ContainsKey(attendance.AbsenceReason.Code))
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_CANT_FIND_ATTENDANCE_REASON_CODE, attendance.AbsenceReason.Code));
                    continue;
                }
                var ar = reasons[attendance.AbsenceReason.Code];
                var classGeneralPeriod = ChalkableEntities.ClassGeneralPeriods.FirstOrDefault(x => x.GeneralPeriodRef == gp.Id && x.ClassRef == csp.ClassRef);
                if (classGeneralPeriod == null)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_CANT_FIND_CLASS_GENERAL_PERIOD, csp.ClassRef, gp.Id));
                    continue;
                }
                var a = new ClassAttendance
                {
                    AttendanceReasonRef = ar.Id,
                    ClassSchoolPersonRef = csp.Id,
                    ClassGeneralPeriodRef = classGeneralPeriod.Id,
                    Date = day.Date1,
                    TypeTyped = ar.AttendanceTypeTyped
                };
                ChalkableEntities.ClassAttendances.AddObject(a);
                success++;
            }
            var sisInfo = ChalkableEntities.SisSyncInfoes.First(x => x.SchoolInfoRef == SchoolId);
            sisInfo.LastAttendanceSync = currentTime;
            ChalkableEntities.SaveChanges();*/
        }

        private void SyncAttendanceReasons()
        {
            /*HashSet<string> codes = new HashSet<string>();
            var excuses = stiEntities.AbsenceReasons.ToList();
            foreach (var attendanceExcuse in excuses)
            {
                if (!codes.Contains(attendanceExcuse.Code))
                {
                    codes.Add(attendanceExcuse.Code);
                    if (!ChalkableEntities.AttendanceReasons.Any(x => x.SchoolInfoRef == SchoolId && x.Code == attendanceExcuse.Code))
                    {
                        AttendanceTypeEnum type;
                        if (attendanceExcuse.AbsenceCategory == "E")
                            type = AttendanceTypeEnum.Excused;
                        else
                        {
                            if (attendanceExcuse.AbsenceCategory == "U")
                                type = AttendanceTypeEnum.Absent;
                            else
                                type = AttendanceTypeEnum.Late;
                        }
                        var ar = new AttendanceReason
                        {
                            AttendanceTypeTyped = type,
                            Code = attendanceExcuse.Code,
                            Description = attendanceExcuse.Description,
                            SchoolInfoRef = SchoolId
                        };
                        ChalkableEntities.AttendanceReasons.AddObject(ar);
                        ChalkableEntities.SaveChanges();
                    }
                }
            }*/
        }

        private void ImportClassSchoolPersons(IList<int> sessionIds)
        {
            var persons = schoolServiceLocator.PersonService.GetPersons();
            var classes = schoolServiceLocator.ClassService.GetClasses(null, null, null);
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
                    schoolServiceLocator.ClassService.AddStudent(clazz.Id, person.Id);
                }
                success++;
            }
        }

        private void ImportCourses(int stiSchoolId)
        {
            var departments = serviceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
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

                schoolServiceLocator.CourseService.Add(course.ShortName, course.FullName, null,
                                                           closestDep != null ? closestDep.Second : (Guid?) null,
                                                           course.CourseID);
            }
        }

        private void ImportClasses()
        {
            var markingPeriods = schoolServiceLocator.MarkingPeriodService.GetMarkingPeriods(null).Where(x=>x.SisId.HasValue).ToList();
            var termIds = markingPeriods.Select(x => x.SisId).ToList();
            var stiCourses = stiEntities.Courses.Where(x => x.SectionTerms.Any(y => termIds.Contains(y.TermID)) && x.Active).ToList();
            var noRoom = schoolServiceLocator.RoomService.GetRooms().First(x => x.RoomNumber == NO_ROOM_NUMBER);
            int counter = 0;
            var courses = schoolServiceLocator.CourseService.GetCourses().Where(x => x.SisId.HasValue).ToDictionary(x => x.SisId.Value, x => x);
            var schoolYears = schoolServiceLocator.SchoolYearService.GetSchoolYears().Where(x => x.SisId.HasValue).ToDictionary(x => x.SisId.Value, x => x);
            var teachers = schoolServiceLocator.PersonService.GetPersons().Where(x => x.SisId.HasValue && x.RoleRef == CoreRoles.TEACHER_ROLE.Id).ToDictionary(x => x.SisId.Value, x => x);
            var gradeLevels = schoolServiceLocator.GradeLevelService.GetGradeLevels();

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

                var c = schoolServiceLocator.ClassService.Add(schoolYear.Id, course.Id, stiCourse.FullName, stiCourse.FullName, teacher.Id
                    , gradeLevels.First(x => x.Name == glName).Id, mps);
                
                
                

                var scheduledSections = stiCourse.ScheduledSections.ToList();
                foreach (var scheduledSection in scheduledSections)
                {
                    var generalPeriods = schoolServiceLocator.PeriodService.GetPeriods(null, null).Where(x => x.SisId2 == scheduledSection.TimeSlotID
                        && x.Section.SisId == scheduledSection.DayTypeID && mps.Contains(x.MarkingPeriodRef)).ToList();
                    foreach (var generalPeriod in generalPeriods)
                    {
                        Room room;
                        if (stiCourse.RoomID.HasValue)
                            room = schoolServiceLocator.RoomService.GetRooms().First(x => x.SisId == stiCourse.RoomID.Value);
                        else
                            room = noRoom;
                        schoolServiceLocator.ClassPeriodService.Add(generalPeriod.Id, c.Id, room.Id);
                    }
                    
                }
                counter++;
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.CLASS_RECORDS_PROCESSED, counter));
            }
        }

        private void ImportGradeLevels()
        {
            var existing = schoolServiceLocator.GradeLevelService.GetGradeLevels();
            var sisGradeLevels = stiEntities.GradeLevels.ToList();
            foreach (var gradeLevel in sisGradeLevels)
                if (!existing.Any(x=>x.Name == gradeLevel.Name))
                    schoolServiceLocator.GradeLevelService.AddGradeLevel(gradeLevel.Name);
        }


        private const string userEmailFmt = "user{0}_{1}@chalkable.com";
        private const string defUserPass = "tester";
        private const string descrWork = "Work";
        private const string descrCell = "cell";
        private const string img = "image";

        private void ImportSchoolPersons()
        {
            int counter = 0;
            
            var now = DateTime.UtcNow;
            var persons =
                stiEntities.People.Where(x => 
                    stiEntities.StaffSchools.Any(y=>y.SchoolID == SisSchoolId && y.StaffID == x.PersonID)
                    ||stiEntities.StudentSchools.Any(y=>y.SchoolID == SisSchoolId && y.StudentID == x.PersonID)
                    );
            var gradeLevels = schoolServiceLocator.GradeLevelService.GetGradeLevels();

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
                    pr = schoolServiceLocator.PersonService.Add(email, defUserPass, person.FirstName, person.LastName, roleId, person.Gender.Code, null, person.DateOfBirth
                        ,gradeLevels.First(x=>x.Name == gl.GradeLevel.Name).Id);
                }
                else
                    pr = schoolServiceLocator.PersonService.Add(email, defUserPass, person.FirstName, person.LastName, roleId, person.Gender.Code, null, person.DateOfBirth, null);
                
                foreach (var personTelephone in person.PersonTelephones)
                {
                    var type = PhoneType.Home;
                    if (personTelephone.Description == descrWork)
                        type = PhoneType.Work;
                    if (personTelephone.Description == descrCell)
                        type = PhoneType.Mobile;
                    if (!string.IsNullOrEmpty(personTelephone.FormattedTelephoneNumber))
                    {
                        schoolServiceLocator.PhoneService.Add(pr.Id, personTelephone.FormattedTelephoneNumber, type, personTelephone.IsPrimary);
                    }
                    
                }

                var addr = person.Address ?? person.Address1;
                if (addr != null)
                {
                    schoolServiceLocator.AddressSerivce.Add(pr.Id, addr.AddressLine1 + "," + addr.City + "," + addr.PostalCode + "," + addr.State, null, AddressType.Home);
                }
                else
                    Log.LogWarning(string.Format(ChlkResources.ERR_STI_NO_ADDRESS_FOR_PERSON, person.PersonID));

                if (person.Document != null && person.Document.MIMEType.ToLower().StartsWith(img))//TODO: other picture formats
                {
                    //TODO: implement
                    /*var p = new Picture()
                    {
                        Content = person.Document.Data
                    };
                    var pp = new PersonPicture()
                    {
                        Picture = p,
                        PersonInfo = sp.PersonInfo
                    };
                    ChalkableEntities.Pictures.AddObject(p);
                    ChalkableEntities.PersonPictures.AddObject(pp);*/
                }
            }
        }
        
        private void ImportSchoolYears(IList<int> sessionIds)
        {
            for (int i = 0; i < sessionIds.Count; i++)
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
                var schoolYear = schoolServiceLocator.SchoolYearService.Add(calendar.AcadYear.ToString(CultureInfo.InvariantCulture),
                                                           calendar.Name, calendar.StartDate.Value, calendar.EndDate.Value);
                
                Log.LogInfo(string.Format(ChlkResources.MARKING_PERIODS_FOR_SCHOOL_YEAR_IMPORT_START, schoolYear.Id));
                ImportMarkingPeriods(schoolYear.Id, sessionId);
                ImportDays(schoolYear.Id, sessionId);
            }

        }

        private void ImportDays(Guid schoolYearId, int sessionId)
        {
            var days = stiEntities.CalendarDays.Where(x => x.AcadSessionID == sessionId).ToList();

            var markingPeriods = schoolServiceLocator.MarkingPeriodService.GetMarkingPeriods(schoolYearId);
            var scheduleSections = new List<ScheduleSection>();
            var mpIds = markingPeriods.Select(x => x.Id).ToList();
            foreach (var id in mpIds)
            {
                scheduleSections.AddRange(schoolServiceLocator.ScheduleSectionService.GetSections(id));
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
                schoolServiceLocator.CalendarDateService.Add(day.Date, day.InSchool, mpId, ssId, day.BellScheduleID);//TODO: need 2 sis id
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
                var markingPeriod = schoolServiceLocator.MarkingPeriodService.Add(schoolYearId, term.StartDate.Value,
                                                                                               term.EndDate.Value, term.Name, term.Description, 62);

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
                schoolServiceLocator.RoomService.AddRoom(room.RoomNumber ?? UNKNOWN_ROOM_NUMBER, room.Description, null, room.StudentCapacity??0, null, room.RoomID);
            }
            schoolServiceLocator.RoomService.AddRoom(NO_ROOM_NUMBER, NO_ROOM_NUMBER, null, 0, null);
        }

        private void ImportScheduleSections(Guid markingPeriodId, int sessionId)
        {
            var dayTypes = stiEntities.DayTypes.Where(x => x.AcadSessionID == sessionId).ToList();
            foreach (var dayType in dayTypes)
            {
                ScheduleSection scheduleSection = schoolServiceLocator.ScheduleSectionService.Add(dayType.Sequence, dayType.Name, markingPeriodId, dayType.DayTypeID);
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
            var scheduleSection = schoolServiceLocator.ScheduleSectionService.GetSectionById(scheduleSectionId);
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
                schoolServiceLocator.PeriodService.Add(scheduleSection.MarkingPeriodRef,scheduledTimeSlot.StartTime.Value,
                                                       scheduledTimeSlot.EndTime.Value, scheduleSectionId , scheduledTimeSlot.TimeSlot.Sequence, sisId, sisId2);
            }
        }
    }
}
