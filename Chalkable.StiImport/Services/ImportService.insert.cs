using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AlphaGrade = Chalkable.StiConnector.SyncModel.AlphaGrade;
using AlternateScore = Chalkable.StiConnector.SyncModel.AlternateScore;
using ClassroomOption = Chalkable.StiConnector.SyncModel.ClassroomOption;
using DayType = Chalkable.StiConnector.SyncModel.DayType;
using Person = Chalkable.StiConnector.SyncModel.Person;
using School = Chalkable.StiConnector.SyncModel.School;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StandardSubject = Chalkable.StiConnector.SyncModel.StandardSubject;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;
using GradingComment = Chalkable.StiConnector.SyncModel.GradingComment;
using GradingPeriod = Chalkable.StiConnector.SyncModel.GradingPeriod;
using GradingScale = Chalkable.StiConnector.SyncModel.GradingScale;
using GradingScaleRange = Chalkable.StiConnector.SyncModel.GradingScaleRange;
using Infraction = Chalkable.StiConnector.SyncModel.Infraction;
using Room = Chalkable.StiConnector.SyncModel.Room;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private void ProcessInsert()
        {
            InsertSchools();
            InsertAddresses();
            InsertSisUsers();
            InsertPersons();
            InsertSchoolPersons();
            InsertPhones();
            InsertGradeLevels();
            InsertSchoolYears();
            InsertStudentSchoolYears();
            InsertMarkingPeriods();
            InsertGradingPeriods();
            InsertDayTypes();
            InsertDays();
            InsertRooms();
            InsertCourses();
            InsertStandardSubject();
            InsertStandards();
            InsertClassStandard();
            InsertMarkingPeriodClasses();
            InsertClassAnnouncementTypes();
            InsertPeriods();
            InsertClassPeriods();
            InsertClassPersons();
            InsertAttendanceReasons();
            InsertAttendanceLevelReasons();
            InsertAlphaGrades();
            InsertAlternateScores();
            InsertInfractions();
            InsertGradingScales();
            InsertGradingScaleRanges();
            InsertClassroomOptions();
            InsertGradingComments();
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
                    Name = school.Name
                });
                importedSchoolIds.Add(school.SchoolID);
            }
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

        private void InsertSisUsers()
        {
            var users = context.GetSyncResult<User>().All.Select(x=>new SisUser
                {
                    Id = x.UserID,
                    IsDisabled = x.IsDisabled,
                    IsSystem = x.IsSystem,
                    LockedOut = x.LockedOut,
                    UserName = x.UserName
                }).ToList();
            ServiceLocatorSchool.SisUserService.Add(users);
        }
        
        private void InsertPersons()
        {
            int counter = 0;
            var persons = context.GetSyncResult<Person>().All;
            var ps = new List<PersonInfo>();
            var students = context.GetSyncResult<Student>().All.ToDictionary(x => x.StudentID);
            var staff = context.GetSyncResult<Staff>().All.ToDictionary(x => x.StaffID);
            Dictionary<int, SisUser> sisUsers;
            if (persons.Length > 20)
            {
                sisUsers = ServiceLocatorSchool.SisUserService.GetAll().ToDictionary(x=>x.Id);
            }
            else
            {
                var ids = students.Values.Select(x => x.UserID)
                            .Union(staff.Values.Where(x => x.UserID.HasValue).Select(x => x.UserID.Value)).ToList();
                sisUsers = ids.Select(ServiceLocatorSchool.SisUserService.GetById).ToDictionary(x => x.Id);
            }
            var genders = context.GetSyncResult<Gender>().All.ToDictionary(x => x.GenderID);
            var spEdStatuses = context.GetSyncResult<SpEdStatus>().All.ToDictionary(x => x.SpEdStatusID);
            foreach (var person in persons)
            {
                counter++;
                if (counter % 100 == 0)
                    Log.LogWarning(string.Format(ChlkResources.USERS_PROCESSED, counter));
                var email = string.Format(USER_EMAIL_FMT, person.PersonID, ServiceLocatorSchool.Context.DistrictId);
                //TODO: what about admins? probably will be resolved by API
                var userName = string.Empty;
                var hasMedicalAlert = false;
                var isAllowedInetAccess = false;
                string specialInstructions = "";
                string spEdStatus = null;

                if (students.ContainsKey(person.PersonID))
                {
                    userName = sisUsers[students[person.PersonID].UserID].UserName;
                    hasMedicalAlert = students[person.PersonID].HasMedicalAlert;
                    isAllowedInetAccess = students[person.PersonID].IsAllowedInetAccess;
                    specialInstructions = students[person.PersonID].SpecialInstructions;
                    if (students[person.PersonID].SpEdStatusID.HasValue)
                        spEdStatus = spEdStatuses[students[person.PersonID].SpEdStatusID.Value].Name;
                }
                if (staff.ContainsKey(person.PersonID) && staff[person.PersonID].UserID.HasValue)
                    userName = sisUsers[staff[person.PersonID].UserID.Value].UserName;
                
                ps.Add(new PersonInfo
                {
                    Active = true,
                    AddressRef = person.PhysicalAddressID,
                    BirthDate = person.DateOfBirth,
                    Email = email,
                    FirstName = person.FirstName,
                    Gender = person.GenderID.HasValue ? genders[person.GenderID.Value].Code : "U",
                    Id = person.PersonID,
                    LastName = person.LastName,
                    Password = ServiceLocatorMaster.UserService.PasswordMd5(DEF_USER_PASS),
                    SisUserName = userName,
                    HasMedicalAlert = hasMedicalAlert,
                    IsAllowedInetAccess = isAllowedInetAccess,
                    SpecialInstructions = specialInstructions,
                    SpEdStatus = spEdStatus,
                    PhotoModifiedDate = person.PhotoModifiedDate
                });
                if (person.PhotoModifiedDate.HasValue)
                    personsForImportPictures.Add(person);
            }
            ServiceLocatorSchool.PersonService.Add(ps);
        }

        private void InsertSchoolPersons()
        {
            var existsing = ServiceLocatorSchool.SchoolPersonService.GetAll();
            var students = context.GetSyncResult<StudentSchool>().All;
            var staff = context.GetSyncResult<StaffSchool>().All;
            IList<SchoolPerson> assignments = new List<SchoolPerson>();
            foreach (var studentSchool in students)
            {
                if (!existsing.Any(x => x.PersonRef == studentSchool.StudentID && x.SchoolRef == studentSchool.SchoolID))
                {
                    var sp = new SchoolPerson
                        {
                            RoleRef = CoreRoles.STUDENT_ROLE.Id,
                            SchoolRef = studentSchool.SchoolID,
                            PersonRef = studentSchool.StudentID
                        };
                    assignments.Add(sp);
                    existsing.Add(sp);
                }
            }
            foreach (var staffSchool in staff)
            {
                if (!existsing.Any(x => x.PersonRef == staffSchool.StaffID && x.SchoolRef == staffSchool.SchoolID))
                {
                    var sp = new SchoolPerson
                        {
                            RoleRef = CoreRoles.TEACHER_ROLE.Id,
                            SchoolRef = staffSchool.SchoolID,
                            PersonRef = staffSchool.StaffID
                        };
                    assignments.Add(sp);
                    existsing.Add(sp);
                }
            }
            ServiceLocatorSchool.PersonService.AsssignToSchool(assignments);
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
                .Select(x=>new Data.School.Model.GradingPeriod()
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
                                      .ToList();
            foreach (var dayType in dayTypes)
            {
                var dt = ServiceLocatorSchool.DayTypeService.Add(dayType.DayTypeID, dayType.Sequence, dayType.Name, dayType.AcadSessionID);
                Log.LogInfo(string.Format(ChlkResources.IMPORT_GENERAL_PERIODS_FOR_SCHEDULE_SECTION_START, dt.Id));
            }
        }

        private void InsertDays()
        {
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToDictionary(x => x.Id);

            var days = context.GetSyncResult<CalendarDay>().All
                .ToList()
                .Where(x => years.ContainsKey(x.AcadSessionID))
                .Select(x => new Date
                {
                    DayTypeRef = x.DayTypeID,
                    IsSchoolDay = x.InSchool,
                    SchoolRef = years[x.AcadSessionID].SchoolRef,
                    SchoolYearRef = x.AcadSessionID,
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

        private void InsertCourses()
        {
            var years = ServiceLocatorSchool.SchoolYearService.GetSchoolYears().ToDictionary(x => x.Id);
            var departments = ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
            var sep = new[] { ',' };
            var departmenPairs = new List<Pair<string, Guid>>();

            foreach (var chalkableDepartment in departments)
            {
                departmenPairs.AddRange(chalkableDepartment.Keywords.ToLower().Split(sep, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Pair<string, Guid>(x, chalkableDepartment.Id)));
            }

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
                string name = course.ShortName.ToLower();
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
                    Id = course.CourseID,
                    Name = course.ShortName,
                    SchoolRef = course.AcadSessionID != null ? years[course.AcadSessionID.Value].SchoolRef : (int?)null,
                    SchoolYearRef = course.AcadSessionID,
                    TeacherRef = course.PrimaryTeacherID,
                    RoomRef = course.RoomID,
                    CourseRef = course.SectionOfCourseID
                });
            }

            ServiceLocatorSchool.ClassService.Add(classes);
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
            ServiceLocatorSchool.StandardService.AddStandards(sts);
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
            var classes = ServiceLocatorSchool.ClassService.GetClasses(null);
            var mps = ServiceLocatorSchool.MarkingPeriodService.GetMarkingPeriods(null);
            var cts = context.GetSyncResult<SectionTerm>().All
                .Where(x => classes.Any(y => y.Id == x.SectionID))
                .Where(x => mps.Any(y => y.Id == x.TermID))
                .Select(x => new MarkingPeriodClass
                {
                    ClassRef = x.SectionID,
                    MarkingPeriodRef = x.TermID,
                    SchoolRef = mps.First(y=>y.Id == x.TermID).SchoolRef
                }).ToList();
            ServiceLocatorSchool.ClassService.AssignClassToMarkingPeriod(cts);
        }

        private void InsertClassAnnouncementTypes()
        {
            var types = context.GetSyncResult<ActivityCategory>().All.Select(x => new ClassAnnouncementType
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
            ServiceLocatorSchool.ClassAnnouncementTypeService.Add(types);
        }

        private void InsertPeriods()
        {
            //TODO: this logic is not exact how it is in INOW
            var periods = context.GetSyncResult<TimeSlot>().All.ToList();
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
                ServiceLocatorSchool.PeriodService.Add(timeSlot.TimeSlotID, timeSlot.AcadSessionID, startTime, endTime, timeSlot.Sequence);
            }
        }

        private void InsertClassPeriods()
        {
            var classes = ServiceLocatorSchool.ClassService.GetClasses(null).ToDictionary(x=>x.Id);
            var scheduledSections = context.GetSyncResult<ScheduledSection>().All
                .Where(x => classes.ContainsKey(x.SectionID))
                .Select(x => new ClassPeriod
                {
                    ClassRef = x.SectionID,
                    DayTypeRef = x.DayTypeID,
                    PeriodRef = x.TimeSlotID,
                    SchoolRef = classes[x.SectionID].SchoolRef.Value
                }).ToList();
            ServiceLocatorSchool.ClassPeriodService.Add(scheduledSections);
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
