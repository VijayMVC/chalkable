using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
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
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StandardSubject = Chalkable.StiConnector.SyncModel.StandardSubject;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private void ProcessUpdate()
        {
            UpdateSchools();
            UpdateAddresses();
            UpdatePersons();
            UpdateSchoolPersons();
            UpdatePhones();
            UpdateGradeLevels();
            UpdateSchoolYears();
            UpdateStudentSchoolYears();
            UpdateMarkingPeriods();
            UpdateGradingPeriods();
            UpdateDayTypes();
            UpdateDays();
            UpdateRooms();
            UpdateCourses();
            UpdateStandardSubject();
            UpdateStandards();
            UpdateClassStandard();
            UpdateMarkingPeriodClasses();
            UpdateClassAnnouncementTypes();
            UpdatePeriods();
            UpdateClassPeriods();
            UpdateClassPersons();
            UpdateAttendanceReasons();
            UpdateAttendanceLevelReasons();
            UpdateAlphaGrades();
            UpdateAlternateScores();
            UpdateInfractions();
            UpdateGradingScales();
            UpdateGradingScaleRanges();
            UpdateClassroomOptions();
            UpdateGradingComments();
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
                    Name = x.Name
                }).ToList();
            ServiceLocatorSchool.SchoolService.Edit(schools);
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

        private void UpdatePersons()
        {
            if (context.GetSyncResult<Person>().Updated == null)
                return;
            var persons = context.GetSyncResult<Person>().Updated;
            var genders = context.GetSyncResult<Gender>().All.ToDictionary(x => x.GenderID);
            var students = context.GetSyncResult<Student>().All.ToDictionary(x => x.StudentID);
            var spEdStatuses = context.GetSyncResult<SpEdStatus>().All.ToDictionary(x => x.SpEdStatusID);
            IList<PersonInfo> pi = new List<PersonInfo>();
            
            foreach (var person in persons)
            {
                var hasMedicalAlert = false;
                var isAllowedInetAccess = false;
                string specialInstructions = "";
                string spEdStatus = null;
                if (students.ContainsKey(person.PersonID))
                {
                    hasMedicalAlert = students[person.PersonID].HasMedicalAlert;
                    isAllowedInetAccess = students[person.PersonID].IsAllowedInetAccess;
                    specialInstructions = students[person.PersonID].SpecialInstructions;
                    if (students[person.PersonID].SpEdStatusID.HasValue)
                    {
                        spEdStatus = spEdStatuses[students[person.PersonID].SpEdStatusID.Value].Name;
                    }
                }
                var chalkablePerson = ServiceLocatorSchool.PersonService.GetPerson(person.PersonID);
                var email = chalkablePerson.Email;
                pi.Add(new PersonInfo
                    {
                        Id = person.PersonID,
                        Active = true,
                        AddressRef = person.PhysicalAddressID,
                        BirthDate = person.DateOfBirth,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        Gender = person.GenderID.HasValue ? genders[person.GenderID.Value].Code : "U",
                        HasMedicalAlert = hasMedicalAlert,
                        IsAllowedInetAccess = isAllowedInetAccess,
                        SpecialInstructions = specialInstructions,
                        SpEdStatus = spEdStatus,
                        Email = email,
                        PhotoModifiedDate = person.PhotoModifiedDate
                    });
                if (person.PhotoModifiedDate.HasValue)
                {
                    if (!chalkablePerson.PhotoModifiedDate.HasValue || chalkablePerson.PhotoModifiedDate < person.PhotoModifiedDate)
                        personsForImportPictures.Add(person);
                }
            }
            ServiceLocatorSchool.PersonService.Edit(pi);
        }

        private void UpdateSchoolPersons()
        {
            //TODO: there is no way to update it....only insert or delete
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
                    GradeLevelRef = x.GradeLevelID.Value
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
                EndTime = x.EndDate,
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
            foreach (var course in courses)
            {
                var glId = course.MaxGradeLevelID ?? course.MinGradeLevelID;
                if (!glId.HasValue)
                {
                    Log.LogWarning(string.Format("No grade level for class {0}", course.CourseID));
                    continue;
                }
                var current = ServiceLocatorSchool.ClassService.GetClassById(course.CourseID);
                classes.Add(new Class
                {
                    ChalkableDepartmentRef = current.ChalkableDepartmentRef,
                    Description = course.FullName,
                    GradeLevelRef = glId.Value,
                    Id = course.CourseID,
                    Name = course.ShortName,
                    SchoolRef = course.AcadSessionID != null ? years[course.AcadSessionID.Value].SchoolRef : (int?)null,
                    SchoolYearRef = course.AcadSessionID,
                    TeacherRef = course.PrimaryTeacherID,
                    RoomRef = course.RoomID,
                    CourseRef = course.SectionOfCourseID
                });   
            }
            ServiceLocatorSchool.ClassService.Edit(classes);
        }

        public void UpdateStandardSubject()
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

        private void UpdateClassAnnouncementTypes()
        {
            if (context.GetSyncResult<ActivityCategory>().Updated == null)
                return;
            var types = context.GetSyncResult<ActivityCategory>().Updated.Select(x => new ClassAnnouncementType
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
            ServiceLocatorSchool.ClassAnnouncementTypeService.Edit(types);
        }

        private void UpdatePeriods()
        {
            if (context.GetSyncResult<TimeSlot>().Updated == null)
                return;
            var periods = context.GetSyncResult<TimeSlot>().Updated.ToList();
            var allSts = context.GetSyncResult<ScheduledTimeSlot>().All.ToList();
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
