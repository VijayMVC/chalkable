using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AlphaGrade = Chalkable.StiConnector.SyncModel.AlphaGrade;
using AlternateScore = Chalkable.StiConnector.SyncModel.AlternateScore;
using ClassroomOption = Chalkable.StiConnector.SyncModel.ClassroomOption;
using DayType = Chalkable.StiConnector.SyncModel.DayType;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;
using GradingComment = Chalkable.StiConnector.SyncModel.GradingComment;
using GradingPeriod = Chalkable.StiConnector.SyncModel.GradingPeriod;
using GradingScale = Chalkable.StiConnector.SyncModel.GradingScale;
using GradingScaleRange = Chalkable.StiConnector.SyncModel.GradingScaleRange;
using Infraction = Chalkable.StiConnector.SyncModel.Infraction;
using Person = Chalkable.StiConnector.SyncModel.Person;
using Room = Chalkable.StiConnector.SyncModel.Room;
using School = Chalkable.StiConnector.SyncModel.School;
using SchoolOption = Chalkable.StiConnector.SyncModel.SchoolOption;
using Staff = Chalkable.StiConnector.SyncModel.Staff;
using StaffSchool = Chalkable.StiConnector.SyncModel.StaffSchool;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StandardSubject = Chalkable.StiConnector.SyncModel.StandardSubject;
using Student = Chalkable.StiConnector.SyncModel.Student;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private void ProcessUpdate()
        {
            Log.LogInfo("update schools");
            UpdateSchools();
            Log.LogInfo("update adresses");
            UpdateAddresses();
            
            Log.LogInfo("update users");
            UpdateUsers();
            Log.LogInfo("update persons");
            UpdatePersons();
            Log.LogInfo("update Staff");
            UpdateStaff();
            Log.LogInfo("update Student");
            UpdateStudent();
            Log.LogInfo("update StudentSchool");
            UpdateStudentSchool();
            Log.LogInfo("update StaffSchool");
            UpdateStaffSchool();
            
            
            Log.LogInfo("update persons emails");
            UpdatePersonsEmails();
            Log.LogInfo("update phones");
            UpdatePhones();
            Log.LogInfo("update grade levels");
            UpdateGradeLevels();
            Log.LogInfo("update school years");
            UpdateSchoolYears();
            Log.LogInfo("update student school years");
            UpdateStudentSchoolYears();
            Log.LogInfo("update marking periods");
            UpdateMarkingPeriods();
            Log.LogInfo("update grading periods");
            UpdateGradingPeriods();
            Log.LogInfo("update day types");
            UpdateDayTypes();
            Log.LogInfo("update days");
            UpdateDays();
            Log.LogInfo("update rooms");
            UpdateRooms();
            Log.LogInfo("update grading scales");
            UpdateGradingScales();
            Log.LogInfo("update courses");
            UpdateCourses();
            Log.LogInfo("update class teachers");
            UpdateClassTeachers();
            Log.LogInfo("update standard subjects");
            UpdateStandardSubject();
            Log.LogInfo("update standards");
            UpdateStandards();
            Log.LogInfo("update class standards");
            UpdateClassStandard();
            Log.LogInfo("update marking period classes");
            UpdateMarkingPeriodClasses();
            Log.LogInfo("update scheduled time slots");
            UpdateScheduledTimeSlots();
            Log.LogInfo("update periods");
            UpdatePeriods();
            Log.LogInfo("update class periods");
            UpdateClassPeriods();
            Log.LogInfo("update class persons");
            UpdateClassPersons();
            Log.LogInfo("update attendance reasons");
            UpdateAttendanceReasons();
            Log.LogInfo("update attendance level reasons");
            UpdateAttendanceLevelReasons();
            Log.LogInfo("update alpha grades");
            UpdateAlphaGrades();
            Log.LogInfo("update alternate scores");
            UpdateAlternateScores();
            Log.LogInfo("update infractions");
            UpdateInfractions();
            Log.LogInfo("update scale ranges");
            UpdateGradingScaleRanges();
            Log.LogInfo("update classroom options");
            UpdateClassroomOptions();
            Log.LogInfo("update grading comments");
            UpdateGradingComments();
            Log.LogInfo("update schoolsOptions");
            UpdateSchoolsOptions();
        }

        private void UpdateSchools()
        {
            if (context.GetSyncResult<School>().Updated == null)
                return;
            var schools = context.GetSyncResult<School>().Updated.Select(x=>new Data.School.Model.School
                {
                    Id = x.SchoolID,
                    IsActive = x.IsActive,
                    IsPrivate = x.IsPrivate,
                    Name = x.Name,
                    IsChalkableEnabled = x.IsChalkableEnabled
                }).ToList();
            ServiceLocatorSchool.SchoolService.Edit(schools);
        }

        private void UpdateSchoolsOptions()
        {
            if(context.GetSyncResult<SchoolOption>().Updated == null)
                return;
            var res = context.GetSyncResult<SchoolOption>().Updated.Select(schoolOption => new Data.School.Model.SchoolOption
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
            ServiceLocatorSchool.SchoolService.EditSchoolOptions(res);
        }

        private void UpdateAddresses()
        {
            if (context.GetSyncResult<Address>().Updated == null)
                return;
            var addresses = context.GetSyncResult<Address>().Updated.Select(x=>new Data.School.Model.Address
                {
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    AddressNumber = x.AddressNumber,
                    City = x.City,
                    Country = x.Country,
                    CountyId = x.CountryID,
                    Id = x.AddressID,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    PostalCode = x.PostalCode,
                    StreetNumber = x.StreetNumber,
                    State = x.State
                }).ToList();
            ServiceLocatorSchool.AddressService.Edit(addresses);
        }
        
        private void UpdateUsers()
        {
            if (context.GetSyncResult<User>().Updated == null)
                return;
            var users = context.GetSyncResult<User>().All.Select(x => new Data.Master.Model.User
            {
                DistrictRef = ServiceLocatorSchool.Context.DistrictId,
                FullName = x.FullName,
                SisUserId = x.UserID,
                SisUserName = x.UserName
            }).ToList();
            ServiceLocatorMaster.UserService.ImportEdit(users);
        }

        private void UpdatePersons()
        {
            if (context.GetSyncResult<Person>().Updated == null)
                return;
            var genders = context.GetSyncResult<Gender>().All.ToDictionary(x => x.GenderID);
            var persons = context.GetSyncResult<Person>().All
                .Select(x => new Data.School.Model.Person
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
            ServiceLocatorSchool.PersonService.Edit(persons);
            foreach (var person in context.GetSyncResult<Person>().Updated)
                if (person.PhotoModifiedDate.HasValue)
                    personsForImportPictures.Add(person);
        }

        private void UpdateStaff()
        {
            if (context.GetSyncResult<Staff>().Updated == null)
                return;
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
            ServiceLocatorSchool.StaffService.Edit(staff);
        }

        private void UpdateStudent()
        {
            if (context.GetSyncResult<Student>().Updated == null)
                return;
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
            ServiceLocatorSchool.StudentService.EditStudents(students);
        }

        private void UpdateStaffSchool()
        {
            if (context.GetSyncResult<StaffSchool>().Updated == null)
                return;
            var staffSchool = context.GetSyncResult<StaffSchool>().All.Select(x => new Data.School.Model.StaffSchool
            {
                SchoolRef = x.SchoolID,
                StaffRef = x.StaffID
            }).ToList();
            ServiceLocatorSchool.StaffService.EditStaffSchools(staffSchool);
        }

        private void UpdateStudentSchool()
        {
            //TODO: justinsert or delete is possible in INOW
            /*if (context.GetSyncResult<StudentSchool>().Updated == null)
                return;
            var studentSchools = context.GetSyncResult<StudentSchool>().All.Select(x => new Data.School.Model.StudentSchool
            {
                SchoolRef = x.SchoolID,
                StudentRef = x.StudentID
            }).ToList();
            ServiceLocatorSchool.StudentService.EditStudentSchools(studentSchools);*/
        }
        
        private void UpdatePersonsEmails()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.PersonEmail>().Updated == null)
                return;
            var personsEmails = context.GetSyncResult<StiConnector.SyncModel.PersonEmail>().Updated;
            var chlkPersonsEmails = personsEmails.Select(x => new Data.School.Model.PersonEmail
            {
                PersonRef = x.PersonID,
                Description = x.Description,
                EmailAddress = x.EmailAddress,
                IsListed = x.IsListed,
                IsPrimary = x.IsPrimary
            }).ToList();
            ServiceLocatorSchool.PersonEmailService.UpdatePersonsEmails(chlkPersonsEmails);
        }

        private void UpdatePhones()
        {
            if (context.GetSyncResult<PersonTelephone>().Updated == null)
                return;
            var phones = context.GetSyncResult<PersonTelephone>().Updated;
            IList<Phone> ps = new List<Phone>();
            foreach (var pt in phones)
            {
                var type = PhoneType.Home;
                if (pt.Description == DESCR_WORK)
                    type = PhoneType.Work;
                if (pt.Description == DESCR_CELL)
                    type = PhoneType.Mobile;
                if (!string.IsNullOrEmpty(pt.FormattedTelephoneNumber))
                {
                    ps.Add(new Phone
                        {
                            DigitOnlyValue = pt.TelephoneNumber,
                            PersonRef = pt.PersonID,
                            IsPrimary = pt.IsPrimary,
                            Type = type,
                            Value = pt.FormattedTelephoneNumber
                        });
                }
            }
            ServiceLocatorSchool.PhoneService.EditPhones(ps);
        }

        private void UpdateGradeLevels()
        {
            if (context.GetSyncResult<GradeLevel>().Updated == null)
                return;
            var gradeLevels = context.GetSyncResult<GradeLevel>().Updated
                .Select(x=>new Data.School.Model.GradeLevel
                    {
                        Id = x.GradeLevelID,
                        Description = x.Description,
                        Name = x.Name,
                        Number = x.Sequence
                    }).ToList();
            ServiceLocatorSchool.GradeLevelService.Edit(gradeLevels);
        }

        private void UpdateSchoolYears()
        {
            if (context.GetSyncResult<AcadSession>().Updated == null)
                return;
            var schoolYears = context.GetSyncResult<AcadSession>().Updated.Select(x => new SchoolYear
                {
                    Id = x.AcadSessionID,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Name = x.Name,
                    SchoolRef = x.SchoolID
                }).ToList();
            ServiceLocatorSchool.SchoolYearService.Edit(schoolYears);
        }

        private void UpdateStudentSchoolYears()
        {
            if (context.GetSyncResult<StudentAcadSession>().Updated == null)
                return;
            var ssy = context.GetSyncResult<StudentAcadSession>().Updated.Select(x => new StudentSchoolYear
                {
                    SchoolYearRef = x.AcadSessionID,
                    StudentRef = x.StudentID,
                    GradeLevelRef = x.GradeLevelID.Value,
                    EnrollmentStatus = x.CurrentEnrollmentStatus == "C" ? StudentEnrollmentStatusEnum.CurrentlyEnrolled : StudentEnrollmentStatusEnum.PreviouslyEnrolled
                }).ToList();
            ServiceLocatorSchool.SchoolYearService.EditStudentSchoolYears(ssy);
        }

        private void UpdateMarkingPeriods()
        {
            if (context.GetSyncResult<Term>().Updated == null)
                return;
            var sys = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToDictionary(x => x.Id);
            var mps = context.GetSyncResult<Term>().Updated.Select(x => new MarkingPeriod
                {
                    Description = x.Description,
                    EndDate = x.EndDate,
                    Id = x.TermID,
                    Name = x.Name,
                    SchoolRef = sys[x.AcadSessionID].SchoolRef,
                    SchoolYearRef = x.AcadSessionID,
                    StartDate = x.StartDate,
                    WeekDays = 62
                }).ToList();
            ServiceLocatorSchool.MarkingPeriodService.Edit(mps);
        }

        private void UpdateGradingPeriods()
        {
            if (context.GetSyncResult<GradingPeriod>().Updated == null)
                return;
            var gps = context.GetSyncResult<GradingPeriod>().Updated.Select(x => new Data.School.Model.GradingPeriod
            {
                Description = x.Description,
                EndDate = x.EndDate,
                Id = x.GradingPeriodID,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                StartDate = x.StartDate,
                AllowGradePosting = x.AllowGradePosting,
                Code = x.Code,
                EndTime = x.EndTime,
                MarkingPeriodRef = x.TermID,
                SchoolAnnouncement = x.SchoolAnnouncement
            }).ToList();
            ServiceLocatorSchool.GradingPeriodService.Edit(gps);
        }

        private void UpdateDayTypes()
        {
            if (context.GetSyncResult<DayType>().Updated == null)
                return;
            var dts = context.GetSyncResult<DayType>().Updated.Select(x => new Data.School.Model.DayType
            {
                Id = x.DayTypeID,
                Name = x.Name,
                Number = x.Sequence,
                SchoolYearRef = x.AcadSessionID
            }).ToList();
            ServiceLocatorSchool.DayTypeService.Edit(dts);
        }

        private void UpdateDays()
        {
            if (context.GetSyncResult<CalendarDay>().Updated == null)
                return;
            var sys = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToDictionary(x => x.Id);
            var ds = context.GetSyncResult<CalendarDay>().Updated.Select(x => new Date
                {
                    DayTypeRef = x.DayTypeID,
                    IsSchoolDay = x.InSchool,
                    SchoolRef = sys[x.AcadSessionID].SchoolRef,
                    SchoolYearRef = x.AcadSessionID
                }).ToList();
            ServiceLocatorSchool.CalendarDateService.Edit(ds);
        }

        private void UpdateRooms()
        {
            if (context.GetSyncResult<Room>().Updated == null)
                return;
            var rooms = context.GetSyncResult<Room>().Updated.Select(x=>new Data.School.Model.Room
                {
                    Capacity = x.StudentCapacity,
                    Description = x.Description,
                    Id = x.RoomID,
                    RoomNumber = x.RoomNumber,
                    SchoolRef = x.SchoolID
                }).ToList();
            ServiceLocatorSchool.RoomService.EditRooms(rooms);
        }

        private void UpdateCourses()
        {
            if (context.GetSyncResult<Course>().Updated == null)
                return;
            var courses = context.GetSyncResult<Course>().Updated.ToList();
            var classes = new List<Class>();
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToDictionary(x => x.Id);
            var departmenPairs = PrepareChalkableDepartmentKeywords();
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
            ServiceLocatorSchool.ClassService.Edit(classes);
        }

        private void UpdateClassTeachers()
        {
            if (context.GetSyncResult<SectionStaff>().Updated == null)
                return;
            var teachers = context.GetSyncResult<SectionStaff>().Updated.Select(x => new ClassTeacher
            {
                ClassRef = x.SectionID,
                IsCertified = x.IsCertified,
                IsHighlyQualified = x.IsHighlyQualified,
                IsPrimary = x.IsPrimary,
                PersonRef = x.StaffID
            }).ToList();
            ServiceLocatorSchool.ClassService.EditTeachers(teachers);
        }

        private void UpdateStandardSubject()
        {
            if (context.GetSyncResult<StandardSubject>().Updated == null)
                return;
            var ss = context.GetSyncResult<StandardSubject>().Updated.Select(x => new Data.School.Model.StandardSubject
            {
                AdoptionYear = x.AdoptionYear,
                Id = x.StandardSubjectID,
                Description = x.Description,
                IsActive = x.IsActive,
                Name = x.Name
            }).ToList();
            ServiceLocatorSchool.StandardService.EditStandardSubjects(ss);
        }

        private void UpdateStandards()
        {
            if (context.GetSyncResult<Standard>().Updated == null)
                return;
            var sts = context.GetSyncResult<Standard>().Updated.Select(x => new Data.School.Model.Standard
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
            ServiceLocatorSchool.StandardService.EditStandard(sts);
        }

        private void UpdateClassStandard()
        {
            //TODO: no way to update. delete or insert only
        }

        private void UpdateMarkingPeriodClasses()
        {
            //TODO: no way to update. delete or insert only
        }

        private void UpdateScheduledTimeSlots()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.ScheduledTimeSlot>().Updated == null)
                return;
            var allSts = context.GetSyncResult<StiConnector.SyncModel.ScheduledTimeSlot>().Updated
                .Select(x => new Data.School.Model.ScheduledTimeSlot
                {
                    BellScheduleID = x.BellScheduleID,
                    Description = x.Description,
                    EndTime = x.EndTime,
                    IsDailyAttendancePeriod = x.IsDailyAttendancePeriod,
                    StartTime = x.StartTime,
                    TimeSlotID = x.TimeSlotID
                })
                .ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.Add(allSts);
        }

        private void UpdatePeriods()
        {
            if (context.GetSyncResult<TimeSlot>().Updated == null)
                return;
            var periods = context.GetSyncResult<TimeSlot>().Updated.ToList();
            var allSts = ServiceLocatorSchool.ScheduledTimeSlotService.GetAll();
            foreach (var timeSlot in periods)
            {
                var sts = allSts.FirstOrDefault(x => x.TimeSlotID == timeSlot.TimeSlotID);
                int startTime = 0;
                int endTime = 1;
                if (sts != null)
                {
                    startTime = sts.StartTime ?? 0;
                    endTime = sts.EndTime ?? startTime + 1;
                }
                ServiceLocatorSchool.PeriodService.Edit(timeSlot.TimeSlotID, startTime, endTime);//TODO: sequence update is not supported yet
            }
        }

        private void UpdateClassPeriods()
        {
            //TODO: no way to update. delete or insert only
        }

        private void UpdateClassPersons()
        {
            if (context.GetSyncResult<StudentScheduleTerm>().Updated == null)
                return;
            var mps = ServiceLocatorSchool.MarkingPeriodService.GetMarkingPeriods(null);
            var studentSchedules = context.GetSyncResult<StudentScheduleTerm>().Updated
                .Select(x => new ClassPerson
                {
                    ClassRef = x.SectionID,
                    PersonRef = x.StudentID,
                    MarkingPeriodRef = x.TermID,
                    SchoolRef = mps.First(y => y.Id == x.TermID).SchoolRef,
                    IsEnrolled = x.IsEnrolled
                }).ToList();
            ServiceLocatorSchool.ClassService.EditStudents(studentSchedules);
        }

        private void UpdateAttendanceReasons()
        {
            if (context.GetSyncResult<AbsenceReason>().Updated == null)
                return;
            var reasons = context.GetSyncResult<AbsenceReason>().Updated;
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
            ServiceLocatorSchool.AttendanceReasonService.Edit(rs);
        }

        private void UpdateAttendanceLevelReasons()
        {
            if (context.GetSyncResult<AbsenceLevelReason>().Updated == null)
                return;
            var absenceLevelReasons = context.GetSyncResult<AbsenceLevelReason>().Updated
                                                 .Select(x => new AttendanceLevelReason
                                                 {
                                                     Id = x.AbsenceLevelReasonID,
                                                     AttendanceReasonRef = x.AbsenceReasonID,
                                                     IsDefault = x.IsDefaultReason,
                                                     Level = x.AbsenceLevel
                                                 }).ToList();
            ServiceLocatorSchool.AttendanceReasonService.EditAttendanceLevelReasons(absenceLevelReasons);
        }

        private void UpdateAlphaGrades()
        {
            if (context.GetSyncResult<AlphaGrade>().Updated == null)
                return;
            var alphaGrades = context.GetSyncResult<AlphaGrade>().Updated.Select(x => new Data.School.Model.AlphaGrade
            {
                Id = x.AlphaGradeID,
                Description = x.Description,
                Name = x.Name,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.AlphaGradeService.EditAlphaGrades(alphaGrades);
        }

        private void UpdateAlternateScores()
        {
            if (context.GetSyncResult<AlternateScore>().Updated == null)
                return;
            var alternateScores = context.GetSyncResult<AlternateScore>().Updated.Select(x => new Data.School.Model.AlternateScore
            {
                Id = x.AlternateScoreID,
                Description = x.Description,
                IncludeInAverage = x.IncludeInAverage,
                Name = x.Name,
                PercentOfMaximumScore = x.PercentOfMaximumScore
            }).ToList();
            ServiceLocatorSchool.AlternateScoreService.EditAlternateScores(alternateScores);
        }

        private void UpdateInfractions()
        {
            if (context.GetSyncResult<Infraction>().Updated == null)
                return;
            var infractions = context.GetSyncResult<Infraction>().Updated.Select(x => new Data.School.Model.Infraction
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
            ServiceLocatorSchool.InfractionService.Edit(infractions);
        }

        private void UpdateGradingScales()
        {
            if (context.GetSyncResult<GradingScale>().Updated == null)
                return;
            var gs = context.GetSyncResult<GradingScale>().Updated.Select(x => new Data.School.Model.GradingScale
            {
                Id = x.GradingScaleID,
                Description = x.Description,
                HomeGradeToDisplay = x.HomeGradeToDisplay,
                Name = x.Name,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.GradingScaleService.EditGradingScales(gs);
        }

        private void UpdateGradingScaleRanges()
        {
            if (context.GetSyncResult<GradingScaleRange>().Updated == null)
                return;
            var gsr = context.GetSyncResult<GradingScaleRange>().Updated.Select(x => new Data.School.Model.GradingScaleRange
            {
                AlphaGradeRef = x.AlphaGradeID,
                AveragingEquivalent = x.AveragingEquivalent,
                AwardGradCredit = x.AwardGradCredit,
                GradingScaleRef = x.GradingScaleID,
                HighValue = x.HighValue,
                IsPassing = x.IsPassing,
                LowValue = x.LowValue
            }).ToList();
            ServiceLocatorSchool.GradingScaleService.EditGradingScaleRanges(gsr);
        }

        private void UpdateClassroomOptions()
        {
            if (context.GetSyncResult<ClassroomOption>().Updated == null)
                return;
            var cro = context.GetSyncResult<ClassroomOption>().Updated.Select(x => new Data.School.Model.ClassroomOption()
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
            ServiceLocatorSchool.ClassroomOptionService.Edit(cro);
        }

        private void UpdateGradingComments()
        {
            if (context.GetSyncResult<GradingComment>().Updated == null)
                return;
            var gc = context.GetSyncResult<GradingComment>().Updated.Select(x => new Data.School.Model.GradingComment
            {
                Code = x.Code,
                Comment = x.Comment,
                Id = x.GradingCommentID,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.GradingCommentService.Edit(gc);
        }
    }
}
