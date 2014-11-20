using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AlphaGrade = Chalkable.StiConnector.SyncModel.AlphaGrade;
using AlternateScore = Chalkable.StiConnector.SyncModel.AlternateScore;
using BellSchedule = Chalkable.StiConnector.SyncModel.BellSchedule;
using ClassroomOption = Chalkable.StiConnector.SyncModel.ClassroomOption;
using DayType = Chalkable.StiConnector.SyncModel.DayType;
using Person = Chalkable.StiConnector.SyncModel.Person;
using School = Chalkable.StiConnector.SyncModel.School;
using SchoolOption = Chalkable.StiConnector.SyncModel.SchoolOption;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StandardSubject = Chalkable.StiConnector.SyncModel.StandardSubject;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;
using GradingComment = Chalkable.StiConnector.SyncModel.GradingComment;
using GradingPeriod = Chalkable.StiConnector.SyncModel.GradingPeriod;
using GradingScale = Chalkable.StiConnector.SyncModel.GradingScale;
using GradingScaleRange = Chalkable.StiConnector.SyncModel.GradingScaleRange;
using Infraction = Chalkable.StiConnector.SyncModel.Infraction;
using Room = Chalkable.StiConnector.SyncModel.Room;
using ScheduledTimeSlot = Chalkable.Data.School.Model.ScheduledTimeSlot;
using Staff = Chalkable.StiConnector.SyncModel.Staff;
using StaffSchool = Chalkable.StiConnector.SyncModel.StaffSchool;
using Student = Chalkable.StiConnector.SyncModel.Student;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;
using User = Chalkable.StiConnector.SyncModel.User;
using UserSchool = Chalkable.StiConnector.SyncModel.UserSchool;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private void ProcessInsert()
        {
            Log.LogInfo("insert schools");
            InsertSchools();
            Log.LogInfo("insert addresses");
            InsertAddresses();
            Log.LogInfo("insert users");
            InsertUsers();
            Log.LogInfo("insert school users");
            InsertSchoolUsers();
            Log.LogInfo("insert persons");
            InsertPersons();
            Log.LogInfo("insert Staff");
            InsertStaff();
            Log.LogInfo("insert Student");
            InsertStudent();
            Log.LogInfo("insert StudentSchool");
            InsertStudentSchool();
            Log.LogInfo("insert StaffSchool");
            InsertStaffSchool();
            Log.LogInfo("insert persons emails");
            InsertPersonsEmails();
            Log.LogInfo("insert phones");
            InsertPhones();
            Log.LogInfo("insert grade levels");
            InsertGradeLevels();
            Log.LogInfo("insert school years");
            InsertSchoolYears();
            Log.LogInfo("insert student school years");
            InsertStudentSchoolYears();
            Log.LogInfo("insert marking periods");
            InsertMarkingPeriods();
            Log.LogInfo("insert grading periods");
            InsertGradingPeriods();
            Log.LogInfo("insert day types");
            InsertDayTypes();
            Log.LogInfo("insert Bell Schedules");
            InsertBellSchedules();
            Log.LogInfo("insert days");
            InsertDays();
            Log.LogInfo("insert rooms");
            InsertRooms();
            Log.LogInfo("insert grading scales");
            InsertGradingScales();
            Log.LogInfo("insert courses");
            InsertCourses();
            Log.LogInfo("insert class teachers");
            InsertClassTeachers();
            Log.LogInfo("insert standard subjects");
            InsertStandardSubject();
            Log.LogInfo("insert standards");
            InsertStandards();
            Log.LogInfo("insert class standards");
            InsertClassStandard();
            Log.LogInfo("insert marking period classes");
            InsertMarkingPeriodClasses();
            Log.LogInfo("insert periods");
            InsertPeriods();
            Log.LogInfo("insert scheduled time slots");
            InsertScheduledTimeSlots();
            Log.LogInfo("insert class periods");
            InsertClassPeriods();
            Log.LogInfo("insert class persons");
            InsertClassPersons();
            Log.LogInfo("insert attendance reasons");
            InsertAttendanceReasons();
            Log.LogInfo("insert attendance level reasons");
            InsertAttendanceLevelReasons();
            Log.LogInfo("insert alpha grades");
            InsertAlphaGrades();
            Log.LogInfo("insert alternate scores");
            InsertAlternateScores();
            Log.LogInfo("insert infractions");
            InsertInfractions();
            Log.LogInfo("insert scale ranges");
            InsertGradingScaleRanges();
            Log.LogInfo("insert classroom options");
            InsertClassroomOptions();
            Log.LogInfo("insert grading comments");
            InsertGradingComments();
            Log.LogInfo("insert schoolsOptions");
            InsertSchoolsOptions();
        }

        private void InsertPersonsEmails()
        {
            var personsEmails = context.GetSyncResult<StiConnector.SyncModel.PersonEmail>().All;
            var chlkPersonsEmails = personsEmails.Select(x => new Data.School.Model.PersonEmail
                {
                    PersonRef = x.PersonID,
                    Description = x.Description,
                    EmailAddress = x.EmailAddress,
                    IsListed = x.IsListed,
                    IsPrimary = x.IsPrimary
                }).ToList();
            ServiceLocatorSchool.PersonEmailService.AddPersonsEmails(chlkPersonsEmails);
        }

        private void InsertSchools()
        {
            var schools = context.GetSyncResult<School>().All;
            foreach (var school in schools)
            {
                ServiceLocatorSchool.SchoolService.Add(new Data.School.Model.School
                {
                    Id = school.SchoolID,
                    IsActive = school.IsActive,
                    IsPrivate = school.IsPrivate,
                    Name = school.Name,
                    IsChalkableEnabled = school.IsChalkableEnabled
                });
                importedSchoolIds.Add(school.SchoolID);
            }
        }

        private void InsertSchoolsOptions()
        {
            var schoolOptions = context.GetSyncResult<SchoolOption>().All;
            var res = schoolOptions.Select(schoolOption => new Data.School.Model.SchoolOption
                {
                    Id = schoolOption.SchoolID, 
                    AllowDualEnrollment = schoolOption.AllowDualEnrollment, 
                    AllowScoreEntryForUnexcused = schoolOption.AllowScoreEntryForUnexcused, 
                    AllowSectionAverageModification = schoolOption.AllowSectionAverageModification, 
                    AveragingMethod = schoolOption.AveragingMethod, 
                    BaseHoursOffset = schoolOption.BaseHoursOffset, 
                    BaseMinutesOffset = schoolOption.BaseMinutesOffset, 
                    CategoryAveraging = schoolOption.CategoryAveraging, 
                    CompleteStudentScheduleDefinition = schoolOption.CompleteStudentScheduleDefinition, 
                    DefaultCombinationIndex = schoolOption.DefaultCombinationIndex, 
                    DisciplineOverwritesAttendance = schoolOption.DisciplineOverwritesAttendance, 
                    EarliestPaymentDate = schoolOption.EarliestPaymentDate, 
                    IncludeReportCardCommentsInGradebook = schoolOption.IncludeReportCardCommentsInGradebook, 
                    LockCategories = schoolOption.LockCategories, 
                    MergeRostersForAttendance = schoolOption.MergeRostersForAttendance, 
                    NextReceiptNumber = schoolOption.NextReceiptNumber, 
                    ObservesDst = schoolOption.ObservesDst, 
                    StandardsCalculationMethod = schoolOption.StandardsCalculationMethod, 
                    StandardsCalculationRule = schoolOption.StandardsCalculationRule, 
                    StandardsCalculationWeightMaximumValues = schoolOption.StandardsCalculationWeightMaximumValues, 
                    StandardsGradingScaleRef = schoolOption.StandardsGradingScaleID, 
                    TimeZoneName = schoolOption.TimeZoneName
                }).ToList();
            ServiceLocatorSchool.SchoolService.AddSchoolOptions(res);
        }



        private void InsertAddresses()
        {
            var adds = context.GetSyncResult<Address>().All;
            var addressInfos = adds.Select(x => new Data.School.Model.Address
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

        private void InsertUsers()
        {
            var users = context.GetSyncResult<User>().All.Select(x=>new Data.Master.Model.User
                {
                    Id = Guid.NewGuid(),
                    DistrictRef = ServiceLocatorSchool.Context.DistrictId,
                    FullName = x.FullName,
                    IsDemoUser = false,
                    SisUserId = x.UserID,
                    SisUserName = x.UserName,
                    Password = UserService.PasswordMd5(DEF_USER_PASS),
                    Login = string.Format(USER_EMAIL_FMT, x.UserID, ServiceLocatorSchool.Context.DistrictId),
                    IsSysAdmin = false
                }).ToList();
            ServiceLocatorMaster.UserService.Add(users);
        }
        
        private void InsertSchoolUsers()
        {
            var masterUserSchool = context.GetSyncResult<UserSchool>().All.Select(x => new SchoolUser
                {
                    DistrictRef = ServiceLocatorSchool.Context.DistrictId.Value,
                    SchoolRef = x.SchoolID,
                    UserRef = x.UserID
                }).ToList();
            ServiceLocatorMaster.UserService.AddSchoolUsers(masterUserSchool);
            var districtUserSchool = context.GetSyncResult<UserSchool>().All.Select(x => new Data.School.Model.UserSchool
            {
                SchoolRef = x.SchoolID,
                UserRef = x.UserID
            }).ToList();
            ServiceLocatorSchool.UserSchoolService.Add(districtUserSchool);
        }

        private void InsertPersons()
        {
            var genders = context.GetSyncResult<Gender>().All.ToDictionary(x => x.GenderID);
            var persons = context.GetSyncResult<Person>().All
                .Select(x=>new Data.School.Model.Person
                    {
                        Active = true,
                        AddressRef = x.PhysicalAddressID,
                        BirthDate = x.DateOfBirth,
                        FirstName = x.FirstName,
                        Gender = x.GenderID.HasValue ? genders[x.GenderID.Value].Code : "U",
                        Id = x.PersonID,
                        LastName = x.LastName,
                        PhotoModifiedDate = x.PhotoModifiedDate
                    }).ToList();
            ServiceLocatorSchool.PersonService.Add(persons);
            foreach (var person in context.GetSyncResult<Person>().All)
                if (person.PhotoModifiedDate.HasValue)
                    personsForImportPictures.Add(person);
        }

        private void InsertStaff()
        {
            var genders = context.GetSyncResult<Gender>().All.ToDictionary(x => x.GenderID);
            var staff = context.GetSyncResult<Staff>().All.Select(x => new Data.School.Model.Staff
                {
                    BirthDate = x.DateOfBirth,
                    Id = x.StaffID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Gender = x.GenderID.HasValue ? genders[x.GenderID.Value].Code : "U",
                    UserId = x.UserID
                }).ToList();
            ServiceLocatorSchool.StaffService.Add(staff);
        }

        private void InsertStudent()
        {
            var genders = context.GetSyncResult<Gender>().All.ToDictionary(x => x.GenderID);
            var statuses = context.GetSyncResult<SpEdStatus>().All.ToDictionary(x => x.SpEdStatusID);
            var students = context.GetSyncResult<Student>().All.Select(x => new Data.School.Model.Student
            {
                BirthDate = x.DateOfBirth,
                Id = x.StudentID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Gender = x.GenderID.HasValue ? genders[x.GenderID.Value].Code : "U",
                UserId = x.UserID,
                HasMedicalAlert = x.HasMedicalAlert,
                IsAllowedInetAccess = x.IsAllowedInetAccess,
                SpecialInstructions = x.SpecialInstructions,
                SpEdStatus = x.SpEdStatusID.HasValue ? statuses[x.SpEdStatusID.Value].Name : ""
            }).ToList();
            ServiceLocatorSchool.StudentService.AddStudents(students);
        }

        private void InsertStaffSchool()
        {
            var staffSchool = context.GetSyncResult<StaffSchool>().All.Select(x => new Data.School.Model.StaffSchool
                {
                    SchoolRef = x.SchoolID,
                    StaffRef = x.StaffID
                }).ToList();
            ServiceLocatorSchool.StaffService.AddStaffSchools(staffSchool);
        }

        private void InsertStudentSchool()
        {
            var studentSchools = context.GetSyncResult<StudentSchool>().All.Select(x => new Data.School.Model.StudentSchool
            {
                SchoolRef = x.SchoolID,
                StudentRef = x.StudentID
            }).ToList();
            ServiceLocatorSchool.StudentService.AddStudentSchools(studentSchools);
        }

        private void InsertPhones()
        {
            var phones = context.GetSyncResult<PersonTelephone>().All;
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

        private void InsertGradeLevels()
        {
            var sisGradeLevels = context.GetSyncResult<GradeLevel>().All;
            foreach (var gradeLevel in sisGradeLevels)
            {
                ServiceLocatorSchool.GradeLevelService.AddGradeLevel(gradeLevel.GradeLevelID, gradeLevel.Name, gradeLevel.Sequence);
            }
        }

        private void InsertSchoolYears()
        {
            var sessions = context.GetSyncResult<AcadSession>().All;
            foreach (var session in sessions)
            {
                ServiceLocatorSchool.SchoolYearService.Add(session.AcadSessionID, session.SchoolID, session.Name,
                                                           session.Description, session.StartDate, session.EndDate);
            }
        }

        private void InsertStudentSchoolYears()
        {
            var assignments = context.GetSyncResult<StudentAcadSession>().All
                .Where(x => x.GradeLevelID.HasValue)
                .ToList()
                .Select(x => new StudentSchoolYear
                {
                    GradeLevelRef = x.GradeLevelID.Value,
                    SchoolYearRef = x.AcadSessionID,
                    StudentRef = x.StudentID,
                    EnrollmentStatus = x.CurrentEnrollmentStatus == "C" ? StudentEnrollmentStatusEnum.CurrentlyEnrolled : StudentEnrollmentStatusEnum.PreviouslyEnrolled
                }).ToList();
            ServiceLocatorSchool.SchoolYearService.AssignStudent(assignments);
        }

        private void InsertMarkingPeriods()
        {
            var terms = context.GetSyncResult<Term>().All;
            var sys = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToDictionary(x => x.Id);
            var mps = new List<MarkingPeriod>();
            foreach (var term in terms)
            {
                mps.Add(new MarkingPeriod
                    {
                        Description = term.Description,
                        EndDate = term.EndDate,
                        Id = term.TermID,
                        Name = term.Name,
                        SchoolRef = sys[term.AcadSessionID].SchoolRef,
                        SchoolYearRef = term.AcadSessionID,
                        StartDate = term.StartDate,
                        WeekDays = 62
                    });
            }
            ServiceLocatorSchool.MarkingPeriodService.Add(mps);
        }

        private void InsertGradingPeriods()
        {
            var gPeriods = context.GetSyncResult<GradingPeriod>().All
                .Select(x=>new Data.School.Model.GradingPeriod
                    {
                        Id = x.GradingPeriodID,
                        AllowGradePosting = x.AllowGradePosting,
                        Code = x.Code,
                        Description = x.Description,
                        EndDate = x.EndDate,
                        EndTime = x.EndTime,
                        MarkingPeriodRef = x.TermID,
                        StartDate = x.StartDate,
                        Name = x.Name,
                        SchoolAnnouncement = x.SchoolAnnouncement,
                        SchoolYearRef = x.AcadSessionID
                    }).ToList();
            ServiceLocatorSchool.GradingPeriodService.Add(gPeriods);
        }

        private void InsertDayTypes()
        {
            var dayTypes = context.GetSyncResult<DayType>().All
                .Select(x=>new Data.School.Model.DayType
                {
                    Id = x.DayTypeID,
                    Name = x.Name,
                    Number = x.Sequence,
                    SchoolYearRef = x.AcadSessionID
                }).ToList();
            ServiceLocatorSchool.DayTypeService.Add(dayTypes);
        }

        private void InsertDays()
        {
            var days = context.GetSyncResult<CalendarDay>().All
                .ToList()
                .Select(x => new Date
                {
                    DayTypeRef = x.DayTypeID,
                    IsSchoolDay = x.InSchool,
                    SchoolYearRef = x.AcadSessionID,
                    BellScheduleRef = x.BellScheduleID,
                    Day = x.Date
                }).ToList();
            ServiceLocatorSchool.CalendarDateService.Add(days);
        }

        private void InsertRooms()
        {
            var rooms = context.GetSyncResult<Room>().All;
            foreach (var room in rooms)
            {
                ServiceLocatorSchool.RoomService.AddRoom(room.RoomID, room.SchoolID, room.RoomNumber ?? UNKNOWN_ROOM_NUMBER, room.Description, null, room.StudentCapacity ?? 0, null);
            }
        }

        private List<Pair<string, Guid>> PrepareChalkableDepartmentKeywords()
        {
            var departments = ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
            var sep = new[] { ',' };
            var departmenPairs = new List<Pair<string, Guid>>();

            foreach (var chalkableDepartment in departments)
            {
                departmenPairs.AddRange(chalkableDepartment.Keywords.ToLower().Split(sep, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Pair<string, Guid>(x, chalkableDepartment.Id)));
            }
            return departmenPairs;
        }

        private Pair<string, Guid> FindClosestDepartment(List<Pair<string, Guid>> departmenPairs, string name)
        {
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
            return closestDep;
        }

        private void InsertCourses()
        {
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToDictionary(x => x.Id);
            var departmenPairs = PrepareChalkableDepartmentKeywords();

            var courses = context.GetSyncResult<Course>().All;
            var classes = new List<Class>();
            foreach (var course in courses)
            {
                var glId = course.MaxGradeLevelID ?? course.MinGradeLevelID;
                if (!glId.HasValue)
                {
                    Log.LogWarning(string.Format("No grade level for class {0}", course.CourseID));
                    continue;
                }
                var closestDep = FindClosestDepartment(departmenPairs, course.ShortName.ToLower());
                classes.Add(new Class
                {
                    ChalkableDepartmentRef = closestDep != null ? closestDep.Second : (Guid?)null,
                    Description = course.FullName,
                    GradeLevelRef = glId.Value,
                    Id = course.CourseID,
                    ClassNumber = course.FullSectionNumber,
                    Name = course.ShortName,
                    SchoolRef = course.AcadSessionID != null ? years[course.AcadSessionID.Value].SchoolRef : (int?)null,
                    SchoolYearRef = course.AcadSessionID,
                    PrimaryTeacherRef = course.PrimaryTeacherID,
                    RoomRef = course.RoomID,
                    CourseRef = course.SectionOfCourseID,
                    GradingScaleRef = course.GradingScaleID
                });
            }

            ServiceLocatorSchool.ClassService.Add(classes);
        }

        private void InsertClassTeachers()
        {
            var teachers = context.GetSyncResult<SectionStaff>().All.Select(x => new ClassTeacher
            {
                ClassRef = x.SectionID,
                IsCertified = x.IsCertified,
                IsHighlyQualified = x.IsHighlyQualified,
                IsPrimary = x.IsPrimary,
                PersonRef = x.StaffID
            }).ToList();
            ServiceLocatorSchool.ClassService.AddTeachers(teachers);
        }

        private void InsertStandardSubject()
        {
            var ss = context.GetSyncResult<StandardSubject>().All.Select(x => new Data.School.Model.StandardSubject
            {
                AdoptionYear = x.AdoptionYear,
                Id = x.StandardSubjectID,
                Description = x.Description,
                IsActive = x.IsActive,
                Name = x.Name
            }).ToList();
            ServiceLocatorSchool.StandardService.AddStandardSubjects(ss);
        }

        private void InsertStandards()
        {
            var sts = context.GetSyncResult<Standard>().All.Select(x => new Data.School.Model.Standard
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
            var toInsert = sts.ToDictionary(x=>x.Id);
            var sorted = TopologicSort(x => x.Id, x => x.ParentStandardRef, toInsert);
            ServiceLocatorSchool.StandardService.AddStandards(sorted);
        }

        private void InsertClassStandard()
        {
            var cs = context.GetSyncResult<CourseStandard>().All
                .Select(x => new ClassStandard
                {
                    ClassRef = x.CourseID,
                    StandardRef = x.StandardID
                }).ToList();
            ServiceLocatorSchool.StandardService.AddClassStandards(cs);
        }

        private void InsertMarkingPeriodClasses()
        {
            var mps = ServiceLocatorSchool.MarkingPeriodService.GetMarkingPeriods(null);
            var cts = context.GetSyncResult<SectionTerm>().All
                .Select(x => new MarkingPeriodClass
                {
                    ClassRef = x.SectionID,
                    MarkingPeriodRef = x.TermID,
                    SchoolRef = mps.First(y=>y.Id == x.TermID).SchoolRef
                }).ToList();
            ServiceLocatorSchool.ClassService.AssignClassToMarkingPeriod(cts);
        }

        private void InsertPeriods()
        {
            var periods = context.GetSyncResult<TimeSlot>().All
                .Select(x => new Period
                {
                    Id = x.TimeSlotID,
                    Order = x.Sequence,
                    SchoolYearRef = x.AcadSessionID
                })
                .ToList();
            ServiceLocatorSchool.PeriodService.AddPeriods(periods);
        }

        private void InsertBellSchedules()
        {
            var bellSchedules = context.GetSyncResult<BellSchedule>().All.Select(x => new Data.School.Model.BellSchedule
            {
                Id = x.BellScheduleID,
                Code = x.Code,
                Description = x.Description,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                TotalMinutes = x.TotalMinutes,
                UseStartEndTime = x.UseStartEndTime
            }).ToList();
            ServiceLocatorSchool.BellScheduleService.Add(bellSchedules);
        }
        
        private void InsertScheduledTimeSlots()
        {
            var allSts = context.GetSyncResult<StiConnector.SyncModel.ScheduledTimeSlot>().All
                .Select(x=>new ScheduledTimeSlot
                    {
                        BellScheduleRef = x.BellScheduleID,
                        Description = x.Description,
                        EndTime = x.EndTime,
                        IsDailyAttendancePeriod = x.IsDailyAttendancePeriod,
                        StartTime = x.StartTime,
                        PeriodRef = x.TimeSlotID
                    })
                .ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.Add(allSts);
        }

        private void InsertClassPeriods()
        {
            var scheduledSections = context.GetSyncResult<ScheduledSection>().All;
            var classPeriods = scheduledSections
                .Select(x => new ClassPeriod
                {
                    ClassRef = x.SectionID,
                    DayTypeRef = x.DayTypeID,
                    PeriodRef = x.TimeSlotID,
                }).ToList();
            ServiceLocatorSchool.ClassPeriodService.Add(classPeriods);
        }

        private void InsertClassPersons()
        {
            var mps = ServiceLocatorSchool.MarkingPeriodService.GetMarkingPeriods(null);
            var studentSchedules = context.GetSyncResult<StudentScheduleTerm>().All
                .Select(x => new ClassPerson
                {
                    ClassRef = x.SectionID,
                    PersonRef = x.StudentID,
                    MarkingPeriodRef = x.TermID,
                    SchoolRef = mps.First(y => y.Id == x.TermID).SchoolRef,
                    IsEnrolled = x.IsEnrolled
                }).ToList();
            ServiceLocatorSchool.ClassService.AddStudents(studentSchedules);
        }

        private void InsertAttendanceReasons()
        {
            var reasons = context.GetSyncResult<AbsenceReason>().All;
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
        }

        private void InsertAttendanceLevelReasons()
        {
            var absenceLevelReasons = context.GetSyncResult<AbsenceLevelReason>().All
                                                 .Select(x => new AttendanceLevelReason
                                                 {
                                                     Id = x.AbsenceLevelReasonID,
                                                     AttendanceReasonRef = x.AbsenceReasonID,
                                                     IsDefault = x.IsDefaultReason,
                                                     Level = x.AbsenceLevel
                                                 }).ToList();
            ServiceLocatorSchool.AttendanceReasonService.AddAttendanceLevelReasons(absenceLevelReasons);
        }

        private void InsertAlphaGrades()
        {
            var alphaGrades = context.GetSyncResult<AlphaGrade>().All.Select(x => new Data.School.Model.AlphaGrade
            {
                Id = x.AlphaGradeID,
                Description = x.Description,
                Name = x.Name,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.AlphaGradeService.AddAlphaGrades(alphaGrades);
        }

        private void InsertAlternateScores()
        {
            var alternateScores = context.GetSyncResult<AlternateScore>().All.Select(x => new Data.School.Model.AlternateScore
            {
                Id = x.AlternateScoreID,
                Description = x.Description,
                IncludeInAverage = x.IncludeInAverage,
                Name = x.Name,
                PercentOfMaximumScore = x.PercentOfMaximumScore
            }).ToList();
            ServiceLocatorSchool.AlternateScoreService.AddAlternateScores(alternateScores);
        }

        private void InsertInfractions()
        {
            var infractions = context.GetSyncResult<Infraction>().All.Select(x => new Data.School.Model.Infraction
                {
                    Code = x.Code,
                    Demerits = x.Demerits,
                    Description = x.Description,
                    Id = x.InfractionID,
                    IsActive = x.IsActive,
                    IsSystem = x.IsSystem,
                    Name = x.Name,
                    NCESCode = x.NCESCode,
                    SIFCode = x.SIFCode,
                    StateCode = x.StateCode
                }).ToList();
            ServiceLocatorSchool.InfractionService.Add(infractions);
        }

        private void InsertGradingScales()
        {
            var gs = context.GetSyncResult<GradingScale>().All.Select(x => new Data.School.Model.GradingScale
                {
                    Id = x.GradingScaleID,
                    Description = x.Description,
                    HomeGradeToDisplay = x.HomeGradeToDisplay,
                    Name = x.Name,
                    SchoolRef = x.SchoolID
                }).ToList();
            ServiceLocatorSchool.GradingScaleService.AddGradingScales(gs);
        }

        private void InsertGradingScaleRanges()
        {
            var gsr = context.GetSyncResult<GradingScaleRange>().All.Select(x => new Data.School.Model.GradingScaleRange
                           {
                               AlphaGradeRef = x.AlphaGradeID,
                               AveragingEquivalent = x.AveragingEquivalent,
                               AwardGradCredit = x.AwardGradCredit,
                               GradingScaleRef = x.GradingScaleID,
                               HighValue = x.HighValue,
                               IsPassing = x.IsPassing,
                               LowValue = x.LowValue
                           })
                       .ToList();
            ServiceLocatorSchool.GradingScaleService.AddGradingScaleRanges(gsr);
        }

        private void InsertClassroomOptions()
        {
            var cro = context.GetSyncResult<ClassroomOption>().All.Select(x => new Data.School.Model.ClassroomOption()
                {
                    Id = x.SectionID,
                    DefaultActivitySortOrder = x.DefaultActivitySortOrder,
                    GroupByCategory = x.GroupByCategory,
                    AveragingMethod = x.AveragingMethod,
                    CategoryAveraging = x.CategoryAveraging,
                    IncludeWithdrawnStudents = x.IncludeWithdrawnStudents,
                    DisplayStudentAverage = x.DisplayStudentAverage,
                    DisplayTotalPoints = x.DisplayTotalPoints,
                    RoundDisplayedAverages = x.RoundDisplayedAverages,
                    DisplayAlphaGrade = x.DisplayAlphaGrade,
                    DisplayStudentNames = x.DisplayStudentNames,
                    DisplayMaximumScore = x.DisplayMaximumScore,
                    StandardsGradingScaleRef = x.StandardsGradingScaleID,
                    StandardsCalculationMethod = x.StandardsCalculationMethod,
                    StandardsCalculationRule = x.StandardsCalculationRule,
                    StandardsCalculationWeightMaximumValues = x.StandardsCalculationWeightMaximumValues,
                    DefaultStudentSortOrder = x.DefaultStudentSortOrder,
                    SeatingChartRows = x.SeatingChartRows,
                    SeatingChartColumns = x.SeatingChartColumns
                }).ToList();
            ServiceLocatorSchool.ClassroomOptionService.Add(cro);
        }

        private void InsertGradingComments()
        {
            var gc = context.GetSyncResult<GradingComment>().All.Select(x => new Data.School.Model.GradingComment
                {
                    Code = x.Code,
                    Comment = x.Comment,
                    Id = x.GradingCommentID,
                    SchoolRef = x.SchoolID
                }).ToList();
            ServiceLocatorSchool.GradingCommentService.Add(gc);
        }
    }
}
