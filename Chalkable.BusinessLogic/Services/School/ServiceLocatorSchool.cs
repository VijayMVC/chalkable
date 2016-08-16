using System;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.BusinessLogic.Services.School.Notifications;
using Chalkable.BusinessLogic.Services.School.PanoramaSettings;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model.Announcements;

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
        IStudentAnnouncementService StudentAnnouncementService { get; }
        IClassAnnouncementTypeService ClassAnnouncementTypeService { get; }
        IInfractionService InfractionService { get; }
        IApplicationSchoolService ApplicationSchoolService { get; }
        IDisciplineService DisciplineService { get; }
        IGradingStatisticService GradingStatisticService { get; }
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
        IPracticeGradeService PracticeGradeService { get; }
        IAttendanceMonthService AttendanceMonthService { get; }
        IGradedItemService GradedItemService { get; }
        IAnnouncementAttributeService AnnouncementAttributeService { get; }
        IAnnouncementAssignedAttributeService AnnouncementAssignedAttributeService { get; }
        IContactService ContactService { get; }
        ITeacherCommentService TeacherCommentService { get; }
        IGroupService GroupService { get; }
        ICourseTypeService CourseTypeService { get; }
        IDbService SchoolDbService { get; }
        IDbMaintenanceService DbMaintenanceService { get; }
        ISettingsService SettingsService { get;  }
        ILPGalleryCategoryService LPGalleryCategoryService { get; }
        IPersonSettingService PersonSettingService { get; }

        ILessonPlanService LessonPlanService { get; }
        IClassAnnouncementService ClassAnnouncementService { get; }
        IAdminAnnouncementService AdminAnnouncementService { get; }
        IAnnouncementFetchService AnnouncementFetchService { get; }
        IBaseAnnouncementService GetAnnouncementService(AnnouncementTypeEnum? type);

        IAttachementService AttachementService { get; } 
        ILEService LeService { get; }
        IStudentCustomAlertDetailService StudentCustomAlertDetailService { get; }
        IPanoramaSettingsService PanoramaSettingsService { get; }
        IStandardizedTestService StandardizedTestService { get; }
        ISupplementalAnnouncementService SupplementalAnnouncementService { get; }
        IEthnicityService EthnicityService { get; }
        ILanguageService LanguageService { get; }
        ICountryService CountryService { get; }

        IAnnouncementCommentService AnnouncementCommentService { get; }
        IAppSettingService AppSettingService { get; }
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
        private IClassAnnouncementTypeService classClassAnnouncementTypeService;
        private IInfractionService infractionService;
        private IApplicationSchoolService applicationSchoolService;
        private IDisciplineService disciplineService;
        private IGradingStatisticService gradingStatisticService;
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
        private IPracticeGradeService practiceGradeService;
        private IAttendanceMonthService attendanceMonthService;
        private IGradedItemService gradedItemService;
        private IAnnouncementAttributeService announcementAttributeService;
        private IAnnouncementAssignedAttributeService announcementAssignedAttributeService;
        private IContactService contactService;
        private ITeacherCommentService teacherCommentService;
        private IDbMaintenanceService dbMaintenanceService;
        private IGroupService groupService;
        private ISettingsService settingsService;
        private ICourseTypeService courseTypeService;
        private ILPGalleryCategoryService lpGalleryCategoryService;
        private IPersonSettingService personSettingService;

        private ILessonPlanService lessonPlanService;
        private IClassAnnouncementService classAnnouncementService;
        private IAdminAnnouncementService adminAnnouncementService;
        private IAnnouncementFetchService announcementFetchService;

        private ILEService leService;
        private IAttachementService attachementService;
        private IStudentCustomAlertDetailService studentCustomAlertDetailService;
        private IPanoramaSettingsService panoramaSettingsService;
        private IStandardizedTestService standardizedTestService;

        private ISupplementalAnnouncementService supplementalAnnouncementService;
        private IEthnicityService ethnicityService;

        private ILanguageService languageService;
        private ICountryService countryService;
        private IAnnouncementCommentService announcementCommentService;
        
        private IAppSettingService appSettingService;

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
            studentAnnouncementService = new StudentAnnouncementService(this);
            classClassAnnouncementTypeService = new ClassClassAnnouncementTypeService(this);
            infractionService = new InfractionService(this);
            applicationSchoolService = new ApplicationSchoolService(this);
            disciplineService = new DisciplineService(this);
            gradingStatisticService = new GradingStatisticService(this);
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
            practiceGradeService = new PracticeGradeService(this);
            attendanceMonthService = new AttendanceMonthService(this);
            gradedItemService = new GradedItemService(this);
            announcementAttributeService = new AnnouncementAttributeService(this);
            announcementAssignedAttributeService = new AnnouncementAssignedAttributeService(this);
            contactService = new ContactService(this);
            teacherCommentService = new TeacherCommentService(this);
            dbMaintenanceService = new DbMaintenanceService(this);
            groupService = new GroupService(this);
            courseTypeService = new CourseTypeService(this);
            settingsService = new SettingsService(this);
            lpGalleryCategoryService = new LPGalleryCategoryService(this);
            lessonPlanService = new LessonPlanService(this);
            classAnnouncementService = new ClassAnnouncementService(this);
            adminAnnouncementService = new AdminAnnouncementService(this);
            announcementFetchService = new AnnouncementFetchService(this);
            leService = new LEService(this);
            attachementService = new AttachmentService(this);
            personSettingService = new PersonSettingService(this);
            studentCustomAlertDetailService = new StudentCustomAlertDetailService(this);
            panoramaSettingsService = new PanoramaSettingsService(this);
            standardizedTestService = new StandardizedTestService(this);
            supplementalAnnouncementService = new SupplementalAnnouncementService(this);
            ethnicityService = new EthnicityService(this);
            languageService = new LanguageService(this);
            countryService = new CountryService(this);
            announcementCommentService = new AnnouncementCommentService(this);
            appSettingService = new AppSettingService(this);
        }

        public IPersonService PersonService { get { return personService; } }
        public IAddressService AddressService { get { return addressSerivce; } }
        public IGradeLevelService GradeLevelService{ get { return gradeLevelService; } }
        public IMarkingPeriodService MarkingPeriodService { get { return markingPeriodService; } }
        public IClassService ClassService { get { return classService; } }
        public ISchoolYearService SchoolYearService { get { return schoolYearService; } }
        public IAnnouncementQnAService AnnouncementQnAService{ get { return announcementQnAService; } }
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
        public IClassAnnouncementTypeService ClassAnnouncementTypeService { get { return classClassAnnouncementTypeService; } }
        public IInfractionService InfractionService { get { return infractionService; } }
        public IApplicationSchoolService ApplicationSchoolService { get { return applicationSchoolService; } }
        public IDisciplineService DisciplineService { get { return disciplineService; } }
        public IGradingStatisticService GradingStatisticService { get { return gradingStatisticService; } }
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
        public IPracticeGradeService PracticeGradeService { get { return practiceGradeService; } }
        public IAttendanceMonthService AttendanceMonthService { get { return attendanceMonthService; } }
        public IGradedItemService GradedItemService => gradedItemService;
        public IAnnouncementAttributeService AnnouncementAttributeService => announcementAttributeService;
        public IContactService ContactService => contactService;
        public IAnnouncementAssignedAttributeService AnnouncementAssignedAttributeService => announcementAssignedAttributeService;
        public ITeacherCommentService TeacherCommentService => teacherCommentService;
        public IGroupService GroupService => groupService;
        public ICourseTypeService CourseTypeService => courseTypeService;
        public IDbMaintenanceService DbMaintenanceService => dbMaintenanceService;
        public ISettingsService SettingsService => settingsService;
        public ILPGalleryCategoryService LPGalleryCategoryService => lpGalleryCategoryService;
        public ILEService LeService => leService;
        public IAttachementService AttachementService => attachementService;
        public IPersonSettingService PersonSettingService => personSettingService;

        public IDbService SchoolDbService
        {
            get { return schoolDbService; }
            protected set { schoolDbService = value; }
        }

        public ILessonPlanService LessonPlanService => lessonPlanService;
        public IClassAnnouncementService ClassAnnouncementService => classAnnouncementService;
        public IAdminAnnouncementService AdminAnnouncementService => adminAnnouncementService;
        public IAnnouncementFetchService AnnouncementFetchService => announcementFetchService;

        public IStudentCustomAlertDetailService StudentCustomAlertDetailService => studentCustomAlertDetailService;
        public IPanoramaSettingsService PanoramaSettingsService => panoramaSettingsService;
        public IStandardizedTestService StandardizedTestService => standardizedTestService;

        public IBaseAnnouncementService GetAnnouncementService(AnnouncementTypeEnum? type)
        {
            if (!type.HasValue)
                return classAnnouncementService;
            switch (type.Value)
            {
                case AnnouncementTypeEnum.Class: return classAnnouncementService;
                case AnnouncementTypeEnum.Admin: return adminAnnouncementService;
                case AnnouncementTypeEnum.LessonPlan: return lessonPlanService;
                case AnnouncementTypeEnum.Supplemental: return supplementalAnnouncementService;
                default : throw new NotSupportedException("Not supported announcement type");
            }
        }

        public ISupplementalAnnouncementService SupplementalAnnouncementService => supplementalAnnouncementService;
        public IEthnicityService EthnicityService => ethnicityService;
        public ILanguageService LanguageService => languageService;
        public ICountryService CountryService => countryService;
        public IAnnouncementCommentService AnnouncementCommentService => announcementCommentService;
        public IAppSettingService AppSettingService => appSettingService;
    }
}
