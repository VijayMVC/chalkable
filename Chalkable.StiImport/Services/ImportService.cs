using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Model;
using Standard = Chalkable.Data.School.Model.Standard;
using StandardSubject = Chalkable.Data.School.Model.StandardSubject;

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
            if (ServiceLocatorSchool.SchoolPersonService.GetAll().Count > 0)
                return;

            var district = ServiceLocatorMaster.DistrictService.GetByIdOrNull(ServiceLocatorSchool.Context.DistrictId.Value);
            district.SisDistrictId = stiEntities.Schools.First().DistrictGuid;
            ServiceLocatorMaster.DistrictService.Update(district);

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
            ImportStandards();
            ImportMarkingPeriodClasses();
            Log.LogInfo("Import class announcement types");
            ImportClassAnnouncementTypes();
            Log.LogInfo("Start importing periods");
            ImportPeriods();
            Log.LogInfo(ChlkResources.IMPORT_CLASS_PERIODS_START);
            ImportClassPeriods();
            Log.LogInfo(ChlkResources.IMPORT_CLASS_SCHOOL_PERSONS_START);
            ImportClassPersons();
            Log.LogInfo("Import attendance reasons");
            ImportAttendanceReasons();
        }

        private void ImportStandards()
        {
            var ss = stiEntities.StandardSubjects.ToList().Select(x=>new StandardSubject
                {
                    AdoptionYear = x.AdoptionYear,
                    Id = x.StandardSubjectID,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    Name = x.Name
                }).ToList();
            ServiceLocatorSchool.StandardService.AddStandardSubjects(ss);

            var sts = stiEntities.Standards.ToList().Select(x => new Standard
                {
                    Description = x.Description,
                    Id = x.StandardID,
                    IsActive = x.IsActive,
                    LowerGradeLevelRef = x.LowerGradeLevelID,
                    Name = x.Name,
                    ParentStandardRef = x.ParentStandardID,
                    StandardSubjectRef = x.StandardSubjectID,
                    UpperGradeLevelRef = x.UpperGradeLevelID
                }).ToList();
            ServiceLocatorSchool.StandardService.AddStandards(sts);

            var classes = ServiceLocatorSchool.ClassService.GetClasses(null);
            var cs = stiEntities.CourseStandards.ToList()
                .Where(x=>classes.Any(y=>y.Id == x.CourseID))
                .Select(x => new ClassStandard
                {
                    ClassRef = x.CourseID,
                    StandardRef = x.StandardID
                }).ToList();
            
        }

        private void ImportClassAnnouncementTypes()
        {
            var types = stiEntities.ActivityCategories.ToList().Select(x => new ClassAnnouncementType
                {
                    Id = x.ActivityCategoryID,
                    ClassRef = x.SectionID,
                    Description = x.Description,
                    Gradable = true,
                    Name = x.Name,
                    Percentage = (int)(x.Percentage ?? 0)//TODO: use decimal?
                }).ToList();
            var chalkableTypes = ChalkableAnnouncementType.All;
            foreach (var classAnnouncementType in types)
            {
                var ct =
                    chalkableTypes.FirstOrDefault(
                        x => x.Keywords.Split(',').Any(y => classAnnouncementType.Name.ToLower().Contains(y)));
                if (ct != null)
                    classAnnouncementType.ChalkableAnnouncementTypeRef = ct.Id;
            }
            ServiceLocatorSchool.ClassClassAnnouncementTypeService.Add(types);
        }

        private void ImportAttendanceReasons()
        {
            var reasons = stiEntities.AbsenceReasons.ToList();
            var rs = reasons.Select(x => new AttendanceReason
                {
                    Category = x.AbsenceCategory,
                    Code = x.Code,
                    Description = x.Description,
                    Id = x.AbsenceReasonID,
                    IsActive = x.IsActive,
                    IsSystem = x.IsSystem,
                    Name = x.Name
                }).ToList();
            ServiceLocatorSchool.AttendanceReasonService.Add(rs);


            var absenceLevelReasons = stiEntities.AbsenceLevelReasons.ToList()
                                                 .Select(x => new AttendanceLevelReason
                                                     {
                                                         Id = x.AbsenceLevelReasonID,
                                                         AttendanceReasonRef = x.AbsenceReasonID,
                                                         IsDefault = x.IsDefaultReason,
                                                         Level = x.AbsenceLevel
                                                     }).ToList();
            ServiceLocatorSchool.AttendanceReasonService.AddAttendanceLevelReasons(absenceLevelReasons);
        }

        private void ImportSchools()
        {
            var schools = stiEntities.Schools;
            foreach (var school in schools)
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

        private void ImportClassPersons()
        {
            var studentSchedules = stiEntities.StudentSchedules.ToList()
                .Select(x=>new ClassPerson
                    {
                        ClassRef = x.SectionID,
                        PersonRef = x.StudentID,
                        SchoolRef = x.Course1.AcadSession.SchoolID
                    }).ToList();
            ServiceLocatorSchool.ClassService.AddStudents(studentSchedules);
        }

        private void ImportCourses()
        {
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToList();
            var departments = ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
            var sep = new[] { ',' };
            var departmenPairs = new List<Pair<string, Guid>>();

            foreach (var chalkableDepartment in departments)
            {
                departmenPairs.AddRange(chalkableDepartment.Keywords.ToLower().Split(sep, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Pair<string, Guid>(x, chalkableDepartment.Id)));
            }

            var courses = stiEntities.Courses.ToList()
                .Where(x=>years.Any(y=>x.AcadSessionID == y.Id));
            var classes = new List<Class>();
            foreach (var course in courses)
            {
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
                classes.Add(new Class
                    {
                        ChalkableDepartmentRef = closestDep != null ? closestDep.Second : (Guid?)null,
                        Description = course.FullName,
                        GradeLevelRef = glId.Value,
                        Id =  course.CourseID,
                        Name = course.ShortName,
                        SchoolRef = course.AcadSession != null ? course.AcadSession.SchoolID : (int?)null,
                        SchoolYearRef = course.AcadSessionID,
                        TeacherRef = course.PrimaryTeacherID
                    });
            }

            ServiceLocatorSchool.ClassService.Add(classes);
        }

        private void ImportMarkingPeriodClasses()
        {
            var classes = ServiceLocatorSchool.ClassService.GetClasses(null);
            var mps = ServiceLocatorSchool.MarkingPeriodService.GetMarkingPeriods(null);
            var cts = stiEntities.SectionTerms.ToList()
                .Where(x=>classes.Any(y=>y.Id == x.SectionID))
                .Where(x=>mps.Any(y=>y.Id == x.TermID))
                .Select(x=> new MarkingPeriodClass
                    {
                        ClassRef = x.SectionID,
                        MarkingPeriodRef = x.TermID,
                        SchoolRef = x.Term.AcadSession.SchoolID
                    }).ToList();
            ServiceLocatorSchool.ClassService.AssignClassToMarkingPeriod(cts);
        }

        private void ImportClassPeriods()
        {
            var classes = ServiceLocatorSchool.ClassService.GetClasses(null);
            var scheduledSections = stiEntities.ScheduledSections.ToList()
                .Where(x=>classes.Any(y=>y.Id == x.SectionID))
                .Select(x=>new ClassPeriod
                    {
                        ClassRef = x.SectionID,
                        DayTypeRef = x.DayTypeID,
                        PeriodRef = x.TimeSlotID,
                        RoomRef = x.Course.RoomID,
                        SchoolRef = x.DayType.AcadSession.SchoolID
                    }).ToList();
            ServiceLocatorSchool.ClassPeriodService.Add(scheduledSections);
        }

        private void ImportGradeLevels()
        {
            var sisGradeLevels = stiEntities.GradeLevels.ToList();
            foreach (var gradeLevel in sisGradeLevels)
            {
                ServiceLocatorSchool.GradeLevelService.AddGradeLevel(gradeLevel.GradeLevelID, gradeLevel.Name, gradeLevel.Sequence);
            }
        }


        private const string USER_EMAIL_FMT = "user{0}_{1}@chalkable.com";
        private const string DEF_USER_PASS = "Qwerty1@";
        private const string DESCR_WORK = "Work";
        private const string DESCR_CELL = "cell";
        private const string IMG = "image";

        private void ImportSchoolPersons()
        {
            int counter = 0;
            var persons = stiEntities.People.ToList();
            var assignments = new List<SchoolPerson>();
            var ps = new List<PersonInfo>();
            foreach (var person in persons)
            {
                counter++;
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.USERS_PROCESSED, counter));
                var email = string.Format(USER_EMAIL_FMT, person.PersonID, ServiceLocatorSchool.Context.DistrictId);

                var personAssignments =
                    stiEntities.StudentSchools.Where(x => x.StudentID == person.PersonID).Select(x => new SchoolPerson() { RoleRef = CoreRoles.STUDENT_ROLE.Id, SchoolRef = x.SchoolID, PersonRef = person.PersonID })
                               .ToList();

                personAssignments.AddRange(
                    stiEntities.StaffSchools.Where(x => x.StaffID == person.PersonID).ToList()
                    .Where(x=>!personAssignments.Any(y=>x.StaffID == y.PersonRef && x.SchoolID == y.SchoolRef))
                    .Select(x => new SchoolPerson { RoleRef = CoreRoles.TEACHER_ROLE.Id, SchoolRef = x.SchoolID, PersonRef = person.PersonID})
                );
                assignments.AddRange(personAssignments);
                
                //TODO: what about admins? probably will be resolved by API
                string userName = null;
                if (person.Staff != null && person.Staff.User != null)
                    userName = person.Staff.User.UserName;
                else if (person.Student != null && person.Student.User != null)
                    userName = person.Student.User.UserName;

                ps.Add(new PersonInfo
                    {
                        Active = true,
                        AddressRef = person.PhysicalAddressID,
                        BirthDate = person.DateOfBirth,
                        Email = email,
                        FirstName = person.FirstName,
                        Gender = person.Gender != null ? person.Gender.Code : "M",
                        Id = person.PersonID,
                        LastName = person.LastName,
                        Password = ServiceLocatorMaster.UserService.PasswordMd5(DEF_USER_PASS),
                        SisUserName = userName
                    });
            }

            ServiceLocatorSchool.PersonService.Add(ps, assignments);
        }

        private void ImportStudentSchoolYears()
        {
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToList();
            var assignments = stiEntities.StudentAcadSessions
                .Where(x=>x.GradeLevelID.HasValue)
                .ToList()
                .Where(x=>years.Any(y=>y.Id == x.AcadSessionID))
                .Select(x=>new StudentSchoolYear
                    {
                        GradeLevelRef = x.GradeLevelID.Value,
                        SchoolYearRef = x.AcadSessionID,
                        StudentRef = x.StudentID
                    }).ToList();
            ServiceLocatorSchool.SchoolYearService.AssignStudent(assignments);
        }

        private void ImportPhones()
        {
            var phones = stiEntities.PersonTelephones.ToList();
            foreach (var pt in phones)
            {
                var type = PhoneType.Home;
                if (pt.Description == DESCR_WORK)
                    type = PhoneType.Work;
                if (pt.Description == DESCR_CELL)
                    type = PhoneType.Mobile;
                if (!string.IsNullOrEmpty(pt.FormattedTelephoneNumber))
                {
                    ServiceLocatorSchool.PhoneService.Add(pt.TelephoneNumber, pt.PersonID, pt.FormattedTelephoneNumber, type, pt.IsPrimary);
                }
            }
        }

        private void ImportAddresses()
        {
            var adds = stiEntities.Addresses.ToList();
            var addressInfos = adds.Select(x => new AddressInfo
                {
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    AddressNumber = x.AddressNumber,
                    StreetNumber = x.StreetNumber,
                    City = x.City,
                    PostalCode = x.PostalCode,
                    State = x.State,
                    Country = x.Country,
                    CountyId = x.CountryID,
                    Id = x.AddressID
                }).ToList();
            ServiceLocatorSchool.AddressService.Add(addressInfos);
        }

        
        private void ImportSchoolYears()
        {
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
                ServiceLocatorSchool.SchoolYearService.Add(calendar.AcadSessionID, calendar.SchoolID, calendar.Name,
                                                           calendar.Description, calendar.StartDate.Value, calendar.EndDate.Value);
            }
        }

        private void ImportDays()
        {
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToList();
            var days = stiEntities.CalendarDays
                .ToList()
                .Where(x=>years.Any(y=>y.Id == x.AcadSessionID))
                .Select(x=>new Date
                    {
                        DayTypeRef = x.DayTypeID,
                        IsSchoolDay = x.InSchool,
                        SchoolRef = x.AcadSession.SchoolID,
                        SchoolYearRef = x.AcadSessionID,
                        Day = x.Date
                    }).ToList();
            ServiceLocatorSchool.CalendarDateService.Add(days);
        }

        private void ImportMarkingPeriods()
        {
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
            var rooms = stiEntities.Rooms.ToList();
            foreach (var room in rooms)
            {
                ServiceLocatorSchool.RoomService.AddRoom(room.RoomID, room.SchoolID, room.RoomNumber ?? UNKNOWN_ROOM_NUMBER, room.Description, null, room.StudentCapacity ?? 0, null);
            }
        }

        private void ImportDayTypes()
        {
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToList();
            var dayTypes = stiEntities.DayTypes
                                      .ToList()
                                      .Where(x => years.Any(y => y.Id == x.AcadSessionID));
            foreach (var dayType in dayTypes)
            {
                var dt = ServiceLocatorSchool.DayTypeService.Add(dayType.DayTypeID, dayType.Sequence, dayType.Name, dayType.AcadSessionID);
                Log.LogInfo(string.Format(ChlkResources.IMPORT_GENERAL_PERIODS_FOR_SCHEDULE_SECTION_START, dt.Id));
            }
        }

        private void ImportPeriods()
        {
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToList();
            var periods = stiEntities.TimeSlots.ToList()
                .Where(x=>years.Any(y=>y.Id == x.AcadSessionID));
            
            foreach (var timeSlot in periods)
            {
                var sts = timeSlot.ScheduledTimeSlots.FirstOrDefault();
                int startTime = 0;
                int endTime = 1;
                if (sts != null)
                {
                    startTime = sts.StartTime ?? 0;
                    endTime = sts.EndTime ?? 1;
                }
                ServiceLocatorSchool.PeriodService.Add(timeSlot.TimeSlotID, timeSlot.AcadSessionID, startTime, endTime, timeSlot.Sequence);
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
