using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School.Notifications;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IServiceLocatorSchool : IServiceLocator
    {
        IServiceLocatorMaster ServiceLocatorMaster { get; }
        UserContext Context { get; }
        IPersonService PersonService { get; }
        IAddressService AddressService { get; }
        IGradeLevelService GradeLevelService { get; }
        IMarkingPeriodService MarkingPeriodService { get; }
        IClassService ClassService { get; }
        ISchoolYearService SchoolYearService { get; }
        IAnnouncementQnAService AnnouncementQnAService { get; }
        IAnnouncementService AnnouncementService { get; }
        IAnnouncementReminderService AnnouncementReminderService { get; }
        IAnnouncementAttachmentService AnnouncementAttachmentService { get; }
        IPhoneService PhoneService { get; }
        IPrivateMessageService PrivateMessageService { get; }
        IRoomService RoomService { get; }
        IPeriodService PeriodService { get; }
        ICalendarDateService CalendarDateService { get; }
        IDayTypeService DayTypeService { get; }
        IClassPeriodService ClassPeriodService { get; }
        INotificationService NotificationService { get; }
        IAttendanceService AttendanceService { get; }
        IAttendanceReasonService AttendanceReasonService { get; }
        IStudentParentService StudentParentService { get; }
        IGradingStyleService GradingStyleService { get; }
        IStudentAnnouncementService StudentAnnouncementService { get; }
        IClassAnnouncementTypeService ClassAnnouncementTypeService { get; }
        IInfractionService InfractionService { get; }
        IApplicationSchoolService ApplicationSchoolService { get; }
        IDisciplineService DisciplineService { get; }
        IGradingStatisticService GradingStatisticService { get; }
        IAppMarketService AppMarketService { get; }
        ISchoolService SchoolService { get; }
        ISchoolPersonService SchoolPersonService { get; }
        IStandardService StandardService { get; }
        IAlphaGradeService AlphaGradeService { get; }
        IAlternateScoreService AlternateScoreService { get; }
        IGradingPeriodService GradingPeriodService { get; }
        ISyncService SyncService { get; }
        IGradingStandardService GradingStandardService { get; }
    }
    public class ServiceLocatorSchool : ServiceLocator, IServiceLocatorSchool
    {
        private IServiceLocatorMaster serviceLocatorMaster;
        private IPersonService personService;
        private IAddressService addressSerivce;
        private IGradeLevelService gradeLevelService;
        private IMarkingPeriodService markingPeriodService;
        private IClassService classService;
        private ISchoolYearService schoolYearService;
        private IAnnouncementQnAService announcementQnAService;
        private IAnnouncementReminderService announcementReminderService;
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
        private IClassAnnouncementTypeService classClassAnnouncementTypeService;
        private IInfractionService disciplineTypeService;
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
        private ISyncService syncService;
        private IGradingStandardService gradingStandardService;

        public ServiceLocatorSchool(IServiceLocatorMaster serviceLocatorMaster)
            : base(serviceLocatorMaster.Context)
        {
            this.serviceLocatorMaster = serviceLocatorMaster;
            personService = new PersonService(this);
            addressSerivce = new AddressService(this);
            gradeLevelService = new GradeLevelService(this);
            markingPeriodService = new MarkingPeriodService(this);
            classService = new ClassService(this);
            schoolYearService = new SchoolYearService(this);
            announcementQnAService = new AnnouncementQnAService(this);
            announcementReminderService = new AnnouncementReminderService(this);
            announcementService = new AnnouncementService(this);
            announcementAttachmentService = new AnnouncementAttachmentService(this);
            phoneService = new PhoneService(this);
            privateMessageService = new PrivateMessageService(this);
            roomService = new RoomService(this);
            periodService = new PeriodService(this);
            calendarDateService = new CalendarDateService(this);
            dayTypeService = new DayTypeService(this);
            classPeriodService = new ClassPeriodService(this);
            notificationService = new NotificationService(this);
            attendanceReasonService = new AttendanceReasonService(this);
            attendanceService = new AttendanceService(this);
            studentParentService = new StudentParentService(this);
            gradingStyleService = new GradingStyleService(this);
            studentAnnouncementService = new StudentAnnouncementService(this);
            classClassAnnouncementTypeService = new ClassClassAnnouncementTypeService(this);
            disciplineTypeService = new InfractionService(this);
            applicationSchoolService = new ApplicationSchoolService(this);
            disciplineService = new DisciplineService(this);
            gradingStatisticService = new GradingStatisticService(this);
            appMarketService = new AppMarketService(this);
            schoolService = new SchoolService(this);
            schoolPersonService = new SchoolPersonService(this);
            standardService = new StandardService(this);
            alphaGradeService = new AlphaGradeService(this);
            alternateScoreService = new AlternateScoreService(this);
            gradingPeriodService = new GradingPeriodService(this);
            syncService = new SyncService(this);
            gradingStandardService = new GradingStandardService(this);
        }

        public IPersonService PersonService { get { return personService; } }
        public IAddressService AddressService { get { return addressSerivce; } }
        public IGradeLevelService GradeLevelService{ get { return gradeLevelService; } }
        public IMarkingPeriodService MarkingPeriodService { get { return markingPeriodService; } }
        public IClassService ClassService { get { return classService; } }
        public ISchoolYearService SchoolYearService { get { return schoolYearService; } }
        public IAnnouncementQnAService AnnouncementQnAService{ get { return announcementQnAService; } }
        public IAnnouncementService AnnouncementService { get { return announcementService; } }
        public IAnnouncementReminderService AnnouncementReminderService { get { return announcementReminderService; } }
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
        public IClassAnnouncementTypeService ClassAnnouncementTypeService { get { return classClassAnnouncementTypeService; } }
        public IInfractionService InfractionService { get { return disciplineTypeService; } }
        public IApplicationSchoolService ApplicationSchoolService { get { return applicationSchoolService; } }
        public IDisciplineService DisciplineService { get { return disciplineService; } }
        public IGradingStatisticService GradingStatisticService { get { return gradingStatisticService; } }
        public IAppMarketService AppMarketService { get { return appMarketService; } }
        public ISchoolService SchoolService { get { return schoolService; } }
        public ISchoolPersonService SchoolPersonService { get { return schoolPersonService; } }
        public IStandardService StandardService { get { return standardService; } }
        public IAlphaGradeService AlphaGradeService { get { return alphaGradeService; } }
        public IAlternateScoreService AlternateScoreService { get { return alternateScoreService; } }
        public IGradingPeriodService GradingPeriodService { get { return gradingPeriodService; } }
        public ISyncService SyncService { get { return syncService; } }
        public IGradingStandardService GradingStandardService { get { return gradingStandardService; }
        }
    }
}
