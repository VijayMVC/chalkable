using System;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.BusinessLogic.Services.School.Notifications;
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
        private IAnnouncementService announcementService;
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
        private IGradingStyleService gradingStyleService;
        private IStudentAnnouncementService studentAnnouncementService;
        private IClassAnnouncementTypeService classAnnouncementTypeService;
        private IApplicationSchoolService applicationSchoolService;
        private IDisciplineService disciplineService;
        private IGradingStatisticService gradingStatisticService;
        private IAppMarketService appMarketService;
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
        private ISisUserService sisUserService;
        private IStudentService studentService;
        private IStaffService staffService;


        public DemoServiceLocatorSchool(IServiceLocatorMaster serviceLocatorMaster, DemoStorage storage)
            : base(serviceLocatorMaster.Context)
        {
            this.serviceLocatorMaster = serviceLocatorMaster;
            notificationService = new DemoNotificationService(this, storage);
            appMarketService = new DemoAppMarketService(this, storage);
            announcementService = new DemoAnnouncementService(this, storage);
            personService = new DemoPersonService(this, storage);
            schoolYearService = new DemoSchoolYearService(this, storage);
            markingPeriodService = new DemoMarkingPeriodService(this, storage);
            gradingStyleService = new DemoGradingStyleService(this, storage);
            addressService = new DemoAddressService(this, storage);
            gradeLevelService = new DemoGradeLevelService(this, storage);
            classService = new DemoClassService(this, storage);
            announcementQnAService = new DemoAnnouncementQnAService(this, storage);
            announcementAttachmentService = new DemoAnnouncementAttachmentService(this, storage);
            phoneService = new DemoPhoneService(this, storage);
            privateMessageService = new DemoPrivateMessageService(this, storage);
            roomService = new DemoRoomService(this, storage);
            periodService = new DemoPeriodService(this, storage);
            calendarDateService = new DemoCalendarDateService(this, storage);
            dayTypeService = new DemoDayTypeService(this, storage);
            classPeriodService = new DemoClassPeriodService(this, storage);
            attendanceReasonService = new DemoAttendanceReasonService(this, storage);
            attendanceService = new DemoAttendanceService(this, storage);
            studentParentService = new DemoStudentParentService(this, storage);
            studentAnnouncementService = new DemoStudentAnnouncementService(this , storage);
            applicationSchoolService = new DemoApplicationSchoolService(this, storage);
            disciplineService = new DemoDisciplineService(this, storage);
            gradingStatisticService = new DemoGradingStatisticService(this, storage);
            schoolService = new DemoSchoolService(this, storage);
            schoolPersonService = new DemoSchoolPersonService(this, storage);
            standardService = new DemoStandardService(this, storage);
            alphaGradeService = new DemoAlphaGradeService(this, storage);
            alternateScoreService = new DemoAlternateScoreService(this, storage);
            gradingPeriodService = new DemoGradingPeriodService(this, storage);
            classAnnouncementTypeService = new DemoClassAnnouncementTypeService(this, storage);
            gradingStandardService = new DemoGradingStandardService(this, storage);
            InfractionService = new DemoInfractionService(this, storage);
            StorageBlobService = new DemoStorageBlobService(this ,storage);            
            gradingCommentService = new DemoGradingCommentService(this, storage);
            gradingScaleService = new DemoGradingScaleService(this, storage);
            classroomOptionService = new DemoClassroomOptionService(this, storage);
            sisUserService = new DemoSisUserService(this, storage);
            personEmailService = new DemoPersonEmailService(this, storage);
            studentService = new DemoStudentService(this, storage);
            staffService = new DemoStaffService(this, storage);
        }

        public IPersonService PersonService { get { return personService; } }
        public IAddressService AddressService { get { return addressService; } }
        public IGradeLevelService GradeLevelService { get { return gradeLevelService; } }
        public IMarkingPeriodService MarkingPeriodService { get { return markingPeriodService; } }
        public IClassService ClassService { get { return classService; } }
        public ISchoolYearService SchoolYearService { get { return schoolYearService; } }
        public IAnnouncementQnAService AnnouncementQnAService { get { return announcementQnAService; } }
        public IAnnouncementService AnnouncementService { get { return announcementService; } }
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
        public IGradingStyleService GradingStyleService { get { return gradingStyleService; } }
        public IStudentAnnouncementService StudentAnnouncementService { get { return studentAnnouncementService; } }
        public IClassAnnouncementTypeService ClassAnnouncementTypeService { get { return classAnnouncementTypeService; } }
        public IInfractionService InfractionService { get; private set; }
        public IApplicationSchoolService ApplicationSchoolService { get { return applicationSchoolService; } }
        public IDisciplineService DisciplineService { get { return disciplineService; } }
        public IGradingStatisticService GradingStatisticService { get { return gradingStatisticService; } }
        public IAppMarketService AppMarketService { get { return appMarketService; } }
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
        public ISisUserService SisUserService { get { return sisUserService; } }
        public IStudentService StudentService { get { return studentService; } }
        public IStaffService StaffService { get { return staffService; } }

        public IScheduledTimeSlotService ScheduledTimeSlotService { get{throw new NotImplementedException();} }

        public IDbService SchoolDbService
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

}