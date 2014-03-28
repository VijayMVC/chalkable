using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AlphaGrade = Chalkable.StiConnector.SyncModel.AlphaGrade;
using AlternateScore = Chalkable.StiConnector.SyncModel.AlternateScore;
using DayType = Chalkable.StiConnector.SyncModel.DayType;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;
using GradingPeriod = Chalkable.StiConnector.SyncModel.GradingPeriod;
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
        }

        private void UpdateSchools()
        {
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
                    PostalCode = x.PostalCode
                }).ToList();
            ServiceLocatorSchool.AddressService.Edit(addresses);
        }

        private void UpdatePersons()
        {
            var genders = context.GetSyncResult<Gender>().All.ToDictionary(x => x.GenderID);
            var persons = context.GetSyncResult<Person>().Updated.Select(x => new PersonInfo
                {
                    Id = x.PersonID,
                    Active = true,
                    AddressRef = x.PhysicalAddressID,
                    BirthDate = x.DateOfBirth,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Gender = x.GenderID.HasValue ? genders[x.GenderID.Value].Code : "U"
                }).ToList();
            ServiceLocatorSchool.PersonService.Edit(persons);
        }

        private void UpdateSchoolPersons()
        {
            //TODO: there is no way to update it....only insert or delete
        }

        private void UpdatePhones()
        {
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
            //do we need to update this?
        }

        private void UpdateMarkingPeriods()
        {
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
            var gps = context.GetSyncResult<GradingPeriod>().Updated.Select(x => new Data.School.Model.GradingPeriod
            {
                Description = x.Description,
                EndDate = x.EndDate,
                Id = x.TermID,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                StartDate = x.StartDate,
                AllowGradePosting = x.AllowGradePosting,
                Code = x.Code,
                EndTime = x.EndDate,
                MarkingPeriodRef = x.TermID
            }).ToList();
            ServiceLocatorSchool.GradingPeriodService.Edit(gps);
        }

        private void UpdateDayTypes()
        {
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
            //TODO: need to solve GL problem
        }

        public void UpdateStandardSubject()
        {
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
            //TODO: can we edit this?
        }

        private void UpdateClassPersons()
        {
            //TODO: can we edit this?
        }

        private void UpdateAttendanceReasons()
        {
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
    }
}
