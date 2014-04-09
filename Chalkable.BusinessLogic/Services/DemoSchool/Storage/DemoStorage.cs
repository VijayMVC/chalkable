using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStorage
    {
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
        public DemoApplicationRatingStorage ApplicationRatingStorage { get; private set; }
        public DemoNotificationStorage NotificationStorage{ get; private set; }
        public DemoApplicationInstallStorage ApplicationInstallStorage { get; private set; }
        public IDemoAnnouncementStorage AnnouncementStorage { get; private set; }
        public DemoAnnouncementQnAStorage AnnouncementQnAStorage { get; private set; }
        public DemoAnnouncementReminderStorage AnnouncementReminderStorage { get; private set; }
        public DemoAnnouncementRecipientStorage AnnouncementRecipientStorage { get; private set; }
        public DemoAnnouncementStandardStorage AnnouncementStandardStorage{ get; private set; }
        public DemoDistrictStorage DistrictStorage { get; private set; }
        public DemoApplicationInstallActionStorage ApplicationInstallActionStorage { get; private set; }
        public DemoApplicationInstallActionClassesStorage ApplicationInstallActionClassesStorage { get; private set; }
        public DemoApplicationInstallActionGradeLevelStorage ApplicationInstallActionGradeLevelStorage { get; private set; }
        public DemoApplicationInstallActionDepartmentStorage ApplicationInstallActionDepartmentStorage { get; private set; }
        public DemoApplicationInstallActionRoleStorage ApplicationInstallActionRoleStorage { get; private set; }

        public UserContext Context { get; private set; }

        private IDemoAnnouncementStorage CreateAnnouncementStorage(UserContext context, DemoStorage storage)
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
            AnnouncementReminderStorage = new DemoAnnouncementReminderStorage(this);
            AnnouncementStorage = CreateAnnouncementStorage(Context, this);
            PrivateMessageStore = new DemoPrivateMessageStorage(this);
            SchoolYearStorage = new DemoSchoolYearStorage(this);
            DisciplineTypeStorage = new DemoDisciplineTypeStorage(this);
            DisciplineStorage = new DemoDisciplineStorage();
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
            Setup();
        }

        private void Setup()
        {

            AttendanceReasonStorage.Setup();
            AttendanceLevelReasonStorage.Setup();
            GradeLevelStorage.Setup();
            ClassStorage.Setup();
            MarkingPeriodClassStorage.Setup();
            DayTypeStorage.Setup();
            DistrictStorage.Setup();
            MasterSchoolStorage.Setup();
            PersonStorage.Setup();
            MarkingPeriodStorage.Setup();
            GradingPeriodStorage.Setup();
            ClassAnnouncementTypeStorage.Setup();
            SchoolPersonStorage.Setup();

            SchoolStorage.Add(new Data.School.Model.School
            {
                Id = 1,
                IsActive = true,
                IsPrivate = true,
                Name = "SMITH"
            });




            

            var currentDate = DateTime.Now;
            
            SchoolYearStorage.Add(1, 1, "Current School Year", "", new DateTime(currentDate.Year, 1, 1),
                new DateTime(currentDate.Year, 12, 31));
            //some init data

            //person storage

            


            

        }
    }
    
}
