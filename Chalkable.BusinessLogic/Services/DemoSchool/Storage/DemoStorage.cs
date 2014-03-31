using System;
using System.Diagnostics;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStorage
    {
        public DemoUserStorage UserStorage { get; private set; }
        public DemoPrivateMessageStorage PrivateMessageStore { get; private set; }
        public DemoSchoolYearStorage SchoolYearStorage { get; private set; }
        public DemoStudentSchoolYearStorage StudentSchoolYearStorage { get; private set; }
        public DemoDisciplineTypeStorage DisciplineTypeStorage { get; private set; }
        public DemoDisciplineStorage DisciplineStorage { get; private set; }
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
        public DemoPeriodStorage PeriodStorage{ get; private set; }
        public DemoMarkingPeriodStorage MarkingPeriodStorage { get; private set; }
        public DemoMarkingPeriodClassStorage MarkingPeriodClassStorage { get; private set; }
        public DemoGradingPeriodStorage GradingPeriodStorage { get; private set; }
        public DemoGradeLevelStorage GradeLevelStorage{ get; private set; }
        public DemoSchoolGradeLevelStorage SchoolGradeLevelStorage { get; private set; }
        public DemoDayTypeStorage DayTypeStorage { get; private set; }
        public DemoClassStorage ClassStorage { get; private set; }
        public DemoClassPersonStorage ClassPersonStorage { get; private set; }
        public DemoClassAnnouncementTypeStorage ClassAnnouncementTypeStorage{ get; private set; }
        public DemoCategoryStorage CategoryStorage { get; private set; }
        public DemoDateStorage DateStorage { get; private set; }
        public DemoChalkableDepartmentStorage  ChalkableDepartmentStorage { get; private set; }
        public DemoMasterSchoolStorage MasterSchoolStorage { get; private set; }
        public DemoStudentAnnouncementStorage StudentAnnouncementStorage { get; private set; }
        public DemoAnnouncementAttachmentStorage AnnouncementAttachmentStorage { get; private set; }
        public DemoAttendanceReasonStorage AttendanceReasonStorage { get; private set; }
        public DemoAttendanceLevelReasonStorage AttendanceLevelReasonStorage { get; private set; }
        public DemoApplicationStorage ApplicationStorage { get; private set; }
        public DemoApplicationRatingStorage ApplicationRatingStorage { get; private set; }
        public DemoNotificationStorage NotificationStorage{ get; private set; }
        public DemoApplicationInstallStorage ApplicationInstallStorage { get; private set; }
        public IDemoAnnouncementStorage AnnouncementStorage { get; private set; }


        private UserContext Context { get; set; }

        public DemoStorage(UserContext context)
        {
            Context = context;
            UserStorage = new DemoUserStorage(this);
            PrivateMessageStore = new DemoPrivateMessageStorage();
            SchoolYearStorage = new DemoSchoolYearStorage(this);
            DisciplineTypeStorage = new DemoDisciplineTypeStorage(this);
            DisciplineStorage = new DemoDisciplineStorage();
            AlphaGradeStorage = new DemoAlphaGradeStorage(this);
            AlternateScoreStorage = new DemoAlternateScoreStorage(this);
            StudentParentStorage = new DemoStudentParentStorage();
            StandardStorage = new DemoStandardStorage(this);
            StandardSubjectStorage = new DemoStandardSubjectStorage(this);
            ClasStandardStorage = new DemoClassStandardStorage(this);
            SchoolStorage = new DemoSchoolStorage(this);
            SchoolPersonStorage = new DemoSchoolPersonStorage();
            RoomStorage = new DemoRoomStorage(this);
            ClassPeriodStorage = new DemoClassPeriodStorage(this);
            PrivateMessageStorage = new DemoPrivateMessageStorage();
            PhoneStorage = new DemoPhoneStorage(this);
            PersonStorage = new DemoPersonStorage(this);
            PeriodStorage = new DemoPeriodStorage(this);
            MarkingPeriodStorage = new DemoMarkingPeriodStorage(this);
            MarkingPeriodClassStorage = new DemoMarkingPeriodClassStorage();
            GradingPeriodStorage = new DemoGradingPeriodStorage();
            GradeLevelStorage = new DemoGradeLevelStorage(this);
            SchoolGradeLevelStorage = new DemoSchoolGradeLevelStorage();
            DayTypeStorage = new DemoDayTypeStorage(this);
            ClassStorage = new DemoClassStorage(this);
            ClassPersonStorage = new DemoClassPersonStorage(this);
            ClassAnnouncementTypeStorage = new DemoClassAnnouncementTypeStorage(this);
            CategoryStorage = new DemoCategoryStorage(this);
            DateStorage = new DemoDateStorage();
            ChalkableDepartmentStorage = new DemoChalkableDepartmentStorage(this);
            MasterSchoolStorage = new DemoMasterSchoolStorage(this);
            StudentAnnouncementStorage = new DemoStudentAnnouncementStorage();
            AnnouncementAttachmentStorage = new DemoAnnouncementAttachmentStorage(this);
            AttendanceReasonStorage = new DemoAttendanceReasonStorage(this);
            AttendanceLevelReasonStorage = new DemoAttendanceLevelReasonStorage(this);
            ApplicationStorage = new DemoApplicationStorage(this);
            ApplicationRatingStorage = new DemoApplicationRatingStorage(this);
            NotificationStorage = new DemoNotificationStorage(this);
            ApplicationInstallStorage = new DemoApplicationInstallStorage(this);
            Setup();
        }

        private void Setup()
        {

            AttendanceReasonStorage.Setup();
            AttendanceLevelReasonStorage.Setup();
            GradeLevelStorage.Setup();
            ClassStorage.Setup();

            MasterSchoolStorage.Add(new Data.Master.Model.School
            {
                Id = Guid.NewGuid(),
                LocalId = 1,
                DistrictRef = Context.DistrictId.Value,
                Name = "SMITH",
            });

            SchoolStorage.Add(new Data.School.Model.School
            {
                Id = 1,
                IsActive = true,
                IsPrivate = true,
                Name = "SMITH"
            });

            MarkingPeriodStorage.Add(new MarkingPeriod
            {
                Id = 1,
                Name = "Semester 2",
                Description = "",
                StartDate = new DateTime(2014, 1, 21),
                EndDate = new DateTime(2014, 5, 30),
                SchoolRef = 1,
                SchoolYearRef = 12,
                WeekDays = 62
            });

            var currentDate = DateTime.Now;
            
            SchoolYearStorage.Add(1, 1, "Current School Year", "", new DateTime(currentDate.Year, 1, 1),
                new DateTime(currentDate.Year, 12, 31));
            //some init data

            //person storage

            PersonStorage.Add(new Person
            {
                BirthDate = null,
                Active = false,
                AddressRef = null,
                Email = "e96ef526fe974703bec2592d977b2115user1195_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com",
                Id = 1195,
                FirstName = "ROCKY",
                LastName = "STEIN",
                Gender = "F",
                RoleRef = 2
            });

            PersonStorage.Add(new Person
            {
                BirthDate = new DateTime(1998, 11, 27),
                Active = false,
                AddressRef = null,
                Email = "e96ef526fe974703bec2592d977b2115user19_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com",
                Id = 19,
                FirstName = "KAYE",
                LastName = "BURGESS",
                Gender = "F",
                RoleRef = 3
            });

            PersonStorage.Add(new Person
            {
                BirthDate = null,
                Active = true,
                AddressRef = null,
                Salutation = "Mr.",
                Email = "e96ef526fe974703bec2592d977b2115user2735_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com",
                Id = 2375,
                FirstName = "rosteradmin",
                LastName = "rosteradmin",
                Gender = null,
                RoleRef = 5
            });


            

        }
    }
    
}
