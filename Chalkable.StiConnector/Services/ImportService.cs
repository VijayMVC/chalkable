using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.SisConnector.PublicModel;
using Chalkable.SisConnector.Services;
using Chalkable.StiConnector.Model;
using Address = Chalkable.StiConnector.Model.Address;
//using Address = Chalkable.Domain.Model.Address;
using SchoolInfo = Chalkable.SisConnector.PublicModel.SchoolInfo;

namespace Chalkable.StiConnector.Services
{
    public class ImportService : SisImportService
    {
        private IServiceLocatorSchool schoolServiceLocator;
        private IServiceLocatorMaster serviceLocatorMaster;
        private StiEntities stiEntities;
        private const string NO_ROOM_NUMBER = "No Room";
        private const string sisDataBaseConnectionErrorFmt = "Invalid sis database connection data {0}, {1}, {2}, {3}";

        private const string connectionStringFmt =
            "metadata=res://#1#Model.StiEntities.csdl|res://#1#Model.StiEntities.ssdl|res://#1#Model.StiEntities.msl;provider=System.Data.SqlClient;provider connection string=\"data source={0};initial catalog={1};persist security info=True;user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework\"";
        
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
                throw new Exception(string.Format(sisDataBaseConnectionErrorFmt, dbUrl, dbName, userName, pwd));
            var connectionString = string.Format(connectionStringFmt, dbUrl, dbName, userName, pwd);
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
            ImportSchoolPersons();
        }

        public void ImportAttendances(DateTime? lastUpdate, int stiSchoolId, IList<int> sessionIds)
        {
            var currentTime = DateTime.Now;
            var attendancesQuery = stiEntities.StudentPeriodAbsences.Where(x => x.AcadSession.SchoolID == stiSchoolId);
            if (lastUpdate.HasValue)
                attendancesQuery = attendancesQuery.Where(x => x.Date > lastUpdate);//TODO: need use modified date
            if (sessionIds != null)
                attendancesQuery = attendancesQuery.Where(x => sessionIds.Contains(x.AcadSessionID));
            var attendances = attendancesQuery.ToList();

            var generalPeriods = 
                schoolServiceLocator.PeriodService.GetPeriods(null, null).Where(x => x.SisId2.HasValue).ToDictionary(x => new Pair<int, Guid>(x.SisId2.Value, x.SectionRef), x => x);
            var persons = schoolServiceLocator.PersonService.GetPersons().Where(x=>x.SisId.HasValue).ToDictionary(x => x.SisId.Value, x => x);
            
            
            /*var reasons = ChalkableEntities.AttendanceReasons.Where(x => x.SchoolInfoRef == SchoolId).ToDictionary(x => x.Code, x => x);
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
            
           /* var departments = ChalkableEntities.ChalkableDepartments.ToList();
            char[] sep = new[] { ',' };
            List<Pair<string, int>> departmenPairs = new List<Pair<string, int>>();

            foreach (var chalkableDepartment in departments)
            {
                departmenPairs.AddRange(chalkableDepartment.Keywords.ToLower().Split(sep, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Pair<string, int>(x, chalkableDepartment.Id)));
            }

            var courses = stiEntities.Courses.Where(x => x.AcadSession.SchoolID == stiSchoolId).ToList();
            foreach (var course in courses)
            {
                string name = course.CourseType.Name.ToLower();
                int minDist = int.MaxValue;
                Pair<string, int> closestDep = null;
                for (int i = 0; i < departmenPairs.Count; i++)
                {
                    var d = StringTools.LevenshteinDistance(name, departmenPairs[i].First);
                    if (d < minDist && (d <= 4 || d <= 0.3 * name.Length + 2))
                    {
                        minDist = d;
                        closestDep = departmenPairs[i];
                    }
                }

                var ci = new CourseInfo
                {
                    SisId = course.CourseID,
                    Description = course.FullName,
                    Code = course.ShortName,
                    Title = course.FullName,
                    SchoolInfoRef = SchoolId,
                    ChalkableDepartmentRef = closestDep != null ? closestDep.Second : (int?)null
                };
                ChalkableEntities.CourseInfoes.AddObject(ci);
            }
            ChalkableEntities.SaveChanges();*/
        }

        private void ImportClasses()
        {
            /*var markingPeriods = ChalkableEntities.MarkingPeriods.Where(x => x.SchoolYear.SchoolInfoRef == SchoolId && x.SisId.HasValue).ToList();
            var termIds = markingPeriods.Select(x => x.SisId).ToList();
            var stiCourses = stiEntities.Courses.Where(x => x.SectionTerms.Any(y => termIds.Contains(y.TermID)) && x.Active).ToList();
            var noRoom = ChalkableEntities.RoomInfoes.First(x => x.SchoolInfoRef == SchoolId && x.RoomNumber == NO_ROOM_NUMBER);
            var staffCourses = new Dictionary<Pair<int, int>, StaffCourse>();
            int counter = 0;
            var courses = ChalkableEntities.CourseInfoes.Where(x => x.SchoolInfoRef == SchoolId).ToDictionary(x => x.SisId.Value, x => x);
            var schoolYears = ChalkableEntities.SchoolYears.Where(x => x.SchoolInfoRef == SchoolId).ToDictionary(x => x.SisId.Value, x => x);
            var teachers = ChalkableEntities.SchoolPersons.Where(x => x.SchoolInfoRef == SchoolId && x.RoleRef == CoreRoles.TEACHER_ROLE.Id).ToDictionary(x => x.SisId.Value, x => x);

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
                var p = new Pair<int, int>(course.Id, teacher.Id);
                StaffCourse staffCourse;
                if (staffCourses.ContainsKey(p))
                {
                    staffCourse = staffCourses[p];
                }
                else
                {
                    staffCourse = new StaffCourse
                    {
                        CourseInfoRef = course.Id,
                        SchoolPersonRef = teacher.Id
                    };
                    staffCourses.Add(p, staffCourse);
                    ChalkableEntities.StaffCourses.AddObject(staffCourse);
                }
                var c = new Class
                {
                    Name = stiCourse.FullName,
                    Description = stiCourse.FullName,
                    CourseInfoRef = course.Id,
                    SchoolYearRef = schoolYear.Id,
                    SisId = stiCourse.CourseID,
                    StaffCourse = staffCourse,
                    GradeLevelRef = Math.Max((stiCourse.MaxGradeLevelID ?? stiCourse.MinGradeLevelID ?? 0) - 5, 1)
                };
                ChalkableEntities.Classes.AddObject(c);
                
                var terms = stiCourse.SectionTerms.Select(x => x.TermID).Distinct().ToList();
                var mps = new List<int>();
                foreach (var t in terms)
                {
                    var mp = markingPeriods.First(x => x.SisId == t);
                    mps.Add(mp.Id);
                    var mpc = new MarkingPeriodClass
                    {
                        Class = c,
                        MarkingPeriod = mp
                    };
                    ChalkableEntities.MarkingPeriodClasses.AddObject(mpc);
                }

                var scheduledSections = stiCourse.ScheduledSections.ToList();
                foreach (var scheduledSection in scheduledSections)
                {
                    var generalPeriods = ChalkableEntities.GeneralPeriods.Where(x => x.SisId2 == scheduledSection.TimeSlotID
                        && x.ScheduleSection.SisId == scheduledSection.DayTypeID && mps.Contains(x.MarkingPeriodRef)).ToList();
                    foreach (var generalPeriod in generalPeriods)
                    {
                        RoomInfo room;
                        if (stiCourse.RoomID.HasValue)
                            room = ChalkableEntities.RoomInfoes.First(x => x.SisId == stiCourse.RoomID.Value && x.SchoolInfoRef == SchoolId);
                        else
                            room = noRoom;
                        var classGeneralPeriod = new ClassGeneralPeriod
                        {
                            Class = c,
                            GeneralPeriod = generalPeriod,
                            RoomInfo = room
                        };
                        ChalkableEntities.ClassGeneralPeriods.AddObject(classGeneralPeriod);    
                    }
                    
                }
                counter++;
                if (counter % 100 == 0)
                {
                    Log.LogWarning(string.Format(ChlkResources.CLASS_RECORDS_PROCESSED, counter));
                    ChalkableEntities.SaveChanges();
                }
            }
            ChalkableEntities.SaveChanges();*/
        }


        private const string userEmailFmt = "user{0}_{1}@chalkable.com";
        private const string defUserPass = "tester";
        private const string descrWork = "Work";
        private const string descrCell = "cell";
        private const string img = "image";

        private void ImportSchoolPersons()
        {
            /*var membershipService = new AccountMembershipService();
            int counter = 0;
            
            var now = DateTime.UtcNow;
            var persons =
                stiEntities.People.Where(x => 
                    stiEntities.StaffSchools.Any(y=>y.SchoolID == SisSchoolId && y.StaffID == x.PersonID)
                    ||stiEntities.StudentSchools.Any(y=>y.SchoolID == SisSchoolId && y.StudentID == x.PersonID)
                    );

            foreach (var person in persons)
            {
                counter++;
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.USERS_PROCESSED, counter));
                string email = string.Format(userEmailFmt, person.PersonID, SchoolId);
                var status = membershipService.CreateUser(email, defUserPass, email);//TODO: generate password?

                int? roleId;
                if (person.Student != null)
                    roleId = CoreRoles.STUDENT_ROLE.Id;
                else if (person.Staff != null)
                {
                    roleId = CoreRoles.TEACHER_ROLE.Id;
                }
                else
                {
                    roleId = CoreRoles.ADMIN_VIEW_ROLE.Id;
                }
                if (status == MembershipCreateStatus.Success)
                {
                    var usr = Membership.GetUser(email);
                    if (usr == null)
                    {
                        Log.LogWarning(string.Format(ChlkResources.ERR_PEOPLE_IMPORT_USER_WASNT_ADDED_TO_MEMBERSHIP, email));
                        continue;
                    }
                    var uId = usr.ProviderUserKey;
                    if (uId == null)
                    {
                        Log.LogWarning(string.Format(ChlkResources.ERR_PEOPLE_IMPORT_USER_DOESNT_HAVE_PROVIDER_KEY, email));
                        continue;
                    }
                    var userId = new Guid(uId.ToString());
                    var pi = new PersonInfo
                    {
                        AspnetUserId = userId,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        LocalId = person.PersonID.ToString(),
                        Gender = person.Gender.Code,
                        BirthDate = person.DateOfBirth,
                        Ethinicity = "",
                        SisId = person.PersonID
                    };
                    ChalkableEntities.PersonInfoes.AddObject(pi);

                    var sp = new SchoolPerson
                    {
                        PersonInfo = pi,
                        SchoolInfoRef = SchoolId,
                        RoleRef = roleId.Value,
                        SisId = person.PersonID
                    };
                    ChalkableEntities.SchoolPersons.AddObject(sp);
                    if (person.Student != null)
                    {
                        var gl = person.Student.StudentGradeLevels.OrderByDescending(x => x.GradeLevelID).FirstOrDefault(x => x.EndTime == null || x.EndTime < now);
                        if (gl == null)
                        {
                            Log.LogWarning(string.Format(ChlkResources.ERR_STI_NO_GRADE_LEVEL_FOR_STUDENT, person.PersonID));
                        }
                        var si = new StudentInfo
                        {
                            EnrollmentDate = gl.StartDate,
                            GradeLevelRef = gl.GradeLevelID - 5,
                            SchoolPerson = sp
                        };
                        ChalkableEntities.StudentInfoes.AddObject(si);
                    }
                    foreach (var personTelephone in person.PersonTelephones)
                    {
                        var type = PhoneType.Home;
                        if (personTelephone.Description == descrWork)
                            type = PhoneType.Work;
                        if (personTelephone.Description == descrCell)
                            type = PhoneType.Mobile;
                        TryAddPhone(ChalkableEntities, sp, personTelephone.FormattedTelephoneNumber, type, personTelephone.IsPrimary);
                    }

                    var addr = person.Address ?? person.Address1;
                    if (addr != null)
                    {
                        var address = new Address
                        {
                            Value = addr.AddressLine1 + "," + addr.City + "," + addr.PostalCode + "," + addr.State,
                            SisId = person.PhysicalAddressID,
                            SchoolPerson = sp
                        };
                        ChalkableEntities.Addresses.AddObject(address);
                    }
                    else
                        Log.LogWarning(string.Format(ChlkResources.ERR_STI_NO_ADDRESS_FOR_PERSON, person.PersonID));

                    if (person.Document != null && person.Document.MIMEType.ToLower().StartsWith(img))//TODO: other picture formats
                    {
                        var p = new Picture()
                            {
                                Content = person.Document.Data
                            };    
                        var pp = new PersonPicture()
                            {
                                Picture = p,
                                PersonInfo = sp.PersonInfo
                            };
                        ChalkableEntities.Pictures.AddObject(p);
                        ChalkableEntities.PersonPictures.AddObject(pp);
                    }
                    

                }
                else
                {
                    Log.LogWarning(string.Format(ChlkResources.ERR_PEOPLE_IMPORT_USER_WASNT_ADDED, email, status));
                }
                ChalkableEntities.SaveChanges();
            }*/
        }

        /*protected void TryAddPhone(ChalkableEntities entities, SchoolPerson schoolPerson, string phoneNum, PhoneType type, bool isPrimary)
        {
            if (!string.IsNullOrEmpty(phoneNum))
            {
                var digitsOnly = PhoneService.DigitsOnly(phoneNum);
                var phone = new Phone
                {
                    SchoolPerson = schoolPerson,
                    TypeTyped = type,
                    Value = phoneNum,
                    IsPrimary = isPrimary,
                    DigitOnlyValue = digitsOnly
                };
                entities.Phones.AddObject(phone);
            }
        }*/

        private void ImportSchoolYears(IList<int> sessionIds)
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
                var schoolYear = new SchoolYear
                {
                    Code = calendar.AcadYear.ToString(CultureInfo.InvariantCulture),
                    Description = calendar.Name,
                    StartDate = calendar.StartDate.Value,
                    EndDate = calendar.EndDate.Value,
                    SchoolInfoRef = SchoolId,
                    SisId = calendar.AcadSessionID
                };
                ChalkableEntities.SchoolYears.AddObject(schoolYear);
                ChalkableEntities.SaveChanges();
                Log.LogInfo(string.Format(ChlkResources.MARKING_PERIODS_FOR_SCHOOL_YEAR_IMPORT_START, schoolYear.Id));
                ImportMarkingPeriods(schoolYear.Id, sessionId);
                ImportDays(schoolYear.Id, sessionId);
            }*/

        }

        private void ImportDays(int schoolYearId, int sessionId)
        {
            /*var days = stiEntities.CalendarDays.Where(x => x.AcadSessionID == sessionId).ToList();
            var markingPeriods = ChalkableEntities.MarkingPeriods.Where(x => x.SchoolYearRef == schoolYearId).ToList();
            var scheduleSections = ChalkableEntities.ScheduleSections.Where(x => x.MarkingPeriod.SchoolYearRef == schoolYearId).ToList();
            var added = new HashSet<DateTime>();
            foreach (var day in days)
            {
                if (added.Contains(day.Date))
                    continue;
                added.Add(day.Date);
                int? mpId = null;
                int? ssId = null;
                var mp = markingPeriods.FirstOrDefault(x => x.StartDate <= day.Date && x.EndDate >= day.Date);
                if (mp != null)
                {
                    mpId = mp.Id;
                    var ss = scheduleSections.FirstOrDefault(x => x.SisId == day.DayTypeID && x.MarkingPeriodRef == mpId);
                    if (ss != null)
                        ssId = ss.Id;
                }
                var date = new Date
                {
                    Date1 = day.Date,
                    IsSchoolDay = day.InSchool,
                    MarkingPeriodRef = mpId,
                    ScheduleSectionRef = ssId,
                    SchoolInfoRef = SchoolId
                };
                ChalkableEntities.Dates.AddObject(date);
            }
            ChalkableEntities.SaveChanges();
            Log.LogInfo(string.Format(ChlkResources.DAYS_FOR_SCHOOL_YEAR_IMPORTED, schoolYearId));*/
        }

        private void ImportMarkingPeriods(int schoolYearId, int sessionId)
        {
            /*var terms = stiEntities.Terms.Where(x => x.AcadSessionID == sessionId).ToList();
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
                MarkingPeriod markingPeriod = new MarkingPeriod
                {
                    Description = term.Description,
                    StartDate = term.StartDate.Value,
                    EndDate = term.EndDate.Value,
                    Name = term.Name,
                    SisId = term.TermID,
                    SchoolYearRef = schoolYearId,
                    WeekDays = 62//TODO: think about where to get this
                };
                ChalkableEntities.MarkingPeriods.AddObject(markingPeriod);
                ChalkableEntities.SaveChanges();
                Log.LogInfo(string.Format(ChlkResources.IMPORT_SCHEDULE_SECTION_START, markingPeriod.Id));
                ImportScheduleSections(markingPeriod.Id, sessionId);
            }*/
        }

        private const string unknownRoomNumber = "Unknown number";
        private const string defaultRoomType = "Default room type";

        private void ImportRooms(int icSchoolId)
        {
            /*var rt = new RoomType
            {
                Description = defaultRoomType,
                SchoolInfoRef = SchoolId
            };
            ChalkableEntities.RoomTypes.AddObject(rt);
            ChalkableEntities.SaveChanges();

            var rooms = stiEntities.Rooms.Where(x => x.SchoolID == icSchoolId).ToList();
            foreach (var room in rooms)
            {
                var roomInfo = new RoomInfo
                {
                    Description = room.Description,
                    RoomNumber = room.RoomNumber ?? unknownRoomNumber,
                    PhoneNumber = "",
                    Capacity = room.StudentCapacity,
                    SisId = room.RoomID,
                    RoomTypeRef = rt.Id,
                    SchoolInfoRef = SchoolId
                };
                ChalkableEntities.RoomInfoes.AddObject(roomInfo);
            }

            var defaultRoom = new RoomInfo
            {
                Description = NO_ROOM_NUMBER,
                RoomNumber = NO_ROOM_NUMBER,
                RoomTypeRef = rt.Id,
                SchoolInfoRef = SchoolId
            };
            ChalkableEntities.RoomInfoes.AddObject(defaultRoom);
            ChalkableEntities.SaveChanges();

            ChalkableEntities.SaveChanges();*/
        }

        private void ImportScheduleSections(int markingPeriodId, int sessionId)
        {
           /* var dayTypes = stiEntities.DayTypes.Where(x => x.AcadSessionID == sessionId).ToList();
            foreach (var dayType in dayTypes)
            {
                ScheduleSection scheduleSection = new ScheduleSection
                {
                    MarkingPeriodRef = markingPeriodId,
                    Name = dayType.Name,
                    Number = dayType.Sequence,
                    SisId = dayType.DayTypeID
                };
                ChalkableEntities.ScheduleSections.AddObject(scheduleSection);
                ChalkableEntities.SaveChanges();
                Log.LogInfo(string.Format(ChlkResources.IMPORT_GENERAL_PERIODS_FOR_SCHEDULE_SECTION_START, scheduleSection.Id));
                ImportGeneralperiods(scheduleSection.Id, dayType.DayTypeID, dayType.AcadSessionID);
            }*/
        }

        private void ImportGeneralperiods(int scheduleSectionId, int dayTypeId, int sessionId)
        {
            //TODO: this is heuristic
            /*var bs = stiEntities.BellSchedules.Where(x => x.AcadSessionID == sessionId).OrderByDescending(x => x.CalendarDays.Count).FirstOrDefault();
            if (bs == null)
            {
                Log.LogWarning(string.Format(ChlkResources.ERR_STI_NO_BELL_SCHEDULE, sessionId));
                return;
            }
            var scheduledTimeSlots = stiEntities.ScheduledTimeSlots.Where(x => x.BellScheduleID == bs.BellScheduleID).ToList();
            var scheduleSection = ChalkableEntities.ScheduleSections.First(x => x.Id == scheduleSectionId);
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
                var generalPeriod = new GeneralPeriod
                {
                    SectionRef = scheduleSectionId,
                    StartTime = scheduledTimeSlot.StartTime.Value,
                    EndTime = scheduledTimeSlot.EndTime.Value,
                    MarkingPeriodRef = scheduleSection.MarkingPeriodRef,
                    Order = scheduledTimeSlot.TimeSlot.Sequence,
                    SisId = sisId,
                    SisId2 = sisId2
                };
                ChalkableEntities.GeneralPeriods.AddObject(generalPeriod);
            }
            ChalkableEntities.SaveChanges();*/
        }
    }
}
