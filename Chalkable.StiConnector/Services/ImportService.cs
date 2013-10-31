using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Model;

namespace Chalkable.StiConnector.Services
{
    public class ImportService
    {
        private StiEntities stiEntities;
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

        public void Import()
        {
            ImportSchools();
            Log.LogInfo(ChlkResources.IMPORT_PERSONS_START);
            ImportAddresses();
            ImportSchoolPersons();
            ImportPhones();
            Log.LogInfo("Start importing grade levels");
            ImportGradeLevels();
            Log.LogInfo(ChlkResources.SCHOOL_YEAR_IMPORT_START);
            ImportSchoolYears();
            ImportStudentSchoolYears();
            
            Log.LogInfo("Start importing marking periods");
            ImportMarkingPeriods();
            Log.LogInfo("Start importing day types");
            ImportDayTypes();
            Log.LogInfo("Start importing days");
            ImportDays();
            
            Log.LogInfo(ChlkResources.IMPORT_ROOMS_START);
            ImportRooms();
            Log.LogInfo(ChlkResources.IMPORT_COURSES_START);
            ImportCourses();
            Log.LogInfo("Start importing periods");
            ImportPeriods();
            Log.LogInfo(ChlkResources.IMPORT_CLASS_PERIODS_START);
            ImportClassPeriods();
            Log.LogInfo(ChlkResources.IMPORT_CLASS_SCHOOL_PERSONS_START);
            ImportClassSchoolPersons();
        }
        
        private void ImportSchools()
        {
            if (ServiceLocatorSchool.SchoolService.GetSchools().Count > 0)
                return;//TODO: untill versioning is implemented
            foreach (var school in stiEntities.Schools)
            {
                ServiceLocatorSchool.SchoolService.Add(new Data.School.Model.School
                    {
                        Id = school.SchoolID,
                        IsActive = school.IsActive,
                        IsPrivate = school.IsPrivate,
                        Name = school.Name
                    });
            }
        }


        private void ImportClassSchoolPersons()
        {
            var studentSchedules = stiEntities.StudentSchedules.ToList();
            int counter = 0, success = 0;
            foreach (var studentSchedule in studentSchedules)
            {
                counter++;
                ServiceLocatorSchool.ClassService.AddStudent(studentSchedule.SectionID, studentSchedule.StudentID);
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.ROSTER_RECORDS_PROCESSED_ADDED, counter, success));
                
                success++;
            }
        }

        private void ImportCourses()
        {
            var departments = ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
            var sep = new[] { ',' };
            var departmenPairs = new List<Pair<string, Guid>>();

            foreach (var chalkableDepartment in departments)
            {
                departmenPairs.AddRange(chalkableDepartment.Keywords.ToLower().Split(sep, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Pair<string, Guid>(x, chalkableDepartment.Id)));
            }

            var courses = stiEntities.Courses.ToList();
            foreach (var course in courses)
                if (course.AcadSessionID.HasValue)
            {
                if (!course.PrimaryTeacherID.HasValue)
                {
                    Log.LogWarning(string.Format("No primary teacher for class {0}", course.CourseID));
                    continue;
                }
                var glId = course.MaxGradeLevelID ?? course.MinGradeLevelID;
                if (!glId.HasValue)
                {
                    Log.LogWarning(string.Format("No grade level for class {0}", course.CourseID));
                    continue;
                }
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
                ServiceLocatorSchool.ClassService.Add(course.CourseID, course.AcadSessionID.Value
                                                      , closestDep != null ? closestDep.Second : (Guid?) null,
                                                      course.ShortName, course.FullName, course.PrimaryTeacherID.Value,
                                                      glId.Value);

                    
            }
        }

        private void ImportClassPeriods()
        {
            var scheduledSections = stiEntities.ScheduledSections.ToList();
            foreach (var scheduledSection in scheduledSections)
            {
                ServiceLocatorSchool.ClassPeriodService.Add(0, scheduledSection.TimeSlotID, scheduledSection.SectionID, scheduledSection.Course.RoomID,
                                                            scheduledSection.DayTypeID);
            }
        }

        private void ImportGradeLevels()
        {
            var existing = ServiceLocatorSchool.GradeLevelService.GetGradeLevels();
            if (existing.Count > 0)
                return;//TODO: untill versioning is implemented
            var sisGradeLevels = stiEntities.GradeLevels.ToList();
            foreach (var gradeLevel in sisGradeLevels)
                if (!existing.Any(x => x.Name == gradeLevel.Name))
                {
                    ServiceLocatorSchool.GradeLevelService.AddGradeLevel(gradeLevel.GradeLevelID, gradeLevel.Name, gradeLevel.Sequence);
                }
        }


        private const string userEmailFmt = "user{0}_{1}@chalkable.com";
        private const string defUserPass = "tester";
        private const string descrWork = "Work";
        private const string descrCell = "cell";
        private const string img = "image";

        private void ImportSchoolPersons()
        {
            if (ServiceLocatorSchool.SchoolPersonService.GetAll().Count > 0)
                return;//TODO: untill versioning is implemented

            int counter = 0;
            var persons = stiEntities.People.ToList();

            foreach (var person in persons)
            {
                counter++;
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.USERS_PROCESSED, counter));
                var email = string.Format(userEmailFmt, person.PersonID, ServiceLocatorSchool.Context.DistrictId);
                var assignments = new List<SchoolAssignmentInfo>();
                assignments.AddRange(stiEntities.StudentSchools.Where(x=>x.StudentID == person.PersonID).Select(x=>new SchoolAssignmentInfo{Role = CoreRoles.STUDENT_ROLE.Id, SchoolId = x.SchoolID}));
                assignments.AddRange(stiEntities.StaffSchools.Where(x => x.StaffID == person.PersonID).Select(x => new SchoolAssignmentInfo { Role = CoreRoles.TEACHER_ROLE.Id, SchoolId = x.SchoolID }));
                //TODO: what about admins? probably will be resolved by API
                
                try
                {
                    ServiceLocatorSchool.PersonService.Add(person.PersonID, email, defUserPass, person.FirstName, person.LastName
                        , person.Gender != null ? person.Gender.Code : "M", null, person.DateOfBirth, person.PhysicalAddressID, assignments);
                }
                catch (Exception ex)
                {
                    Log.LogError(string.Format(ex.Message));
                    Log.LogError(string.Format(ex.StackTrace));
                }
                
            }
        }

        private void ImportStudentSchoolYears()
        {
            if (ServiceLocatorSchool.SchoolYearService.GetStudentAssignments().Count > 0)
                return;//TODO: untill versioning is implemented
            var studentAcadSessions = stiEntities.StudentAcadSessions.ToList();
            foreach (var studentAcadSession in studentAcadSessions)
            {
                if (studentAcadSession.GradeLevelID.HasValue)
                    ServiceLocatorSchool.SchoolYearService.AssignStudent(studentAcadSession.AcadSessionID, studentAcadSession.StudentID, studentAcadSession.GradeLevelID.Value);
                else
                    Log.LogWarning(string.Format("No grade level for student {0} school year {1}", studentAcadSession.StudentID, studentAcadSession.AcadSessionID));
            }
        }

        private void ImportPhones()
        {
            if (ServiceLocatorSchool.PhoneService.GetPhones().Count > 0)
                return;//TODO: untill versioning is implemented
            var phones = stiEntities.PersonTelephones.ToList();
            foreach (var pt in phones)
            {
                var type = PhoneType.Home;
                if (pt.Description == descrWork)
                    type = PhoneType.Work;
                if (pt.Description == descrCell)
                    type = PhoneType.Mobile;
                if (!string.IsNullOrEmpty(pt.FormattedTelephoneNumber))
                {
                    ServiceLocatorSchool.PhoneService.Add(pt.TelephoneNumber, pt.PersonID, pt.FormattedTelephoneNumber, type, pt.IsPrimary);
                }
            }
        }

        private void ImportAddresses()
        {
            if (ServiceLocatorSchool.AddressService.GetAddress().Count > 0)
                return;//TODO: untill versioning is implemented
            var adds = stiEntities.Addresses.ToList();
            foreach (var address in adds)
            {
                ServiceLocatorSchool.AddressService.Add(new AddressInfo
                    {
                        AddressLine1 = address.AddressLine1,
                        AddressLine2 = address.AddressLine2,
                        AddressNumber = address.AddressNumber,
                        StreetNumber = address.StreetNumber,
                        City = address.City,
                        PostalCode = address.PostalCode,
                        State = address.State,
                        Country = address.Country,
                        CountyId = address.CountryID,
                        Id = address.AddressID
                    });
            }
        }

        
        private void ImportSchoolYears()
        {
            if (ServiceLocatorSchool.SchoolYearService.GetSortedYears().Count > 0)
                return;//TODO: untill versioning is implemented
            foreach (var calendar in stiEntities.AcadSessions)
            {
                if (!calendar.StartDate.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_SCHEDULE_IMPORT_START_DATE_IS_NULL, calendar.AcadSessionID));
                    continue;
                }
                if (!calendar.EndDate.HasValue)
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_SCHEDULE_IMPORT_END_DATE_IS_NULL, calendar.AcadSessionID));
                    continue;
                }
                ServiceLocatorSchool.SchoolYearService.Add(calendar.AcadSessionID, calendar.SchoolID, calendar.AcadYear.ToString(CultureInfo.InvariantCulture),
                                                           calendar.Name, calendar.StartDate.Value, calendar.EndDate.Value);
            }
        }

        private void ImportDays()
        {
            var schoolYears = ServiceLocatorSchool.SchoolYearService.GetSchoolYears();
            if (schoolYears.Any(x => ServiceLocatorSchool.CalendarDateService.GetLastDays(x.Id, false, null, null).Count > 0))
                return;//TODO: untill versioning is implemented

            var days = stiEntities.CalendarDays.ToList();
            foreach (var day in days)
            {
                ServiceLocatorSchool.CalendarDateService.Add(day.Date,  day.InSchool, day.AcadSessionID, day.DayTypeID);
            }
        }

        private void ImportMarkingPeriods()
        {
            if (ServiceLocatorSchool.MarkingPeriodService.GetMarkingPeriods(null).Count > 0)
                return;//TODO: untill versioning is implemented

            var terms = stiEntities.Terms.ToList();
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
                ServiceLocatorSchool.MarkingPeriodService.Add(term.TermID, term.AcadSessionID, term.StartDate.Value, term.EndDate.Value, term.Name, term.Description, 62);
            }
        }

        private const string UNKNOWN_ROOM_NUMBER = "Unknown number";
        private void ImportRooms()
        {
            if (ServiceLocatorSchool.RoomService.GetRooms().Count > 0)
                return;//TODO: untill versioning is implemented
            var rooms = stiEntities.Rooms.ToList();
            foreach (var room in rooms)
            {
                ServiceLocatorSchool.RoomService.AddRoom(room.RoomID, room.SchoolID, room.RoomNumber ?? UNKNOWN_ROOM_NUMBER, room.Description, null, room.StudentCapacity ?? 0, null);
            }
        }

        private void ImportDayTypes()
        {
            var schoolYears = ServiceLocatorSchool.SchoolYearService.GetSchoolYears();
            if (schoolYears.Any(x => ServiceLocatorSchool.DayTypeService.GetSections(x.Id).Count > 0))
                return;//TODO: untill versioning is implemented
            
            var dayTypes = stiEntities.DayTypes.ToList();
            foreach (var dayType in dayTypes)
            {
                var dt = ServiceLocatorSchool.DayTypeService.Add(dayType.DayTypeID, dayType.Sequence, dayType.Name, dayType.AcadSessionID);
                Log.LogInfo(string.Format(ChlkResources.IMPORT_GENERAL_PERIODS_FOR_SCHEDULE_SECTION_START, dt.Id));
            }
        }

        private void ImportPeriods()
        {
            var schoolYears = ServiceLocatorSchool.SchoolYearService.GetSchoolYears();
            if (schoolYears.Any(x => ServiceLocatorSchool.PeriodService.GetPeriods(x.Id).Count > 0))
                return;//TODO: untill versioning is implemented

            var periods = stiEntities.TimeSlots.ToList();
            foreach (var timeSlot in periods)
            {
                var sts = timeSlot.ScheduledTimeSlots.First();
                ServiceLocatorSchool.PeriodService.Add(timeSlot.TimeSlotID, timeSlot.AcadSessionID, sts.StartTime ?? 0, sts.EndTime ?? 0, timeSlot.Sequence);
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
