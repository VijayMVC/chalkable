﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using ClassroomOption = Chalkable.Data.School.Model.ClassroomOption;
using Infraction = Chalkable.Data.School.Model.Infraction;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStorage
    {
        public DemoPrivateMessageStorage PrivateMessageStore { get; private set; }
        public DemoSchoolYearStorage SchoolYearStorage { get; private set; }
        public DemoStudentSchoolYearStorage StudentSchoolYearStorage { get; private set; }
        public DemoAddressStorage AddressStorage { get; private set; }
        public DemoAlphaGradeStorage AlphaGradeStorage { get; private set; }
        public DemoAlternateScoreStorage AlternateScoreStorage { get; private set; }
        public DemoStudentParentStorage StudentParentStorage { get; private set; }
        public DemoStandardStorage StandardStorage { get; private set; }
        public DemoStandardSubjectStorage StandardSubjectStorage { get; private set; }
        public DemoClassStandardStorage ClasStandardStorage { get; private set; }
        public DemoSchoolStorage SchoolStorage { get; private set; }
        public DemoSchoolPersonStorage SchoolPersonStorage { get; private set; }
        public DemoRoomStorage RoomStorage { get; private set; }
        public DemoClassPeriodStorage ClassPeriodStorage { get; private set; }
        public DemoPrivateMessageStorage PrivateMessageStorage { get; private set; }
        public DemoPhoneStorage PhoneStorage { get; private set; }
        public DemoPersonStorage PersonStorage { get; private set; }
        public DemoPeriodStorage PeriodStorage { get; private set; }
        public DemoMarkingPeriodStorage MarkingPeriodStorage { get; private set; }
        public DemoMarkingPeriodClassStorage MarkingPeriodClassStorage { get; private set; }
        public DemoGradingPeriodStorage GradingPeriodStorage { get; private set; }
        public DemoGradeLevelStorage GradeLevelStorage { get; private set; }
        public DemoSchoolGradeLevelStorage SchoolGradeLevelStorage { get; private set; }
        public DemoDayTypeStorage DayTypeStorage { get; private set; }
        public DemoClassStorage ClassStorage { get; private set; }
        public DemoClassPersonStorage ClassPersonStorage { get; private set; }
        public DemoClassTeacherStorage ClassTeacherStorage { get; private set; }
        public DemoClassAnnouncementTypeStorage ClassAnnouncementTypeStorage { get; private set; }
        public DemoCategoryStorage CategoryStorage { get; private set; }
        public DemoDateStorage DateStorage { get; private set; }
        public DemoChalkableDepartmentStorage ChalkableDepartmentStorage { get; private set; }
        public DemoMasterSchoolStorage MasterSchoolStorage { get; private set; }
        public DemoStudentAnnouncementStorage StudentAnnouncementStorage { get; private set; }
        public DemoAnnouncementAttachmentStorage AnnouncementAttachmentStorage { get; private set; }
        public DemoAttendanceReasonStorage AttendanceReasonStorage { get; private set; }
        public DemoAttendanceLevelReasonStorage AttendanceLevelReasonStorage { get; private set; }
        public DemoApplicationRatingStorage ApplicationRatingStorage { get; private set; }
        public DemoNotificationStorage NotificationStorage { get; private set; }
        public DemoApplicationInstallStorage ApplicationInstallStorage { get; private set; }
        public IDemoAnnouncementStorage AnnouncementStorage { get; private set; }
        public DemoAnnouncementQnAStorage AnnouncementQnAStorage { get; private set; }
        public DemoAnnouncementRecipientStorage AnnouncementRecipientStorage { get; private set; }
        public DemoAnnouncementStandardStorage AnnouncementStandardStorage { get; private set; }
        public DemoDistrictStorage DistrictStorage { get; private set; }
        public DemoApplicationInstallActionStorage ApplicationInstallActionStorage { get; private set; }
        public DemoApplicationInstallActionClassesStorage ApplicationInstallActionClassesStorage { get; private set; }

        public DemoApplicationInstallActionGradeLevelStorage ApplicationInstallActionGradeLevelStorage { get;
            private set; }

        public DemoApplicationInstallActionDepartmentStorage ApplicationInstallActionDepartmentStorage { get;
            private set; }

        public DemoApplicationInstallActionRoleStorage ApplicationInstallActionRoleStorage { get; private set; }
        public DemoAnnouncementApplicationStorage AnnouncementApplicationStorage { get; private set; }
        public DemoInfractionStorage InfractionStorage { get; private set; }
        public DemoBlobStorage BlobStorage { get; private set; }
        public DemoGradingScaleStorage GradingScaleStorage { get; private set; }
        public DemoGradingScaleRangeStorage GradingScaleRangeStorage { get; private set; }
        public DemoClassRoomOptionStorage ClassRoomOptionStorage { get; private set; }
        public DemoSchoolOptionStorage SchoolOptionStorage { get; private set; }
        public DemoAnnouncementCompleteStorage AnnouncementCompleteStorage { get; private set; }

        public DemoStiDisciplineStorage StiDisciplineStorage { get; private set; }
        public DemoStiStandardScoreStorage StiStandardScoreStorage { get; private set; }
        public DemoStiGradeBookStorage StiGradeBookStorage { get; private set; }
        public DemoStiAttendanceStorage StiAttendanceStorage { get; private set; }
        public DemoStiSeatingChartStorage StiSeatingChartStorage { get; private set; }
        public DemoStiActivityScoreStorage StiActivityScoreStorage { get; private set; }
        public DemoStiInfractionStorage StiInfractionStorage { get; private set; }
        public DemoStiActivityStorage StiActivityStorage { get; private set; }

        public UserContext Context { get; private set; }

        public void UpdateContext(UserContext context)
        {
            Context = context;
        }

        public void UpdateAnnouncementStorage(IDemoAnnouncementStorage storage)
        {
            AnnouncementStorage = storage;
        }

        public static IDemoAnnouncementStorage CreateAnnouncementStorage(UserContext context, DemoStorage storage)
        {
            if (BaseSecurity.IsAdminViewer(context))
                return new DemoAnnouncementForAdminStorage(storage);
            if (context.Role == CoreRoles.TEACHER_ROLE)
                return new DemoAnnouncementForTeacherStorage(storage);
            if (context.Role == CoreRoles.STUDENT_ROLE)
                return new DemoAnnouncementForStudentStorage(storage);
            throw new ChalkableException("Unsupported role for announcements");
        }

        public DemoStorage(UserContext context)
        {
            Context = context;
            PrivateMessageStore = new DemoPrivateMessageStorage(this);
            SchoolYearStorage = new DemoSchoolYearStorage(this);
            InfractionStorage = new DemoInfractionStorage(this);
            AddressStorage = new DemoAddressStorage(this);
            AlphaGradeStorage = new DemoAlphaGradeStorage(this);
            AlternateScoreStorage = new DemoAlternateScoreStorage(this);
            StudentParentStorage = new DemoStudentParentStorage(this);
            StandardStorage = new DemoStandardStorage(this);
            StandardSubjectStorage = new DemoStandardSubjectStorage(this);
            ClasStandardStorage = new DemoClassStandardStorage(this);
            SchoolStorage = new DemoSchoolStorage(this);
            SchoolPersonStorage = new DemoSchoolPersonStorage(this);
            RoomStorage = new DemoRoomStorage(this);
            ClassPeriodStorage = new DemoClassPeriodStorage(this);
            PrivateMessageStorage = new DemoPrivateMessageStorage(this);
            PhoneStorage = new DemoPhoneStorage(this);
            PersonStorage = new DemoPersonStorage(this);
            PeriodStorage = new DemoPeriodStorage(this);
            MarkingPeriodStorage = new DemoMarkingPeriodStorage(this);
            MarkingPeriodClassStorage = new DemoMarkingPeriodClassStorage(this);
            GradingPeriodStorage = new DemoGradingPeriodStorage(this);
            GradeLevelStorage = new DemoGradeLevelStorage(this);
            SchoolGradeLevelStorage = new DemoSchoolGradeLevelStorage(this);
            DayTypeStorage = new DemoDayTypeStorage(this);
            ClassStorage = new DemoClassStorage(this);
            ClassPersonStorage = new DemoClassPersonStorage(this);
            ClassTeacherStorage = new DemoClassTeacherStorage(this);
            ClassAnnouncementTypeStorage = new DemoClassAnnouncementTypeStorage(this);
            CategoryStorage = new DemoCategoryStorage(this);
            DateStorage = new DemoDateStorage(this);
            ChalkableDepartmentStorage = new DemoChalkableDepartmentStorage(this);
            MasterSchoolStorage = new DemoMasterSchoolStorage(this);
            StudentAnnouncementStorage = new DemoStudentAnnouncementStorage(this);
            AnnouncementAttachmentStorage = new DemoAnnouncementAttachmentStorage(this);
            AttendanceReasonStorage = new DemoAttendanceReasonStorage(this);
            AttendanceLevelReasonStorage = new DemoAttendanceLevelReasonStorage(this);
            ApplicationRatingStorage = new DemoApplicationRatingStorage(this);
            NotificationStorage = new DemoNotificationStorage(this);
            ApplicationInstallStorage = new DemoApplicationInstallStorage(this);
            AnnouncementQnAStorage = new DemoAnnouncementQnAStorage(this);
            AnnouncementRecipientStorage = new DemoAnnouncementRecipientStorage(this);
            AnnouncementStandardStorage = new DemoAnnouncementStandardStorage(this);
            DistrictStorage = new DemoDistrictStorage(this);
            ApplicationInstallActionStorage = new DemoApplicationInstallActionStorage(this);
            ApplicationInstallActionClassesStorage = new DemoApplicationInstallActionClassesStorage(this);
            ApplicationInstallActionGradeLevelStorage = new DemoApplicationInstallActionGradeLevelStorage(this);
            ApplicationInstallActionDepartmentStorage = new DemoApplicationInstallActionDepartmentStorage(this);
            ApplicationInstallActionRoleStorage = new DemoApplicationInstallActionRoleStorage(this);
            StudentSchoolYearStorage = new DemoStudentSchoolYearStorage(this);
            AnnouncementApplicationStorage = new DemoAnnouncementApplicationStorage(this);
            BlobStorage = new DemoBlobStorage(this);
            ClassRoomOptionStorage = new DemoClassRoomOptionStorage(this);
            SchoolOptionStorage = new DemoSchoolOptionStorage(this);
            AnnouncementCompleteStorage = new DemoAnnouncementCompleteStorage(this);

            GradingScaleRangeStorage = new DemoGradingScaleRangeStorage(this);
            GradingScaleStorage = new DemoGradingScaleStorage(this);
            StiDisciplineStorage = new DemoStiDisciplineStorage(this);
            StiStandardScoreStorage = new DemoStiStandardScoreStorage(this);
            StiGradeBookStorage = new DemoStiGradeBookStorage(this);
            StiAttendanceStorage = new DemoStiAttendanceStorage(this);
            StiSeatingChartStorage = new DemoStiSeatingChartStorage(this);
            StiActivityScoreStorage = new DemoStiActivityScoreStorage(this);
            StiInfractionStorage = new DemoStiInfractionStorage(this);
            StiActivityStorage = new DemoStiActivityStorage(this);

            AnnouncementStorage = CreateAnnouncementStorage(Context, this);
            Setup();
        }

        private void Setup()
        {
            PrepareGeneralData();
            AddClasses();
            AddAdmin();
            AddTeacher();
            PrepareStudents();

            AddAttendances();

            AnnouncementStorage.Setup();
            StiStandardScoreStorage.Setup();
            StiSeatingChartStorage.Setup();

            
        }

        private void AddAttendances()
        {
            var gp = GradingPeriodStorage.GetGradingPeriodsDetails(new GradingPeriodQuery()
            {
                SchoolYearId = DemoSchoolConstants.CurrentSchoolYearId,
                FromDate = Context.NowSchoolTime.Date
            }).FirstOrDefault();

            if (gp == null) return;
            for (var classId = DemoSchoolConstants.AlgebraClassId; classId <= DemoSchoolConstants.PreCalculusClassId; ++classId)
                StiAttendanceStorage.GenerateSectionAttendanceForClass(classId, gp.StartDate, gp.EndDate);
        }

        private void AddGradeBookForClass(int classId, int gradingPeriodId)
        {
            var studentAverages = ClassPersonStorage.GetClassPersons(new ClassPersonQuery()
            {
                ClassId = classId
            }).Select(x => x.PersonRef).Distinct().Select(x => new StudentAverage()
            {
                CalculatedNumericAverage = 100,
                EnteredNumericAverage = 100,
                IsGradingPeriodAverage = true,
                GradingPeriodId = gradingPeriodId,
                StudentId = x
            });
            StiGradeBookStorage.Add(new Gradebook()
            {
                SectionId = classId,
                Activities = new List<Activity>(),
                Options = new Chalkable.StiConnector.Connectors.Model.ClassroomOption(),
                Scores = new List<Score>(),
                StudentAverages = studentAverages
            });
        }


        private void AddClasses()
        {
            AddClass(DemoSchoolConstants.AlgebraClassId, "Algebra", DemoSchoolConstants.GradeLevel12,
                DemoSchoolConstants.FirstMarkingPeriodId);
            AddClass(DemoSchoolConstants.AlgebraClassId, "Algebra", DemoSchoolConstants.GradeLevel12,
                DemoSchoolConstants.SecondMarkingPeriodId);

            AddClass(DemoSchoolConstants.GeometryClassId, "Geometry", DemoSchoolConstants.GradeLevel12,
                DemoSchoolConstants.FirstMarkingPeriodId);
            AddClass(DemoSchoolConstants.GeometryClassId, "Geometry", DemoSchoolConstants.GradeLevel12,
                DemoSchoolConstants.SecondMarkingPeriodId);

            AddClass(DemoSchoolConstants.PhysicsClassId, "Physics", DemoSchoolConstants.GradeLevel12,
                DemoSchoolConstants.FirstMarkingPeriodId);
            AddClass(DemoSchoolConstants.PhysicsClassId, "Physics", DemoSchoolConstants.GradeLevel12,
                DemoSchoolConstants.SecondMarkingPeriodId);

            AddClass(DemoSchoolConstants.PreCalculusClassId, "Pre-Calculus", DemoSchoolConstants.GradeLevel12,
                DemoSchoolConstants.FirstMarkingPeriodId);
            AddClass(DemoSchoolConstants.PreCalculusClassId, "Pre-Calculus", DemoSchoolConstants.GradeLevel12,
                DemoSchoolConstants.SecondMarkingPeriodId);
        }

        private string BuildDemoEmail(int studentId, string districtId)
        {
            return "demo-user_" + studentId.ToString() + districtId + "@chalkable.com";
        }

        private void PrepareStudents()
        {
            if (!Context.DistrictId.HasValue) throw new Exception("District id is null");

            var districtRef = Context.DistrictId.Value.ToString();
            AddStudent(DemoSchoolConstants.Student1, PreferenceService.Get("demoschool" + CoreRoles.STUDENT_ROLE.LoweredName).Value,
                "KAYE", "BURGESS", "F",
                DemoSchoolConstants.GradeLevel12, new DateTime(1998, 11, 27));
            AddStudentAddress(DemoSchoolConstants.Student1, new Address());

            AddStudent(DemoSchoolConstants.Student2, BuildDemoEmail(DemoSchoolConstants.Student2, districtRef), 
                "BRYON", "BOWEN", "M",
                DemoSchoolConstants.GradeLevel12, new DateTime(1998, 1, 23));
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

            AddStudent(DemoSchoolConstants.Student3,
                BuildDemoEmail(DemoSchoolConstants.Student3, districtRef),
                "ADRIAN", "BEAN", "F",
                DemoSchoolConstants.GradeLevel12, new DateTime(1998, 3, 10));
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

            AddStudent(DemoSchoolConstants.Student4,
                BuildDemoEmail(DemoSchoolConstants.Student4, districtRef),
                "COLLEEN", "HOLDEN", "F",
                DemoSchoolConstants.GradeLevel12, new DateTime(1998, 9, 17));
            AddStudentAddress(DemoSchoolConstants.Student4, new Address());

            AddStudent(DemoSchoolConstants.Student5,
                BuildDemoEmail(DemoSchoolConstants.Student5, districtRef),
                "INGRID", "LOWERY", "F",
                DemoSchoolConstants.GradeLevel12, new DateTime(1998, 2, 12));
            AddStudentAddress(DemoSchoolConstants.Student5, new Address());

            AddStudent(DemoSchoolConstants.Student6,
                BuildDemoEmail(DemoSchoolConstants.Student6, districtRef),
                "LUCIA", "SNYDER", "F",
                DemoSchoolConstants.GradeLevel12, new DateTime(1998, 8, 12));
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

            AddStudent(DemoSchoolConstants.Student7,
                BuildDemoEmail(DemoSchoolConstants.Student7, districtRef),
                "BYRON", "BYERS", "M",
                DemoSchoolConstants.GradeLevel12, new DateTime(1998, 12, 14));
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

            AddStudent(DemoSchoolConstants.Student8,
                BuildDemoEmail(DemoSchoolConstants.Student8, districtRef),
                "NOEL", "BOYD", "M",
                DemoSchoolConstants.GradeLevel12, new DateTime(1998, 12, 14));
            AddStudentAddress(DemoSchoolConstants.Student8, new Address());

            AddStudent(DemoSchoolConstants.Student9,
               BuildDemoEmail(DemoSchoolConstants.Student9, districtRef),
               "JAMEL", "HARRIS", "M",
               DemoSchoolConstants.GradeLevel12, new DateTime(1998, 4, 4));
            AddStudentAddress(DemoSchoolConstants.Student9, new Address());

            AddStudent(DemoSchoolConstants.Student10,
              BuildDemoEmail(DemoSchoolConstants.Student10, districtRef),
              "MOLLIE", "PAUL", "F",
              DemoSchoolConstants.GradeLevel12, new DateTime(1998, 4, 4));
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

            AddStudentsToClasses();

            

            
        }

        private void AddStudentAddress(int studentId, Address address)
        {
            address.Id = studentId;
            AddressStorage.Add(address);
        }

        private void AddStudentsToClasses()
        {
            var studentIds = PersonStorage.GetPersons(new PersonQuery()
            {
                RoleId = CoreRoles.STUDENT_ROLE.Id
            }).Persons.Select(x => x.Id);

            for (var cls = DemoSchoolConstants.AlgebraClassId; cls <= DemoSchoolConstants.PreCalculusClassId; ++cls)
            {
                foreach (var studentId in studentIds)
                {
                    AddStudentToClass(studentId, cls, DemoSchoolConstants.FirstMarkingPeriodId);
                    AddStudentToClass(studentId, cls, DemoSchoolConstants.SecondMarkingPeriodId);    
                }
            }
        }

        private void AddAdmin()
        {
            PersonStorage.Add(new Person
            {
                Active = true,
                Salutation = "Mr.",
                Email = PreferenceService.Get("demoschool" + CoreRoles.ADMIN_GRADE_ROLE.LoweredName).Value,
                Id = DemoSchoolConstants.AdminGradeId,
                FirstName = "rosteradmin",
                LastName = "rosteradmin",
                Gender = null,
                RoleRef = CoreRoles.ADMIN_GRADE_ROLE.Id
            });
            SchoolPersonStorage.Add(new SchoolPerson
            {
                PersonRef = DemoSchoolConstants.AdminGradeId,
                RoleRef = CoreRoles.ADMIN_GRADE_ROLE.Id,
                SchoolRef = DemoSchoolConstants.SchoolId
            });
        }

        private void AddTeacher()
        {
            PersonStorage.Add(new Person
            {
                Active = true,
                Email = PreferenceService.Get("demoschool" + CoreRoles.TEACHER_ROLE.LoweredName).Value,
                Gender = "M",
                FirstName = "ROCKY",
                LastName = "STEIN",
                Id = DemoSchoolConstants.TeacherId,
                RoleRef = CoreRoles.TEACHER_ROLE.Id
            });

            SchoolPersonStorage.Add(new SchoolPerson
            {
                PersonRef = DemoSchoolConstants.TeacherId,
                RoleRef = CoreRoles.TEACHER_ROLE.Id,
                SchoolRef = DemoSchoolConstants.SchoolId
            });
        }

        private void AddStudent(int id, string email, string firstName, string lastName, string gender, int gradeLevelRef, DateTime birthDate)
        {
            PersonStorage.Add(new Person
            {
                BirthDate = birthDate,
                Active = true,
                Email = email,
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                RoleRef = CoreRoles.STUDENT_ROLE.Id,
                AddressRef = id
            });


            SchoolPersonStorage.Add(new SchoolPerson
            {
                PersonRef = id,
                RoleRef = CoreRoles.STUDENT_ROLE.Id,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            StudentSchoolYearStorage.Add(new StudentSchoolYear
            {
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                StudentRef = id,
                GradeLevel = GradeLevelStorage.GetById(gradeLevelRef),
                GradeLevelRef = gradeLevelRef
            });

            StiDisciplineStorage.Create(new DisciplineReferral
            {
                Date = DateTime.Now.Date,
                Infractions = StiInfractionStorage.GetAll(),
                StudentId = id
            });
        }

        private void AddStudentToClass(int studentId, int classId, int markingPeriodId)
        {
            ClassPersonStorage.Add(new ClassPerson
            {
                ClassRef = classId,
                MarkingPeriodRef = markingPeriodId,
                PersonRef = studentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });
        }

        private void PrepareGeneralData()
        {
            MasterSchoolStorage.AddMasterSchool();
            DateStorage.AddDates();
            DistrictStorage.AddDistrict();
            AddPeriods();
            AddMarkingPeriods();
            AddDayTypes();
            AddGradingPeriods();
            AddSchool();
            AddSchoolYear();
            AddStandards();
            AddStandardSubjects();
            AddAlphaGrades();
            AddGradeLevels();
            AddGradingScales();
            AddGradingScaleRanges();
            AddAttendanceReasons();
            AddAttendanceLevelReasons();
            AddInfractions();
        }

        private void AddMarkingPeriods()
        {
            var currentYear = DateTime.Now.Year;

            MarkingPeriodStorage.Add(new MarkingPeriod
            {
                Id = DemoSchoolConstants.FirstMarkingPeriodId,
                Name = "Semester 1",
                Description = "",
                StartDate = new DateTime(currentYear, 1, 21),
                EndDate = new DateTime(currentYear, 5, 30),
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                WeekDays = 62
            });

            MarkingPeriodStorage.Add(new MarkingPeriod
            {
                Id = DemoSchoolConstants.SecondMarkingPeriodId,
                Name = "Semester 2",
                Description = "",
                StartDate = new DateTime(currentYear, 6, 30),
                EndDate = new DateTime(currentYear, 10, 30),
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                WeekDays = 62
            });
        }

        private void AddGradingScaleRanges()
        {
            var gdScaleRanges = new List<GradingScaleRange>()
            {
                new GradingScaleRange()
                {
                    GradingScaleRef = 1,
                    AlphaGradeRef = 6,
                    LowValue = 84.50m,
                    HighValue = 100.0m,
                    AveragingEquivalent = 100,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 1,
                    AlphaGradeRef = 7,
                    LowValue = 69.50m,
                    HighValue = 84.49999m,
                    AveragingEquivalent = 84,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 1,
                    AlphaGradeRef = 8,
                    LowValue = 0.0m,
                    HighValue = 69.499999m,
                    AveragingEquivalent = 69,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 1,
                    LowValue = 93.50m,
                    HighValue = 96.49999m,
                    AveragingEquivalent = 96,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 2,
                    LowValue = 83.50m,
                    HighValue = 86.49999m,
                    AveragingEquivalent = 86,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 3,
                    LowValue = 73.50m,
                    HighValue = 76.49999m,
                    AveragingEquivalent = 76,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 4,
                    LowValue = 63.50m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 66,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 4,
                    LowValue = 63.50m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 66,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 5,
                    LowValue = 0.0m,
                    HighValue = 60.49999m,
                    AveragingEquivalent = 60,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 9,
                    LowValue = 96.5m,
                    HighValue = 100.00m,
                    AveragingEquivalent = 100,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 10,
                    LowValue = 90.5m,
                    HighValue = 93.49999m,
                    AveragingEquivalent = 93,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 11,
                    LowValue = 86.5m,
                    HighValue = 90.49999m,
                    AveragingEquivalent = 90,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 12,
                    LowValue = 80.5m,
                    HighValue = 83.49999m,
                    AveragingEquivalent = 83,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 13,
                    LowValue = 76.5m,
                    HighValue = 80.49999m,
                    AveragingEquivalent = 80,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 14,
                    LowValue = 70.5m,
                    HighValue = 73.49999m,
                    AveragingEquivalent = 73,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 15,
                    LowValue = 66.5m,
                    HighValue = 70.49999m,
                    AveragingEquivalent = 70,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 16,
                    LowValue = 60.5m,
                    HighValue = 63.49999m,
                    AveragingEquivalent = 63,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 1,
                    LowValue = 93.0m,
                    HighValue = 97.49999m,
                    AveragingEquivalent = 95,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 2,
                    LowValue = 83.0m,
                    HighValue = 86.49999m,
                    AveragingEquivalent = 85,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 3,
                    LowValue = 73.0m,
                    HighValue = 76.49999m,
                    AveragingEquivalent = 75,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 4,
                    LowValue = 63.0m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 65,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 5,
                    LowValue = 1.0m,
                    HighValue = 59.49999m,
                    AveragingEquivalent = 50,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 6,
                    LowValue = 0.05m,
                    HighValue = 0.06m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 7,
                    LowValue = 0.03m,
                    HighValue = 0.04m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 8,
                    LowValue = 0.00m,
                    HighValue = 0.02m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 9,
                    LowValue = 98.00m,
                    HighValue = 110.00m,
                    AveragingEquivalent = 100,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 10,
                    LowValue = 90.00m,
                    HighValue = 92.9999m,
                    AveragingEquivalent = 91,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 11,
                    LowValue = 87.00m,
                    HighValue = 89.9999m,
                    AveragingEquivalent = 88,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 12,
                    LowValue = 80.00m,
                    HighValue = 82.9999m,
                    AveragingEquivalent = 81,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 13,
                    LowValue = 77.00m,
                    HighValue = 79.9999m,
                    AveragingEquivalent = 78,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 14,
                    LowValue = 70.00m,
                    HighValue = 72.9999m,
                    AveragingEquivalent = 71,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 15,
                    LowValue = 67.00m,
                    HighValue = 69.9999m,
                    AveragingEquivalent = 68,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 16,
                    LowValue = 60.00m,
                    HighValue = 62.9999m,
                    AveragingEquivalent = 61,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 17,
                    LowValue = 00.70m,
                    HighValue = 00.8m,
                    AveragingEquivalent = 61,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 19,
                    LowValue = 00.90m,
                    HighValue = 0.99999m,
                    AveragingEquivalent = 50,
                }
            };
            GradingScaleRangeStorage.Add(gdScaleRanges);
        }

        private void AddGradingScales()
        {
            GradingScaleStorage.Add(new List<GradingScale>
            {
                new GradingScale()
                {
                    Name = "K-3 Grade Scale",
                    SchoolRef = DemoSchoolConstants.SchoolId
                },
                new GradingScale()
                {
                    Name = "Grades 4-5",
                    SchoolRef = DemoSchoolConstants.SchoolId
                },
                new GradingScale()
                {
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

            GradingPeriodStorage.Add(gpList);
        }

        private void AddGradeLevels()
        {
            for (var lvl = DemoSchoolConstants.GradeLevel1; lvl <= DemoSchoolConstants.GradeLevel12; ++lvl)
            {
                GradeLevelStorage.Add(new GradeLevel
                {
                    Description = "",
                    Id = lvl,
                    Name = lvl.ToString(CultureInfo.InvariantCulture),
                    Number = lvl
                });
            }
        }

        private void AddDayTypes()
        {
            DayTypeStorage.Add(new DayType
            {
                Id = 1,
                Name = "M",
                Number = 0,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            DayTypeStorage.Add(new DayType
            {
                Id = 2,
                Name = "TTh",
                Number = 1,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            DayTypeStorage.Add(new DayType
            {
                Id = 3,
                Name = "WF",
                Number = 2,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });
        }

        private void AddAttendanceLevelReasons()
        {
            var attendanceLevelReasons = new List<AttendanceLevelReason>
            {
                new AttendanceLevelReason
                {
                    Level = "AO",
                    AttendanceReasonRef = 6,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "AO",
                    AttendanceReasonRef = 2,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "HO",
                    AttendanceReasonRef = 6,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "HO",
                    AttendanceReasonRef = 2,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 1,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 8,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 1,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 10,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 1,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 10,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 12,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 1,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 10,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                }
            };

            AttendanceLevelReasonStorage.Add(attendanceLevelReasons);
        }

        private void AddAttendanceReasons()
        {
            var reasons = new List<AttendanceReason>
            {
                new AttendanceReason
                {
                    Id = 1,
                    Code = "IL",
                    Name = "Illness",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(1)
                },
                new AttendanceReason
                {
                    Id = 2,
                    Code = "SA",
                    Name = "School Activity",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(2)
                },
                new AttendanceReason
                {
                    Id = 3,
                    Code = "FRE",
                    Name = "Family Reason(Excused)",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(3)
                },
                new AttendanceReason
                {
                    Id = 4,
                    Code = "FRU",
                    Name = "Family Reason(Unexcused)",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(4)
                },
                new AttendanceReason
                {
                    Id = 5,
                    Code = "OSS",
                    Name = "Out of School Suspension",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(5)
                },
                new AttendanceReason
                {
                    Id = 6,
                    Code = "ISS",
                    Name = "In-School Suspension",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(6)
                },
                new AttendanceReason
                {
                    Id = 7,
                    Code = "SK",
                    Name = "Skipping",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(7)
                },
                new AttendanceReason
                {
                    Id = 8,
                    Code = "DD",
                    Name = "Doctor or Dentist",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(8)
                },
                new AttendanceReason
                {
                    Id = 9,
                    Code = "IM",
                    Name = "Noncompliance",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(9)
                },
                new AttendanceReason
                {
                    Id = 10,
                    Code = "MB",
                    Name = "Missed Bus",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(10)
                },
                new AttendanceReason
                {
                    Id = 11,
                    Code = "LB",
                    Name = "Late Bus",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = false,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(11)
                },
                new AttendanceReason
                {
                    Id = 12,
                    Code = "EC",
                    Name = "Early Checkout",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = AttendanceLevelReasonStorage.GetForAttendanceReason(12)
                }
            };

            AttendanceReasonStorage.Add(reasons);
        }

        private void AddPeriods()
        {
            PeriodStorage.Add(new Period
            {
                StartTime = 615,
                EndTime = 659,
                Order = 1,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            PeriodStorage.Add(new Period
            {
                StartTime = 663,
                EndTime = 707,
                Order = 2,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            PeriodStorage.Add(new Period
            {
                StartTime = 710,
                EndTime = 740,
                Order = 3,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            PeriodStorage.Add(new Period
            {
                StartTime = 744,
                EndTime = 783,
                Order = 4,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            PeriodStorage.Add(new Period
            {
                StartTime = 787,
                EndTime = 826,
                Order = 5,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            PeriodStorage.Add(new Period
            {
                StartTime = 830,
                EndTime = 869,
                Order = 6,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            PeriodStorage.Add(new Period
            {
                StartTime = 873,
                EndTime = 912,
                Order = 7,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            PeriodStorage.Add(new Period
            {
                StartTime = 916,
                EndTime = 955,
                Order = 8,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            PeriodStorage.Add(new Period
            {
                StartTime = 959,
                EndTime = 1000,
                Order = 9,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });
        }

        private void AddSchool()
        {
            SchoolStorage.Add(new Data.School.Model.School
            {
                Id = DemoSchoolConstants.SchoolId,
                IsActive = true,
                IsPrivate = true,
                Name = "SMITH"
            });

            SchoolOptionStorage.Add(new SchoolOption
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
            });
        }

        private void AddAlphaGrades()
        {
            AlphaGradeStorage.Add(new List<AlphaGrade>()
            {
                new AlphaGrade()
                {
                    Id = 1,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "A"
                },

                new AlphaGrade()
                {
                    Id = 2,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "B"
                },

                new AlphaGrade()
                {
                    Id = 3,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "C"
                },

                new AlphaGrade()
                {
                    Id = 4,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "D"
                },

                new AlphaGrade()
                {
                    Id = 5,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "F"
                },

                new AlphaGrade()
                {
                    Id = 6,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "E"
                },

                new AlphaGrade()
                {
                    Id = 7,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "S"
                },

                new AlphaGrade()
                {
                    Id = 8,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "N",
                    Description = "No pass"

                },

                new AlphaGrade()
                {
                    Id = 9,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "A+"
                },

                new AlphaGrade()
                {
                    Id = 10,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "A-"
                },

                new AlphaGrade()
                {
                    Id = 11,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "B+"
                },

                new AlphaGrade()
                {
                    Id = 12,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "B-"
                },

                new AlphaGrade()
                {
                    Id = 13,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "C+"
                },

                new AlphaGrade()
                {
                    Id = 14,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "C-"
                },

                new AlphaGrade()
                {
                    Id = 15,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "D+"
                },

                new AlphaGrade()
                {
                    Id = 16,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "D-"
                },

                new AlphaGrade()
                {
                    Id = 17,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "P",
                    Description = "Pass"
                },

                new AlphaGrade()
                {
                    Id = 18,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "Audit",
                },

                new AlphaGrade()
                {
                    Id = 19,
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    Name = "I",
                    Description = "Incomplete"
                }

            });
        }

        private void AddSchoolYear()
        {
            var currentDate = DateTime.Now;
            SchoolYearStorage.Add(DemoSchoolConstants.CurrentSchoolYearId, DemoSchoolConstants.SchoolId,
                "Current School Year", "",
                new DateTime(currentDate.Year, 1, 1),
                new DateTime(currentDate.Year, 12, 31));
        }

        private void AddStandards()
        {
            StandardStorage.Add(new List<Standard>
            {
                new Standard
                {
                    Id = DemoSchoolConstants.MathStandard1,
                    IsActive = true,
                    Name = "Math 1",
                    LowerGradeLevelRef = DemoSchoolConstants.GradeLevel10,
                    UpperGradeLevelRef = DemoSchoolConstants.GradeLevel10,
                    StandardSubjectRef = DemoSchoolConstants.ScienceStandardSubject
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
            StandardSubjectStorage.Add(new List<StandardSubject>
            {
                new StandardSubject()
                {
                    Id = 1,
                    AdoptionYear = 2010,
                    Name = "Math Standards",
                    Description = "",
                    IsActive = true
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
            var infractions = new List<Infraction>();

            var infr1 = new Infraction
            {
                Code = "FI",
                Demerits = 0,
                Description = "Fighting",
                IsActive = true,
                IsSystem = false,
                Name = "Fighting"
            };

            var infr2 = new Infraction
            {
                Code = "DI",
                Demerits = 0,
                Description = "Disrespect",
                IsActive = true,
                IsSystem = false,
                Name = "Disrespect"
            };

            infractions.Add(infr1);
            infractions.Add(infr2);

            InfractionStorage.Add(infractions);
        }

        private void AddClass(int id, string name, int gradeLevelRef, int markingPeriodRef)
        {
            ClassStorage.Add(new Class
            {
                Id = id,
                Name = name,
                Description = name,
                GradeLevelRef = gradeLevelRef,
                ChalkableDepartmentRef = null,
                PrimaryTeacherRef = DemoSchoolConstants.TeacherId,
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });


            MarkingPeriodClassStorage.Add(new MarkingPeriodClass
            {
                SchoolRef = DemoSchoolConstants.SchoolId,
                MarkingPeriodRef = markingPeriodRef,
                ClassRef = id
            });


            ClassAnnouncementTypeStorage.Add(new ClassAnnouncementType
            {
                ClassRef = id,
                Description = "Academic Achievement",
                Gradable = true,
                Id = DemoSchoolConstants.AlgebraAcademicAchievementId,
                Name = "Academic Achievement",
                Percentage = 50
            });

            ClassAnnouncementTypeStorage.Add(new ClassAnnouncementType
            {
                ClassRef = id,
                Description = "Academic Practice",
                Gradable = true,
                Id = DemoSchoolConstants.AlgebraAcademicPracticeId,
                Name = "Task",
                Percentage = 50
            });

            ClassPeriodStorage.Add(new ClassPeriod
            {
                ClassRef = id,
                DayTypeRef = 19,
                PeriodRef = DemoSchoolConstants.FirstPeriodId,
                SchoolRef = DemoSchoolConstants.SchoolId,
                Period = PeriodStorage.GetById(DemoSchoolConstants.FirstPeriodId)
            });

            ClassPeriodStorage.Add(new ClassPeriod
            {
                ClassRef = id,
                DayTypeRef = 20,
                PeriodRef = DemoSchoolConstants.SecondPeriodId,
                SchoolRef = DemoSchoolConstants.SchoolId,
                Period = PeriodStorage.GetById(DemoSchoolConstants.SecondPeriodId)
            });

            ClassTeacherStorage.Add(new ClassTeacher
            {
                ClassRef = id,
                PersonRef = DemoSchoolConstants.TeacherId,
                IsPrimary = true
            });


            ClassRoomOptionStorage.Add(new ClassroomOption()
            {
                Id = id,
                SeatingChartColumns = 3,
                SeatingChartRows = 3,
                AveragingMethod = "P",
                DefaultActivitySortOrder = "D",
                StandardsCalculationMethod = "A",
                StandardsCalculationRule = "G",
                DisplayStudentAverage = true
            });

            AddClassStandard(id, DemoSchoolConstants.MathStandard1);
            AddClassStandard(id, DemoSchoolConstants.MathStandard2);
            AddClassStandard(id, DemoSchoolConstants.MathStandard3);

            AddClassStandard(id, DemoSchoolConstants.MathStandard1);
            AddClassStandard(id, DemoSchoolConstants.MathStandard2);
            AddClassStandard(id, DemoSchoolConstants.MathStandard3);

            for(var gp = DemoSchoolConstants.GradingPeriodQ1; gp <= DemoSchoolConstants.GradingPeriodQ4; ++gp)
                AddGradeBookForClass(id, gp);

        }

        private void AddClassStandard(int classId, int standardId)
        {
            ClasStandardStorage.Add(new ClassStandard()
            {
                ClassRef = classId,
                StandardRef = standardId
            });
        }

    }
}
