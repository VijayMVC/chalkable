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

        public DemoStorage()
        {
            UserStorage = new DemoUserStorage(this);
            PrivateMessageStore = new DemoPrivateMessageStorage();
            SchoolYearStorage = new DemoSchoolYearStorage();
            DisciplineTypeStorage = new DemoDisciplineTypeStorage(this);
            DisciplineStorage = new DemoDisciplineStorage(this);
            AlphaGradeStorage = new DemoAlphaGradeStorage(this);
            AlternateScoreStorage = new DemoAlternateScoreStorage(this);
            StudentParentStorage = new DemoStudentParentStorage(this);
            StandardStorage = new DemoStandardStorage();
            StandardSubjectStorage = new DemoStandardSubjectStorage();
            ClasStandardStorage = new DemoClassStandardStorage(this);
            SchoolStorage = new DemoSchoolStorage(this);
            SchoolPersonStorage = new DemoSchoolPersonStorage();
            RoomStorage = new DemoRoomStorage();
            ClassPeriodStorage = new DemoClassPeriodStorage(this);
            PrivateMessageStorage = new DemoPrivateMessageStorage();
            PhoneStorage = new DemoPhoneStorage(this);
            PersonStorage = new DemoPersonStorage();
            PeriodStorage = new DemoPeriodStorage();
            MarkingPeriodStorage = new DemoMarkingPeriodStorage();
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
            NotificationStorage = new DemoNotificationStorage();
            ApplicationInstallStorage = new DemoApplicationInstallStorage(this);
            setup();
        }

        private void setup()
        {
            //some init data
        }
    }
}
