using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.BusinessLogic.Services.School.Notifications;
using Chalkable.BusinessLogic.Services.School.PanoramaSettings;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using IDbMaintenanceService = Chalkable.BusinessLogic.Services.School.IDbMaintenanceService;
using ISchoolService = Chalkable.BusinessLogic.Services.School.ISchoolService;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoServiceLocatorSchool : ServiceLocator, IServiceLocatorSchool
    {
        private IServiceLocatorMaster serviceLocatorMaster;
        private IPersonService personService;
        private IAddressService addressService;
        private IGradeLevelService gradeLevelService;
        private IMarkingPeriodService markingPeriodService;
        private IClassService classService;
        private ISchoolYearService schoolYearService;
        private IAnnouncementQnAService announcementQnAService;
        //private IAnnouncementService announcementService;
        private IAnnouncementAttachmentService announcementAttachmentService;
        private IPhoneService phoneService;
        private IPrivateMessageService privateMessageService;
        private IRoomService roomService;
        private IPeriodService periodService;
        private ICalendarDateService calendarDateService;
        private IDayTypeService dayTypeService;
        private IClassPeriodService classPeriodService;
        private INotificationService notificationService;
        private IAttendanceService attendanceService;
        private IAttendanceReasonService attendanceReasonService;
        private IStudentParentService studentParentService;
        private IStudentAnnouncementService studentAnnouncementService;
        private IClassAnnouncementTypeService classAnnouncementTypeService;
        private IApplicationSchoolService applicationSchoolService;
        private IDisciplineService disciplineService;
        private IGradingStatisticService gradingStatisticService;
        private ISchoolService schoolService;
        private ISchoolPersonService schoolPersonService;
        private IStandardService standardService;
        private IAlphaGradeService alphaGradeService;
        private IAlternateScoreService alternateScoreService;
        private IGradingPeriodService gradingPeriodService;
        private IGradingStandardService gradingStandardService;
        private IGradingCommentService gradingCommentService;
        private IGradingScaleService gradingScaleService;
        private IClassroomOptionService classroomOptionService;
        private IPersonEmailService personEmailService;
        private IStudentService studentService;
        private IStaffService staffService;
        private IUserSchoolService userSchoolService;
        private IPracticeGradeService practiceGradeService;
        private IAttendanceMonthService attendanceMonthService;
        private IGradedItemService gradedItemService;
        private IAnnouncementAttributeService announcementAttributeService;
        private IContactService contactService;
        private ITeacherCommentService teacherCommentService;
        private IGroupService groupService;
        private ICourseTypeService courseTypeService;
        private ISettingsService settingsService;
        private IPersonSettingService personSettingService;
        private ILPGalleryCategoryService lpGalleryCategoryService { get; set; }
        private IAnnouncementAssignedAttributeService announcementAssignedAttributeService;


        public DemoServiceLocatorSchool(IServiceLocatorMaster serviceLocatorMaster): base(serviceLocatorMaster.Context)        
        {
            this.serviceLocatorMaster = serviceLocatorMaster;
            notificationService = new DemoNotificationService(this);
            //announcementService = new DemoAnnouncementService(this);
            personService = new DemoPersonService(this);
            schoolYearService = new DemoSchoolYearService(this);
            markingPeriodService = new DemoMarkingPeriodService(this);
            addressService = new DemoAddressService(this);
            gradeLevelService = new DemoGradeLevelService(this);
            classService = new DemoClassService(this);
            announcementQnAService = new DemoAnnouncementQnAService(this);
            announcementAttachmentService = new DemoAnnouncementAttachmentService(this);
            phoneService = new DemoPhoneService(this);
            privateMessageService = new DemoPrivateMessageService(this);
            roomService = new DemoRoomService(this);
            periodService = new DemoPeriodService(this);
            calendarDateService = new DemoCalendarDateService(this);
            dayTypeService = new DemoDayTypeService(this);
            classPeriodService = new DemoClassPeriodService(this);
            attendanceReasonService = new DemoAttendanceReasonService(this);
            attendanceService = new DemoAttendanceService(this);
            studentParentService = new DemoStudentParentService(this);
            studentAnnouncementService = new DemoStudentAnnouncementService(this);
            applicationSchoolService = new DemoApplicationSchoolService(this);
            disciplineService = new DemoDisciplineService(this);
            gradingStatisticService = new DemoGradingStatisticService(this);
            schoolService = new DemoSchoolService(this);
            schoolPersonService = new DemoSchoolPersonService(this);
            standardService = new DemoStandardService(this);
            alphaGradeService = new DemoAlphaGradeService(this);
            alternateScoreService = new DemoAlternateScoreService(this);
            gradingPeriodService = new DemoGradingPeriodService(this);
            classAnnouncementTypeService = new DemoClassAnnouncementTypeService(this);
            gradingStandardService = new DemoGradingStandardService(this);
            InfractionService = new DemoInfractionService(this);
            StorageBlobService = new DemoStorageBlobService(this);            
            gradingCommentService = new DemoGradingCommentService(this);
            gradingScaleService = new DemoGradingScaleService(this);
            classroomOptionService = new DemoClassroomOptionService(this);
            personEmailService = new DemoPersonEmailService(this);
            studentService = new DemoStudentService(this);
            staffService = new DemoStaffService(this);
            userSchoolService = new DemoUserSchoolService(this);
            practiceGradeService = new DemoPracticeGradeService(this);
            attendanceMonthService = new DemoAttendanceMonthService(this);
            gradedItemService = new DemoGradedItemService(this);
            announcementAttributeService = new DemoAnnouncementAttributeService(this);
            contactService = new DemoContactService(this);
            Setup(ServiceLocatorMaster.Context);
            teacherCommentService = new DemoTeacherCommentService(this);
            groupService = new DemoGroupService(this);
            courseTypeService = new DemoCourseTypeService(this);
            settingsService = new DemoSettingsService(this);
            announcementAssignedAttributeService = new DemoAnnouncementAssignedAttributeService(this);
            lpGalleryCategoryService = new LPGalleryCategoryService(this);
            personSettingService = new PersonSettingService(this);
        }

        public bool IsInitialized { get; private set; }

        private void Setup(UserContext context)
        {
            if (IsInitialized)
                return;
            PrepareGeneralData(context);
            AddTeacher();
            PrepareStudents();
            AddStudentsToClasses();
            AddClasses();
            AddAttendances();
            IsInitialized = true;
        }

        public IPersonService PersonService { get { return personService; } }
        public IAddressService AddressService { get { return addressService; } }
        public IGradeLevelService GradeLevelService { get { return gradeLevelService; } }
        public IMarkingPeriodService MarkingPeriodService { get { return markingPeriodService; } }
        public IClassService ClassService { get { return classService; } }
        public ISchoolYearService SchoolYearService { get { return schoolYearService; } }
        public IAnnouncementQnAService AnnouncementQnAService { get { return announcementQnAService; } }
        //public IAnnouncementService AnnouncementService { get { return announcementService; } }
        public IAnnouncementAttachmentService AnnouncementAttachmentService { get { return announcementAttachmentService; } }
        public IPhoneService PhoneService { get { return phoneService; } }
        public IPrivateMessageService PrivateMessageService { get { return privateMessageService; } }
        public IRoomService RoomService { get { return roomService; } }
        public IPeriodService PeriodService { get { return periodService; } }
        public IServiceLocatorMaster ServiceLocatorMaster { get { return serviceLocatorMaster; } }
        public ICalendarDateService CalendarDateService { get { return calendarDateService; } }
        public IDayTypeService DayTypeService { get { return dayTypeService; } }
        public IClassPeriodService ClassPeriodService { get { return classPeriodService; } }
        public INotificationService NotificationService { get { return notificationService; } }
        public IAttendanceService AttendanceService { get { return attendanceService; } }
        public IAttendanceReasonService AttendanceReasonService { get { return attendanceReasonService; } }
        public IStudentParentService StudentParentService { get { return studentParentService; } }
        public IStudentAnnouncementService StudentAnnouncementService { get { return studentAnnouncementService; } }
        public IClassAnnouncementTypeService ClassAnnouncementTypeService { get { return classAnnouncementTypeService; } }
        public IInfractionService InfractionService { get; private set; }
        public IApplicationSchoolService ApplicationSchoolService { get { return applicationSchoolService; } }
        public IDisciplineService DisciplineService { get { return disciplineService; } }
        public IGradingStatisticService GradingStatisticService { get { return gradingStatisticService; } }
        public ISchoolService SchoolService { get { return schoolService; } }
        public ISchoolPersonService SchoolPersonService { get { return schoolPersonService; } }
        public IStandardService StandardService { get { return standardService; } }
        public IAlphaGradeService AlphaGradeService { get { return alphaGradeService; } }
        public IAlternateScoreService AlternateScoreService { get { return alternateScoreService; } }
        public IGradingPeriodService GradingPeriodService { get {return gradingPeriodService;} }
        public ISyncService SyncService { get; private set; }
        public IGradingStandardService GradingStandardService { get { return gradingStandardService; } }
        public IReportingService ReportService { get; private set; }
        public IGradingCommentService GradingCommentService { get { return gradingCommentService; } }
        public IGradingScaleService GradingScaleService { get { return gradingScaleService; } }
        public IClassroomOptionService ClassroomOptionService { get { return classroomOptionService; } }
        public IPersonEmailService PersonEmailService { get { return personEmailService; } }
        public IStudentService StudentService { get { return studentService; } }
        public IStaffService StaffService { get { return staffService; } }
        public IUserSchoolService UserSchoolService { get { return userSchoolService; } }

        public IScheduledTimeSlotService ScheduledTimeSlotService { get{throw new NotImplementedException();} }
        public IBellScheduleService BellScheduleService { get { throw new NotImplementedException(); } }
        public IPracticeGradeService PracticeGradeService{get{ return practiceGradeService; } }
        public IAttendanceMonthService AttendanceMonthService { get { return attendanceMonthService; } }
        public IGradedItemService GradedItemService { get { return gradedItemService; } }
        public IAnnouncementAttributeService AnnouncementAttributeService { get { return announcementAttributeService; } }
        public IAnnouncementAssignedAttributeService AnnouncementAssignedAttributeService { get; private set; }
        public IContactService ContactService { get { return contactService; } }
        public IGroupService GroupService { get { return groupService; } }
        public IDbService SchoolDbService { get { throw new NotImplementedException(); } }
        public ITeacherCommentService TeacherCommentService { get { return teacherCommentService; } }
        public IDbMaintenanceService DbMaintenanceService { get { throw new NotImplementedException(); } }
        public ICourseTypeService CourseTypeService { get { return courseTypeService; } }

        public ISettingsService SettingsService { get { return settingsService; } }
        public IPersonSettingService PersonSettingService { get { return personSettingService; } }
        public ILPGalleryCategoryService LPGalleryCategoryService { get { return lpGalleryCategoryService; } }

        private void AddAttendances()
        {
            var gp = GradingPeriodService.GetGradingPeriodsDetails(DemoSchoolConstants.CurrentSchoolYearId).FirstOrDefault();
            if (gp == null) return;
            for (var classId = DemoSchoolConstants.AlgebraClassId; classId <= DemoSchoolConstants.PreCalculusClassId; ++classId)
                ((DemoAttendanceService)AttendanceService).GenerateSectionAttendanceForClass(classId, gp.StartDate, gp.EndDate);
        }

        private void AddClasses()
        {
            AddClass(DemoSchoolConstants.AlgebraClassId, "Algebra", "MA103.01");
            AddClass(DemoSchoolConstants.GeometryClassId, "Geometry", "MA201.01");
            AddClass(DemoSchoolConstants.PhysicsClassId, "Physics", "SC103.G");
            AddClass(DemoSchoolConstants.PreCalculusClassId, "Pre-Calculus", "MA203.11");

        }

        public static string BuildDemoEmail(int personId, string districtId)
        {
            return "demo-user_" + personId + "_" + districtId + "@chalkable.com";
        }

        private void PrepareStudents()
        {
            AddStudent(DemoSchoolConstants.Student1, "KAYE", "BURGESS", "F", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 11, 27));
            AddStudentAddress(DemoSchoolConstants.Student1, new Address());

            AddStudent(DemoSchoolConstants.Student2, "BRYON", "BOWEN", "M", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 1, 23));
            AddStudentAddress(DemoSchoolConstants.Student2, new Address
            {
                AddressNumber = "A1026",
                StreetNumber = "3512",
                AddressLine1 = "William McKinley Causeway",
                City = "Anytown",
                PostalCode = "99999",
                Country = "USA",
                CountyId = 200
            });

            AddStudent(DemoSchoolConstants.Student3, "ADRIAN", "BEAN", "F", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 3, 10));
            AddStudentAddress(DemoSchoolConstants.Student3, new Address
            {
                AddressNumber = "A1185",
                StreetNumber = "4350",
                AddressLine1 = "Ronald Wilson Reagan Grade",
                City = "Anytown",
                PostalCode = "99999",
                Country = "USA",
                CountyId = 220
            });

            AddStudent(DemoSchoolConstants.Student4, "COLLEEN", "HOLDEN", "F", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 9, 17));
            AddStudentAddress(DemoSchoolConstants.Student4, new Address());

            AddStudent(DemoSchoolConstants.Student5, "INGRID", "LOWERY", "F", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 2, 12));
            AddStudentAddress(DemoSchoolConstants.Student5, new Address());

            AddStudent(DemoSchoolConstants.Student6, "LUCIA", "SNYDER", "F", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 8, 12));
            AddStudentAddress(DemoSchoolConstants.Student6, new Address
            {
                AddressNumber = "A1008",
                StreetNumber = "3201",
                AddressLine1 = "Birch Highway",
                City = "Anytown",
                PostalCode = "99999",
                Country = "USA",
                CountyId = 220
            });

            AddStudent(DemoSchoolConstants.Student7, "BYRON", "BYERS", "M", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 12, 14));
            AddStudentAddress(DemoSchoolConstants.Student7, new Address
            {
                AddressNumber = "A1270",
                StreetNumber = "2608",
                AddressLine1 = "John Adams Terrace",
                City = "Anytown",
                PostalCode = "99999",
                Country = "USA",
                CountyId = 220
            });

            AddStudent(DemoSchoolConstants.Student8, "NOEL", "BOYD", "M", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 12, 14));
            AddStudentAddress(DemoSchoolConstants.Student8, new Address());

            AddStudent(DemoSchoolConstants.Student9, "JAMEL", "HARRIS", "M", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 4, 4));
            AddStudentAddress(DemoSchoolConstants.Student9, new Address());

            AddStudent(DemoSchoolConstants.Student10, "MOLLIE", "PAUL", "F", DemoSchoolConstants.GradeLevel12, new DateTime(1998, 4, 4));
            AddStudentAddress(DemoSchoolConstants.Student10, new Address
            {
                AddressNumber = "A1036",
                StreetNumber = "4445",
                AddressLine1 = "Birch Cove",
                City = "Anytown",
                PostalCode = "99999",
                Country = "USA",
                CountyId = 220
            });
        }

        private void AddStudentAddress(int studentId, Address address)
        {
            address.Id = studentId;
            AddressService.Add(new[] {address});
        }

        private void AddStudentsToClasses()
        {
            var studentIds = ((DemoPersonService)PersonService).GetPersons(new PersonQuery()
            {
                RoleId = CoreRoles.STUDENT_ROLE.Id
            }).Persons.Select(x => x.Id);
            var enumerable = studentIds as int[] ?? studentIds.ToArray();

            for (var cls = DemoSchoolConstants.AlgebraClassId; cls <= DemoSchoolConstants.PreCalculusClassId; ++cls)
            {

                foreach (var studentId in enumerable)
                {
                    AddStudentToClass(studentId, cls, DemoSchoolConstants.FirstMarkingPeriodId);
                    AddStudentToClass(studentId, cls, DemoSchoolConstants.SecondMarkingPeriodId);
                }
            }
        }

        private void AddTeacher()
        {
            var person = new Person
            {
                Active = true,
                Gender = "M",
                FirstName = "ROCKY",
                LastName = "STEIN",
                Id = DemoSchoolConstants.TeacherId,
                RoleRef = CoreRoles.TEACHER_ROLE.Id,
                FirstLoginDate = DateTime.Now
            };

            PersonService.Add(new[] {person});

            StaffService.Add(new[] {new Staff
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                BirthDate = person.BirthDate,
                Gender = person.Gender,
                UserId = DemoSchoolConstants.TeacherId,
            }});

            ((DemoSchoolPersonService)SchoolPersonService).AddSchoolPerson(new SchoolPerson
            {
                PersonRef = DemoSchoolConstants.TeacherId,
                RoleRef = CoreRoles.TEACHER_ROLE.Id,
                SchoolRef = DemoSchoolConstants.SchoolId
            });
        }

        private void AddStudent(int id, string firstName, string lastName, string gender, int gradeLevelRef, DateTime birthDate)
        {
            PersonService.Add(new[] {new Person
            {
                BirthDate = birthDate,
                Active = true,
                FirstLoginDate = DateTime.Now,
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                RoleRef = CoreRoles.STUDENT_ROLE.Id,
                AddressRef = id,
            }});

            StudentService.AddStudents(new[] {new Student
            {
                Id = id,
                BirthDate = birthDate,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                UserId = id
            }});


            ((DemoSchoolPersonService)SchoolPersonService).AddSchoolPerson(new SchoolPerson
            {
                PersonRef = id,
                RoleRef = CoreRoles.STUDENT_ROLE.Id,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            SchoolYearService.AssignStudent(new[] {new StudentSchoolYear
            {
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                StudentRef = id,
                GradeLevel = ((DemoGradeLevelService)GradeLevelService).GetGradeLevelById(gradeLevelRef),
                GradeLevelRef = gradeLevelRef,
                EnrollmentStatus = StudentEnrollmentStatusEnum.CurrentlyEnrolled
            }});

            ((DemoDisciplineService)DisciplineService).AddDisciplineReferral(new DisciplineReferral
            {
                Date = DateTime.Now.Date,
                //Infractions = InfractionService.GetInfractions(),
                StudentId = id
            });
        }

        private void AddStudentToClass(int studentId, int classId, int markingPeriodId)
        {
            ((DemoClassService)ClassService).AddStudents(new[] {new ClassPerson
            {
                ClassRef = classId,
                MarkingPeriodRef = markingPeriodId,
                PersonRef = studentId,
                IsEnrolled = true
            }});

            var standards = standardService.GetStandards(null, null, null, null).Select(x => x.Id).ToList();
            var gradingPeriods = ((DemoGradingPeriodService)GradingPeriodService).GetGradingPeriods().Select(x => x.Id).ToList();


            foreach (var gp in gradingPeriods)
            {
                foreach (var standardId in standards)
                {
                    ((DemoGradingStandardService)GradingStandardService).AddStandardScore(new StandardScore
                    {
                        SectionId = classId,
                        GradingPeriodId = gp,
                        StudentId = studentId,
                        StandardId = standardId,
                    });
                }
            }
        }

        private void PrepareGeneralData(UserContext context)
        {
            ((Master.DemoSchoolService)ServiceLocatorMaster.SchoolService).AddMasterSchool(context.DistrictId);
            ((DemoCalendarDateService)CalendarDateService).AddDates();
            ((DemoDistrictService)ServiceLocatorMaster.DistrictService).AddDistrict(context);
            AddPeriods();
            AddMarkingPeriods();
            AddDayTypes();
            AddScheduleTimeSlots();
            AddGradingPeriods();
            AddSchool();
            AddSchoolYear();
            AddStandards();
            AddStandardSubjects();
            AddAlphaGrades();
            AddGradeLevels();
            AddGradingScales();
            AddGradingScaleRanges();
            AddAttendanceLevelReasons();
            AddAttendanceReasons();
            AddInfractions();
        }

        private void AddMarkingPeriods()
        {
            var currentYear = DateTime.Now.Year;

            var mpList = new List<MarkingPeriod>
            {
                new MarkingPeriod
                {
                    Id = DemoSchoolConstants.FirstMarkingPeriodId,
                    Name = "Semester 1",
                    Description = "",
                    StartDate = new DateTime(currentYear, 1, 1),
                    EndDate = new DateTime(currentYear, 6, 30),
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                    WeekDays = 62
                },
                new MarkingPeriod
                {
                    Id = DemoSchoolConstants.SecondMarkingPeriodId,
                    Name = "Semester 2",
                    Description = "",
                    StartDate = new DateTime(currentYear, 7, 1),
                    EndDate = new DateTime(currentYear, 12, 31),
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                    WeekDays = 62
                }
            };

            MarkingPeriodService.Add(mpList);
        }

        private void AddGradingScaleRanges()
        {
            var gdScaleRanges = new List<GradingScaleRange>()
            {
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale1,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_E,
                    LowValue = 84.50m,
                    HighValue = 100.0m,
                    AveragingEquivalent = 100,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale1,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_S,
                    LowValue = 69.50m,
                    HighValue = 84.49999m,
                    AveragingEquivalent = 84,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale1,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_NoPass,
                    LowValue = 0.0m,
                    HighValue = 69.499999m,
                    AveragingEquivalent = 69,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_A,
                    LowValue = 93.50m,
                    HighValue = 96.49999m,
                    AveragingEquivalent = 96,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_B,
                    LowValue = 83.50m,
                    HighValue = 86.49999m,
                    AveragingEquivalent = 86,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_C,
                    LowValue = 73.50m,
                    HighValue = 76.49999m,
                    AveragingEquivalent = 76,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_D,
                    LowValue = 63.50m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 66,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_D,
                    LowValue = 63.50m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 66,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_F,
                    LowValue = 0.0m,
                    HighValue = 60.49999m,
                    AveragingEquivalent = 60,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Aplus,
                    LowValue = 96.5m,
                    HighValue = 100.00m,
                    AveragingEquivalent = 100,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Aminus,
                    LowValue = 90.5m,
                    HighValue = 93.49999m,
                    AveragingEquivalent = 93,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Bplus,
                    LowValue = 86.5m,
                    HighValue = 90.49999m,
                    AveragingEquivalent = 90,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Bminus,
                    LowValue = 80.5m,
                    HighValue = 83.49999m,
                    AveragingEquivalent = 83,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Cplus,
                    LowValue = 76.5m,
                    HighValue = 80.49999m,
                    AveragingEquivalent = 80,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Cminus,
                    LowValue = 70.5m,
                    HighValue = 73.49999m,
                    AveragingEquivalent = 73,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Dplus,
                    LowValue = 66.5m,
                    HighValue = 70.49999m,
                    AveragingEquivalent = 70,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale2,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Dminus,
                    LowValue = 60.5m,
                    HighValue = 63.49999m,
                    AveragingEquivalent = 63,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_A,
                    LowValue = 93.0m,
                    HighValue = 97.49999m,
                    AveragingEquivalent = 95,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_B,
                    LowValue = 83.0m,
                    HighValue = 86.49999m,
                    AveragingEquivalent = 85,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_C,
                    LowValue = 73.0m,
                    HighValue = 76.49999m,
                    AveragingEquivalent = 75,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_D,
                    LowValue = 63.0m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 65,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_F,
                    LowValue = 1.0m,
                    HighValue = 59.49999m,
                    AveragingEquivalent = 50,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_E,
                    LowValue = 0.05m,
                    HighValue = 0.06m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_S,
                    LowValue = 0.03m,
                    HighValue = 0.04m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_NoPass,
                    LowValue = 0.00m,
                    HighValue = 0.02m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Aplus,
                    LowValue = 98.00m,
                    HighValue = 110.00m,
                    AveragingEquivalent = 100,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Aminus,
                    LowValue = 90.00m,
                    HighValue = 92.9999m,
                    AveragingEquivalent = 91,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Bplus,
                    LowValue = 87.00m,
                    HighValue = 89.9999m,
                    AveragingEquivalent = 88,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Bminus,
                    LowValue = 80.00m,
                    HighValue = 82.9999m,
                    AveragingEquivalent = 81,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Cplus,
                    LowValue = 77.00m,
                    HighValue = 79.9999m,
                    AveragingEquivalent = 78,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Cminus,
                    LowValue = 70.00m,
                    HighValue = 72.9999m,
                    AveragingEquivalent = 71,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Dplus,
                    LowValue = 67.00m,
                    HighValue = 69.9999m,
                    AveragingEquivalent = 68,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Dminus,
                    LowValue = 60.00m,
                    HighValue = 62.9999m,
                    AveragingEquivalent = 61,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Pass,
                    LowValue = 00.70m,
                    HighValue = 00.8m,
                    AveragingEquivalent = 61,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = DemoSchoolConstants.GradingScale3,
                    AlphaGradeRef = DemoSchoolConstants.AlphaGrade_Incomplete,
                    LowValue = 00.90m,
                    HighValue = 0.99999m,
                    AveragingEquivalent = 50,
                }
            };
            GradingScaleService.AddGradingScaleRanges(gdScaleRanges);
        }

        private void AddGradingScales()
        {
            GradingScaleService.AddGradingScales(new List<GradingScale>
            {
                new GradingScale()
                {
                    Id = DemoSchoolConstants.GradingScale1,
                    Name = "K-3 Grade Scale",
                    SchoolRef = DemoSchoolConstants.SchoolId
                },
                new GradingScale()
                {
                    Id = DemoSchoolConstants.GradingScale2,
                    Name = "Grades 4-5",
                    SchoolRef = DemoSchoolConstants.SchoolId
                },
                new GradingScale()
                {
                    Id = DemoSchoolConstants.GradingScale3,
                    Name = "Upper School",
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    HomeGradeToDisplay = 0
                }
            });
        }

        private void AddGradingPeriods()
        {
            var currentYear = DateTime.Now.Year;

            var gpList = new List<GradingPeriod>
            {
                new GradingPeriod
                {
                    Id = DemoSchoolConstants.GradingPeriodQ1,
                    Name = "Quarter 1",
                    AllowGradePosting = false,
                    Code = "Q1",
                    Description = "",
                    MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                    SchoolAnnouncement = "",
                    StartDate = new DateTime(currentYear, 1, 1),
                    EndDate = new DateTime(currentYear, 3, 31),
                    EndTime = new DateTime(currentYear, 3, 31, 23, 59, 0),
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                },
                new GradingPeriod
                {
                    Id = DemoSchoolConstants.GradingPeriodQ2,
                    Name = "Quarter 2",
                    AllowGradePosting = false,
                    Code = "Q1",
                    Description = "",
                    MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                    SchoolAnnouncement = "",
                    StartDate = new DateTime(currentYear, 4, 1),
                    EndDate = new DateTime(currentYear, 6, 30),
                    EndTime = new DateTime(currentYear, 6, 30, 23, 59, 0),
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                },
                new GradingPeriod
                {
                    Id = DemoSchoolConstants.GradingPeriodQ3,
                    Name = "Quarter 3",
                    AllowGradePosting = false,
                    Code = "Q3",
                    Description = "",
                    MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                    SchoolAnnouncement = "",
                    StartDate = new DateTime(currentYear, 7, 1),
                    EndDate = new DateTime(currentYear, 9, 30),
                    EndTime = new DateTime(currentYear, 9, 30, 23, 59, 0),
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                },
                new GradingPeriod
                {
                    Id = DemoSchoolConstants.GradingPeriodQ4,
                    Name = "Quarter 4",
                    AllowGradePosting = false,
                    Code = "Q4",
                    Description = "",
                    MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                    SchoolAnnouncement = "",
                    StartDate = new DateTime(currentYear, 10, 1),
                    EndDate = new DateTime(currentYear, 12, 31),
                    EndTime = new DateTime(currentYear, 12, 31, 23, 59, 0),
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                }
            };

            GradingPeriodService.Add(gpList);
        }

        private void AddGradeLevels()
        {
            var gradeLevels = new List<GradeLevel>();
            for (var lvl = DemoSchoolConstants.GradeLevel1; lvl <= DemoSchoolConstants.GradeLevel12; ++lvl)
            {
                gradeLevels.Add(new GradeLevel
                {
                    Description = "",
                    Id = lvl,
                    Name = lvl.ToString(CultureInfo.InvariantCulture),
                    Number = lvl
                });
            }

            GradeLevelService.Add(gradeLevels);
        }

        private void AddDayTypes()
        {
            var dayTypeList = new List<DayType>
            {
                new DayType
                {
                    Id = DemoSchoolConstants.DayTypeId1,
                    Name = "M",
                    Number = 0,
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                },
                new DayType
                {
                    Id = DemoSchoolConstants.DayTypeId2,
                    Name = "TTh",
                    Number = 1,
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                },
                new DayType
                {
                    Id = DemoSchoolConstants.DayTypeId3,
                    Name = "WF",
                    Number = 2,
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                }
            };

            DayTypeService.Add(dayTypeList);
        }

        private void AddAttendanceLevelReasons()
        {
            var attendanceLevelReasons = new List<AttendanceLevelReason>
            {
                new AttendanceLevelReason
                {
                    Level = "AO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_ISS,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "AO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_SchoolActivity,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "HO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_ISS,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "HO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_SchoolActivity,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = DemoSchoolConstants.AR_DoctorDentist,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = DemoSchoolConstants.AR_FamilyReasonExcused,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = DemoSchoolConstants.AR_FamilyReasonUnexcused,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = DemoSchoolConstants.AR_Illness,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = DemoSchoolConstants.AR_NonCompliance,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = DemoSchoolConstants.AR_OSS,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = DemoSchoolConstants.AR_Skipping,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_DoctorDentist,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_FamilyReasonExcused,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_FamilyReasonUnexcused,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_Illness,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_LateBus,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_MissedBus,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_NonCompliance,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_OSS,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = DemoSchoolConstants.AR_Skipping,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_DoctorDentist,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_FamilyReasonExcused,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_FamilyReasonUnexcused,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_Illness,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_LateBus,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_MissedBus,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_NonCompliance,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_OSS,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_Skipping,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = DemoSchoolConstants.AR_EarlyCheckout,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = DemoSchoolConstants.AR_DoctorDentist,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = DemoSchoolConstants.AR_FamilyReasonExcused,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = DemoSchoolConstants.AR_FamilyReasonUnexcused,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = DemoSchoolConstants.AR_Illness,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = DemoSchoolConstants.AR_LateBus,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = DemoSchoolConstants.AR_MissedBus,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = DemoSchoolConstants.AR_Skipping,
                    IsDefault = false
                }
            };

            AttendanceReasonService.AddAttendanceLevelReasons(attendanceLevelReasons);
        }

        private void AddAttendanceReasons()
        {
            var reasons = new List<AttendanceReason>
            {
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_Illness,
                    Code = "IL",
                    Name = "Illness",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_SchoolActivity,
                    Code = "SA",
                    Name = "School Activity",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_FamilyReasonExcused,
                    Code = "FRE",
                    Name = "Family Reason(Excused)",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_FamilyReasonUnexcused,
                    Code = "FRU",
                    Name = "Family Reason(Unexcused)",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_OSS,
                    Code = "OSS",
                    Name = "Out of School Suspension",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_ISS,
                    Code = "ISS",
                    Name = "In-School Suspension",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_Skipping,
                    Code = "SK",
                    Name = "Skipping",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_DoctorDentist,
                    Code = "DD",
                    Name = "Doctor or Dentist",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_NonCompliance,
                    Code = "IM",
                    Name = "Noncompliance",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_MissedBus,
                    Code = "MB",
                    Name = "Missed Bus",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_LateBus,
                    Code = "LB",
                    Name = "Late Bus",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = false
                },
                new AttendanceReason
                {
                    Id = DemoSchoolConstants.AR_EarlyCheckout,
                    Code = "EC",
                    Name = "Early Checkout",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                }
            };

            AttendanceReasonService.Add(reasons);
        }

        private void AddPeriods()
        {
            var periodsList = new List<Period>
            {
                new Period
                {
                    Order = 1,
                    Name = "01",
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                },
                new Period
                {
                    Order = 2,
                    Name = "02",
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                },
                new Period
                {
                    Order = 3,
                    Name = "03",
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                },
                new Period
                {
                    Order = 4,
                    Name = "04",
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
                }
            };

            PeriodService.AddPeriods(periodsList);
            
        }

        private void AddScheduleTimeSlots()
        {
            var times = new Dictionary<int, KeyValuePair<int, int>>();
            times[1] = new KeyValuePair<int, int>(663, 707);
            times[2] = new KeyValuePair<int, int>(710, 740);
            times[3] = new KeyValuePair<int, int>(744, 783);
            times[4] = new KeyValuePair<int, int>(787, 826);

            var periods = PeriodService.GetPeriods(DemoSchoolConstants.CurrentSchoolYearId);

            var slots = periods.Select(period => new ScheduledTimeSlot()
            {
                PeriodRef = period.Id,
                BellScheduleRef = DemoSchoolConstants.BellScheduleId,
                IsDailyAttendancePeriod = true,
                StartTime = times[period.Id].Key,
                EndTime = times[period.Id].Value,
            }).ToList();
            ((DemoClassPeriodService)ClassPeriodService).AddScheduleTimeSlots(slots);
        }

        private void AddSchool()
        {
            SchoolService.Add(new Data.School.Model.School
            {
                Id = DemoSchoolConstants.SchoolId,
                IsActive = true,
                IsPrivate = true,
                Name = "SMITH"
            });

            SchoolService.AddSchoolOptions(new[] {new SchoolOption
            {
                Id = DemoSchoolConstants.SchoolId,
                AllowSectionAverageModification = true,
                DefaultCombinationIndex = 1,
                AllowScoreEntryForUnexcused = true,
                DisciplineOverwritesAttendance = false,
                AllowDualEnrollment = true,
                AveragingMethod = "A",
                CategoryAveraging = false,
                StandardsCalculationMethod = "A",
                StandardsCalculationRule = "G",
                StandardsCalculationWeightMaximumValues = false,
                LockCategories = false,
                IncludeReportCardCommentsInGradebook = false,
                MergeRostersForAttendance = true
            }});
        }

        private void AddAlphaGrades()
        {
            AlphaGradeService.AddAlphaGrades(new List<AlphaGrade>()
            {
                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_A,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "A"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_B,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "B"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_C,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "C"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_D,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "D"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_F,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "F"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_E,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "E"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_S,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "S"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_NoPass,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "N",
                    Description = "No pass"

                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Aplus,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "A+"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Aminus,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "A-"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Bplus,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "B+"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Bminus,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "B-"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Cplus,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "C+"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Cminus,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "C-"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Dplus,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "D+"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Dminus,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "D-"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Pass,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "P",
                    Description = "Pass"
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Audit,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "Audit",
                },

                new AlphaGrade()
                {
                    Id = DemoSchoolConstants.AlphaGrade_Incomplete,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "I",
                    Description = "Incomplete"
                }

            });
        }

        private void AddSchoolYear()
        {
            SchoolYearService.Add(new[] {DemoSchoolYearService.GetDemoSchoolYear()});
        }

        private void AddStandards()
        {
            StandardService.AddStandards(new List<Standard>
            {
                new Standard
                {
                    Id = DemoSchoolConstants.MathStandard1,
                    IsActive = true,
                    Name = "Math 1",
                    LowerGradeLevelRef = DemoSchoolConstants.GradeLevel10,
                    UpperGradeLevelRef = DemoSchoolConstants.GradeLevel10,
                    StandardSubjectRef = DemoSchoolConstants.ScienceStandardSubject,
                    
                },

                new Standard
                {
                    Id = DemoSchoolConstants.MathStandard2,
                    IsActive = true,
                    Name = "Math 2",
                    LowerGradeLevelRef = DemoSchoolConstants.GradeLevel11,
                    UpperGradeLevelRef = DemoSchoolConstants.GradeLevel11,
                    StandardSubjectRef = DemoSchoolConstants.ScienceStandardSubject
                },

                new Standard
                {
                    Id = DemoSchoolConstants.MathStandard3,
                    IsActive = true,
                    Name = "Math 3",
                    LowerGradeLevelRef = DemoSchoolConstants.GradeLevel12,
                    UpperGradeLevelRef = DemoSchoolConstants.GradeLevel12,
                    StandardSubjectRef = DemoSchoolConstants.ScienceStandardSubject
                },

            });
        }

        private void AddStandardSubjects()
        {
            StandardService.AddStandardSubjects(new List<StandardSubject>
            {
                new StandardSubject()
                {
                    Id = 1,
                    AdoptionYear = 2010,
                    Name = "Math Standards",
                    Description = "",
                    IsActive = true,
                },

                new StandardSubject()
                {
                    Id = 2,
                    AdoptionYear = 2010,
                    Name = "Reading Standards",
                    Description = "",
                    IsActive = true
                },

                new StandardSubject()
                {
                    Id = DemoSchoolConstants.ScienceStandardSubject,
                    AdoptionYear = 2010,
                    Name = "Science Standards",
                    Description = "",
                    IsActive = true
                },

                new StandardSubject()
                {
                    Id = 4,
                    AdoptionYear = 2100,
                    Name = "Dance Standards",
                    Description = "",
                    IsActive = true
                }
            });
        }

        private void AddInfractions()
        {
            var infractions = new List<Data.School.Model.Infraction>();

            var infr1 = new Data.School.Model.Infraction
            {
                Code = "FI",
                Demerits = 0,
                Description = "Fighting",
                IsActive = true,
                IsSystem = false,
                Name = "Fighting",
                VisibleInClassroom = true,
            };

            var infr2 = new Data.School.Model.Infraction
            {
                Code = "DI",
                Demerits = 0,
                Description = "Disrespect",
                IsActive = true,
                IsSystem = false,
                Name = "Disrespect",
                VisibleInClassroom = true,
            };

            var infr3 = new Data.School.Model.Infraction
            {
                Code = "CHEAT",
                Demerits = 0,
                Description = "",
                IsActive = true,
                IsSystem = false,
                Name = "D - Cheating on assignment or test",
                VisibleInClassroom = true,
            };

            var infr4 = new Data.School.Model.Infraction
            {
                Code = "DISR",
                Demerits = 0,
                Description = "",
                IsActive = true,
                IsSystem = false,
                Name = "D - Disrespect to a staff member",
                VisibleInClassroom = true,
            };

            var infr5 = new Data.School.Model.Infraction
            {
                Code = "XDRPT",
                Demerits = 0,
                Description = "",
                IsActive = true,
                IsSystem = false,
                Name = "D - Extreme disruption of class",
                VisibleInClassroom = true
            };

            var infr6 = new Data.School.Model.Infraction
            {
                Code = "Unif",
                Demerits = 0,
                Description = "",
                IsActive = true,
                IsSystem = false,
                Name = "D - Uniform violation 3x times in one quarter",
                VisibleInClassroom = true
            };

            var infr7 = new Data.School.Model.Infraction
            {
                Code = "INACT",
                Demerits = 0,
                Description = "",
                IsActive = true,
                IsSystem = false,
                Name = "S - Inappropriate contact with another student",
                VisibleInClassroom = true
            };

            var infr8 = new Data.School.Model.Infraction
            {
                Code = "LIE",
                Demerits = 0,
                Description = "",
                IsActive = true,
                IsSystem = false,
                Name = "D - Lying to teacher or staff member",
                VisibleInClassroom = true
            };

            var infr9 = new Data.School.Model.Infraction
            {
                Code = "DEFI",
                Demerits = 0,
                Description = "",
                IsActive = true,
                IsSystem = false,
                Name = "D - Repeated disobedience or defiance",
                VisibleInClassroom = true
            };

            var infr10 = new Data.School.Model.Infraction
            {
                Code = "STEAL",
                Demerits = 0,
                Description = "",
                IsActive = true,
                IsSystem = false,
                Name = "D - Stealing form aonther student or teacher",
                VisibleInClassroom = true
            };

            infractions.Add(infr1);
            infractions.Add(infr2);
            infractions.Add(infr3);
            infractions.Add(infr4);
            infractions.Add(infr5);
            infractions.Add(infr6);
            infractions.Add(infr7);
            infractions.Add(infr8);
            infractions.Add(infr9);
            infractions.Add(infr10);

            InfractionService.Add(infractions);
        }

        private void AddClass(int id, string name, string classNumber)
        {
            ((DemoClassService)ClassService).AddClass(id, name, classNumber);
            //todo: implement this later
            //((DemoAnnouncementService)AnnouncementService).AddDemoAnnouncementsForClass(id);
            AddSeatingChartForClass(DemoSchoolConstants.FirstMarkingPeriodId, id);
            AddSeatingChartForClass(DemoSchoolConstants.SecondMarkingPeriodId, id);
        }

        private void AddSeatingChartForClass(int mpId, int classId)
        {

            const int seatCount = 3;
            var seats = new List<Seat>();


            for (var i = 0; i < seatCount; ++i)
            {
                for (var j = 0; j < seatCount; ++j)
                {
                    seats.Add(new Seat
                    {
                        Column = (byte?)(j + 1),
                        Row = (byte?)(i + 1)
                    });
                }
            }

            seats[0].StudentId = DemoSchoolConstants.Student1;


            ((DemoAttendanceService)AttendanceService).AddSeatingChart(mpId, classId, new SeatingChart()
            {
                SectionId = classId,
                Columns = seatCount,
                Rows = seatCount,
                Seats = seats
            });
        }

        public void Update(UserContext context)
        {
            Context = context;
            ((DemoServiceLocatorMaster)serviceLocatorMaster).Update(context);
            //todo : implement this later
            //((DemoAnnouncementService)AnnouncementService).SetupAnnouncementProcessor(context, this);
        }


        public School.Announcements.ILessonPlanService LessonPlanService
        {
            get { throw new NotImplementedException(); }
        }

        public School.Announcements.IClassAnnouncementService ClassAnnouncementService
        {
            get { throw new NotImplementedException(); }
        }

        public School.Announcements.IAdminAnnouncementService AdminAnnouncementService
        {
            get { throw new NotImplementedException(); }
        }

        public School.Announcements.IAnnouncementFetchService AnnouncementFetchService
        {
            get { throw new NotImplementedException(); }
        }

        public School.Announcements.IBaseAnnouncementService GetAnnouncementService(Data.School.Model.Announcements.AnnouncementTypeEnum? type)
        {
            throw new NotImplementedException();
        }

        public IAttachementService AttachementService { get { throw new NotImplementedException(); } }

        public ILEService LeService { get { throw new NotImplementedException(); } }

        public IStudentCustomAlertDetailService StudentCustomAlertDetailService { get { throw new NotImplementedException(); } }
        public IPanoramaSettingsService PanoramaSettingsService { get {throw new NotImplementedException();} }
        public IStandardizedTestService StandardizedTestService { get {throw new NotImplementedException();} }

        public ISupplementalAnnouncementService SupplementalAnnouncementService { get { throw new NotImplementedException(); } }
        public IEthnicityService EthnicityService { get {throw new NotImplementedException();} }
        public ILanguageService LanguageService { get {throw new NotImplementedException();} }
        public ICountryService CountryService { get { throw new NotImplementedException(); } }
        public IAnnouncementCommentService AnnouncementCommentService { get { throw new NotImplementedException(); } }
        public IAppSettingService AppSettingService { get { throw new NotImplementedException(); } }
    }
}