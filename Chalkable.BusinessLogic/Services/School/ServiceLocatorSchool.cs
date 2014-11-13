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
        IReportingService ReportService { get; }
        IGradingCommentService GradingCommentService { get; }
        IGradingScaleService GradingScaleService { get; }
        IClassroomOptionService ClassroomOptionService { get; }
        IPersonEmailService PersonEmailService { get; }
        IScheduledTimeSlotService ScheduledTimeSlotService { get; }
        IStudentService StudentService { get; }
        IStaffService StaffService { get; }
        IUserSchoolService UserSchoolService { get; }
        IBellScheduleService BellScheduleService { get; }
        IDbService SchoolDbService { get; }
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
        private IInfractionService infractionService;
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
        private IReportingService reportService;
        private IGradingCommentService gradingCommentService;
        private IGradingScaleService gradingScaleService;
        private IClassroomOptionService classroomOptionService;
        private IPersonEmailService personEmailService;
        private IDbService schoolDbService;
        private IScheduledTimeSlotService scheduledTimeSlotService;
        private IStudentService studentService;
        private IUserSchoolService userSchoolService;
        private IStaffService staffService;
        private IBellScheduleService bellScheduleService;

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
            infractionService = new InfractionService(this);
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
            reportService = new ReportingService(this);
            gradingCommentService = new GradingCommentService(this);
            gradingScaleService = new GradingScaleService(this);
            classroomOptionService = new ClassroomOptionService(this);
            personEmailService = new PersonEmailService(this);
            schoolDbService = new DbService(Context != null ? Context.SchoolConnectionString : null);
            scheduledTimeSlotService = new ScheduledTimeSlotService(this);
            studentService = new StudentService(this);
            staffService = new StaffService(this);
            userSchoolService = new UserSchoolService(this);
            bellScheduleService = new BellScheduleService(this);
        }

        public IPersonService PersonService { get { return personService; } }
        public IAddressService AddressService { get { return addressSerivce; } }
        public IGradeLevelService GradeLevelService{ get { return gradeLevelService; } }
        public IMarkingPeriodService MarkingPeriodService { get { return markingPeriodService; } }
        public IClassService ClassService { get { return classService; } }
        public ISchoolYearService SchoolYearService { get { return schoolYearService; } }
        public IAnnouncementQnAService AnnouncementQnAService{ get { return announcementQnAService; } }
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
        public IClassAnnouncementTypeService ClassAnnouncementTypeService { get { return classClassAnnouncementTypeService; } }
        public IInfractionService InfractionService { get { return infractionService; } }
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
        public IGradingStandardService GradingStandardService { get { return gradingStandardService; } }
        public IReportingService ReportService { get { return reportService; } }
        public IGradingCommentService GradingCommentService { get { return gradingCommentService; } }
        public IGradingScaleService GradingScaleService { get { return gradingScaleService; } }
        public IClassroomOptionService ClassroomOptionService { get { return classroomOptionService; } }
        public IPersonEmailService PersonEmailService { get { return personEmailService; } }
        public IScheduledTimeSlotService ScheduledTimeSlotService { get { return scheduledTimeSlotService; } }
        public IStudentService StudentService { get { return studentService; } }
        public IStaffService StaffService { get { return staffService; } }
        public IUserSchoolService UserSchoolService { get { return userSchoolService; } }
        public IBellScheduleService BellScheduleService { get { return bellScheduleService; } }
        public IDbService SchoolDbService
        {
            get { return schoolDbService; }
            protected set { schoolDbService = value; }
        }
        

    }
}
