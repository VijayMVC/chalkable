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

        public DemoStorage()
        {
            UserStorage = new DemoUserStorage();
            PrivateMessageStore = new DemoPrivateMessageStorage();
            SchoolYearStorage = new DemoSchoolYearStorage();
            DisciplineTypeStorage = new DemoDisciplineTypeStorage();
            DisciplineStorage = new DemoDisciplineStorage();
            AlphaGradeStorage = new DemoAlphaGradeStorage();
            AlternateScoreStorage = new DemoAlternateScoreStorage();
            StudentParentStorage = new DemoStudentParentStorage();
            StandardStorage = new DemoStandardStorage();
            StandardSubjectStorage = new DemoStandardSubjectStorage();
            ClasStandardStorage = new DemoClassStandardStorage();
            SchoolStorage = new DemoSchoolStorage();
            SchoolPersonStorage = new DemoSchoolPersonStorage();
            RoomStorage = new DemoRoomStorage();
            ClassPeriodStorage = new DemoClassPeriodStorage();
            PrivateMessageStorage = new DemoPrivateMessageStorage();
            PhoneStorage = new DemoPhoneStorage();
            PersonStorage = new DemoPersonStorage();
            PeriodStorage = new DemoPeriodStorage();
            MarkingPeriodStorage = new DemoMarkingPeriodStorage();
            MarkingPeriodClassStorage = new DemoMarkingPeriodClassStorage();
            GradingPeriodStorage = new DemoGradingPeriodStorage();
            GradeLevelStorage = new DemoGradeLevelStorage();
            SchoolGradeLevelStorage = new DemoSchoolGradeLevelStorage();
            DayTypeStorage = new DemoDayTypeStorage();
            ClassStorage = new DemoClassStorage();
            ClassPersonStorage = new DemoClassPersonStorage();
            ClassAnnouncementTypeStorage = new DemoClassAnnouncementTypeStorage();
            CategoryStorage = new DemoCategoryStorage();
            DateStorage = new DemoDateStorage();
            ChalkableDepartmentStorage = new DemoChalkableDepartmentStorage();
            MasterSchoolStorage = new DemoMasterSchoolStorage();
            StudentAnnouncementStorage = new DemoStudentAnnouncementStorage();
            AnnouncementAttachmentStorage = new DemoAnnouncementAttachmentStorage();
            AttendanceReasonStorage = new DemoAttendanceReasonStorage();
            AttendanceLevelReasonStorage = new DemoAttendanceLevelReasonStorage();
            ApplicationStorage = new DemoApplicationStorage();
            ApplicationRatingStorage = new DemoApplicationRatingStorage();
            setup();
        }

        private void setup()
        {
            //some init data
        }
    }
}
