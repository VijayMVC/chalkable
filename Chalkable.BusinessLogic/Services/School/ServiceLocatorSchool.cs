using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IServiceLocatorSchool
    {
        IServiceLocatorMaster ServiceLocatorMaster { get; }
        UserContext Context { get; }
        IPersonService PersonService { get; }
        IAddressSerivce AddressSerivce { get; }
        IGradeLevelService GradeLevelService { get; }
        IMarkingPeriodService MarkingPeriodService { get; }
        IClassService ClassService { get; }
        ISchoolYearService SchoolYearService { get; }
        ICourseInfoService CourseInfoService { get; }
        IAnnouncementQnAService AnnouncementQnAService { get; }
        IAnnouncementService AnnouncementService { get; }
        IAnnouncementReminderService AnnouncementReminderService { get; }
        IAnnouncementAttachmentService AnnouncementAttachmentService { get; }
        IStorageMonitorService StorageMonitorService { get; }
        IPhoneService PhoneService { get; }
        IPrivateMessageService PrivateMessageService { get; }
        IRoomService RoomService { get; }
    }
    public class ServiceLocatorSchool : ServiceLocator, IServiceLocatorSchool
    {
        private IServiceLocatorMaster serviceLocatorMaster;
        private IPersonService personService;
        private IAddressSerivce addressSerivce;
        private IGradeLevelService gradeLevelService;
        private IMarkingPeriodService markingPeriodService;
        private IClassService classService;
        private ISchoolYearService schoolYearService;
        private ICourseInfoService courseInfoService;
        private IAnnouncementQnAService announcementQnAService;
        private IAnnouncementReminderService announcementReminderService;
        private IAnnouncementService announcementService;
        private IAnnouncementAttachmentService announcementAttachmentService;
        private IStorageMonitorService storageMonitorService;
        private IPhoneService phoneService;
        private IPrivateMessageService privateMessageService;
        private IRoomService roomService;


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
            courseInfoService = new CourseInfoService(this);
            announcementQnAService = new AnnouncementQnAService(this);
            announcementReminderService = new AnnouncementReminderService(this);
            announcementService = new AnnouncementService(this);
            announcementAttachmentService = new AnnouncementAttachmentService(this);
            storageMonitorService = new StorageMonitorService(this);
            phoneService = new PhoneService(this);
            privateMessageService = new PrivateMessageService(this);
            roomService = new RoomService(this);
        }

        public IPersonService PersonService
        {
            get { return personService; }
        }
        public IAddressSerivce AddressSerivce
        {
            get { return addressSerivce; }
        }
        public IGradeLevelService GradeLevelService
        {
            get { return gradeLevelService; }
        }

        public IMarkingPeriodService MarkingPeriodService 
        {
            get { return markingPeriodService; }
        }

        public IClassService ClassService
        {
            get { return classService; }
        }
        public ISchoolYearService SchoolYearService
        {
            get { return schoolYearService; }
        }
        public ICourseInfoService CourseInfoService
        {
            get { return courseInfoService; }
        }
        public IAnnouncementQnAService AnnouncementQnAService
        {
            get { return announcementQnAService; }
        }
        public IAnnouncementService AnnouncementService
        {
            get { return announcementService; }
        }
        public IAnnouncementReminderService AnnouncementReminderService
        {
            get { return announcementReminderService; }
        }

        public IAnnouncementAttachmentService AnnouncementAttachmentService
        {
            get { return announcementAttachmentService; }
        }

        public IStorageMonitorService StorageMonitorService
        {
            get { return storageMonitorService; }
        }

        public IPhoneService PhoneService
        {
            get { return phoneService; }
        }
        public IPrivateMessageService PrivateMessageService
        {
            get { return privateMessageService; }
        }
        public IRoomService RoomService
        {
            get { return roomService; }
        }

        public IServiceLocatorMaster ServiceLocatorMaster
        {
            get { return serviceLocatorMaster; }
        }
    }
}
