﻿using Chalkable.BusinessLogic.Services.Master;
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
        ICourseService CourseService { get; }
        IAnnouncementQnAService AnnouncementQnAService { get; }
        IAnnouncementService AnnouncementService { get; }
        IAnnouncementReminderService AnnouncementReminderService { get; }
        IAnnouncementAttachmentService AnnouncementAttachmentService { get; }
        IPhoneService PhoneService { get; }
        IPrivateMessageService PrivateMessageService { get; }
        IRoomService RoomService { get; }
        IPeriodService PeriodService { get; }
        ICalendarDateService CalendarDateService { get; }
        IScheduleSectionService ScheduleSectionService { get; }
        IClassPeriodService ClassPeriodService { get; }
        INotificationService NotificationService { get; }
        IAttendanceService AttendanceService { get; }
        IAttendanceReasonService AttendanceReasonService { get; }
        IStudentParentService StudentParentService { get; }
        IGradingStyleService GradingStyleService { get; }
        IStudentAnnouncementService StudentAnnouncementService { get; }
        IAnnouncementTypeService AnnouncementTypeService { get; }
        IFinalGradeService FinalGradeService { get; }
        IDisciplineTypeService DisciplineTypeService { get; }
        IApplicationSchoolService ApplicationSchoolService { get; }
        IDisciplineService DisciplineService { get; }
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
        private ICourseService courseService;
        private IAnnouncementQnAService announcementQnAService;
        private IAnnouncementReminderService announcementReminderService;
        private IAnnouncementService announcementService;
        private IAnnouncementAttachmentService announcementAttachmentService;
        private IPhoneService phoneService;
        private IPrivateMessageService privateMessageService;
        private IRoomService roomService;
        private IPeriodService periodService;
        private ICalendarDateService calendarDateService;
        private IScheduleSectionService scheduleSectionService;
        private IClassPeriodService classPeriodService;
        private INotificationService notificationService;
        private IAttendanceService attendanceService;
        private IAttendanceReasonService attendanceReasonService;
        private IStudentParentService studentParentService;
        private IGradingStyleService gradingStyleService;
        private IStudentAnnouncementService studentAnnouncementService;
        private IAnnouncementTypeService announcementTypeService;
        private IFinalGradeService finalGradeService;
        private IDisciplineTypeService disciplineTypeService;
        private IApplicationSchoolService applicationSchoolService;
        private IDisciplineService disciplineService;

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
            courseService = new CourseService(this);
            announcementQnAService = new AnnouncementQnAService(this);
            announcementReminderService = new AnnouncementReminderService(this);
            announcementService = new AnnouncementService(this);
            announcementAttachmentService = new AnnouncementAttachmentService(this);
            phoneService = new PhoneService(this);
            privateMessageService = new PrivateMessageService(this);
            roomService = new RoomService(this);
            periodService = new PeriodService(this);
            calendarDateService = new CalendarDateService(this);
            scheduleSectionService = new ScheduleSectionService(this);
            classPeriodService = new ClassPeriodService(this);
            notificationService = new NotificationService(this);
            attendanceReasonService = new AttendanceReasonService(this);
            attendanceService = new AttendanceService(this);
            studentParentService = new StudentParentService(this);
            gradingStyleService = new GradingStyleService(this);
            studentAnnouncementService = new StudentAnnouncementService(this);
            announcementTypeService = new AnnouncementTypeService(this);
            finalGradeService = new FinalGradeService(this);
            disciplineTypeService = new DisciplineTypeService(this);
            applicationSchoolService = new ApplicationSchoolService(this);
            disciplineService = new DisciplineService(this);
        }

        public IPersonService PersonService { get { return personService; } }
        public IAddressService AddressService { get { return addressSerivce; } }
        public IGradeLevelService GradeLevelService{ get { return gradeLevelService; } }
        public IMarkingPeriodService MarkingPeriodService { get { return markingPeriodService; } }
        public IClassService ClassService { get { return classService; } }
        public ISchoolYearService SchoolYearService { get { return schoolYearService; } }
        public ICourseService CourseService { get { return courseService; } }
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
        public IScheduleSectionService ScheduleSectionService { get { return scheduleSectionService; } }
        public IClassPeriodService ClassPeriodService { get { return classPeriodService; } }
        public INotificationService NotificationService { get { return notificationService; } }
        public IAttendanceService AttendanceService { get { return attendanceService; } }
        public IAttendanceReasonService AttendanceReasonService { get { return attendanceReasonService; } }
        public IStudentParentService StudentParentService { get { return studentParentService; } }
        public IGradingStyleService GradingStyleService { get { return gradingStyleService; } }
        public IStudentAnnouncementService StudentAnnouncementService { get { return studentAnnouncementService; } }
        public IAnnouncementTypeService AnnouncementTypeService { get { return announcementTypeService; } }
        public IFinalGradeService FinalGradeService { get { return finalGradeService; } }
        public IDisciplineTypeService DisciplineTypeService { get { return disciplineTypeService; } }
        public IApplicationSchoolService ApplicationSchoolService { get { return applicationSchoolService; } }
        public IDisciplineService DisciplineService { get { return disciplineService; } }
    }
}
