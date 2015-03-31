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
        private IStudentService studentService;
        private IStaffService staffService;
        private IUserSchoolService userSchoolService;
        private IPracticeGradeService practiceGradeService;

        public DemoServiceLocatorSchool(IServiceLocatorMaster serviceLocatorMaster)
            : base(serviceLocatorMaster.Context)
        {
            this.serviceLocatorMaster = serviceLocatorMaster;
            notificationService = new DemoNotificationService(this);
            appMarketService = new DemoAppMarketService(this);
            announcementService = new DemoAnnouncementService(this);
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
        public IStudentService StudentService { get { return studentService; } }
        public IStaffService StaffService { get { return staffService; } }
        public IUserSchoolService UserSchoolService { get { return userSchoolService; } }

        public IScheduledTimeSlotService ScheduledTimeSlotService { get{throw new NotImplementedException();} }
        public IBellScheduleService BellScheduleService { get { throw new NotImplementedException(); } }
        public IPracticeGradeService PracticeGradeService{get{ return practiceGradeService; } }

        public IDbService SchoolDbService
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

}