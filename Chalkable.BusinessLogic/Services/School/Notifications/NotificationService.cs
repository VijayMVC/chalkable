using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School.Notifications
{
    public interface INotificationService
    {
        IList<Notification> GetUnshownNotifications();
        IList<Notification> GetNotifications(int start, int count);
        IList<Notification> GetNotificationsByTypes(Guid personId, IList<int> types, bool? wasSent = null);

        void AddAnnouncementNewAttachmentNotification(Guid announcementId);
        void AddAnnouncementNewAttachmentNotificationToPerson(Guid announcementId, Guid fromPersonId);
        void AddAnnouncementReminderNotification(AnnouncementReminder announcementReminder);
        void AddAnnouncementNotificationQnToAuthor(Guid announcementQnAId);
        void AddAnnouncementNotificationAnswerToPerson(Guid announcementQnAId);
        void AddAnnouncementSetGradeNotificationToPerson(Guid announcement, Guid recipient);
        void AddPrivateMessageNotification(Guid privateMessageId);
        void AddApplicationNotification(IList<Person> toPerson, Person fromPerson, Guid applicationId);
        void AddAppBudgetBalanceNotification(Guid recipientId, double budgetBalance);
        void AddEndMarkingPeriodNotification(Guid toPersonId, Guid markingPeriodId, int endDays, bool isNextMpNotExist, bool isNextMpNotAssignedToClass);
        void AddAttendanceNotification(Guid toPersonId, IList<Person> persons);
        void AddAttendanceNotificationToStudent(Guid toPersonId, Guid classAttendanceId);
        void AddAttendanceNotificationToTeacher(Guid toPersonId, ClassPeriod classPeriod, DateTime dateTime);
        void MarkAsShown(int[] notificationIds);
    }


    //TODO: implement service
    public class NotificationService : SchoolServiceBase, INotificationService
    {
        NotificationBuilder builder;


        public NotificationService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            builder = new NotificationBuilder(serviceLocator);
        }

        public IList<Notification> GetUnshownNotifications()
        {
            throw new NotImplementedException();
        }

        public IList<Notification> GetNotifications(int start, int count)
        {
            throw new NotImplementedException();
        }

        public IList<Notification> GetNotificationsByTypes(Guid personId, IList<int> types, bool? wasSent = null)
        {
            throw new NotImplementedException();
        }

        public void AddAnnouncementNewAttachmentNotification(Guid announcementId)
        {
            throw new NotImplementedException();
        }

        public void AddAnnouncementNewAttachmentNotificationToPerson(Guid announcementId, Guid fromPersonId)
        {
            throw new NotImplementedException();
        }

        public void AddAnnouncementReminderNotification(AnnouncementReminder announcementReminder)
        {
            throw new NotImplementedException();
        }

        public void AddAnnouncementNotificationQnToAuthor(Guid announcementQnAId)
        {
            throw new NotImplementedException();
        }

        public void AddAnnouncementNotificationAnswerToPerson(Guid announcementQnAId)
        {
            throw new NotImplementedException();
        }

        public void AddAnnouncementSetGradeNotificationToPerson(Guid announcement, Guid recipient)
        {
            throw new NotImplementedException();
        }

        public void AddPrivateMessageNotification(Guid privateMessageId)
        {
            throw new NotImplementedException();
        }

        public void AddApplicationNotification(IList<Person> toPerson, Person fromPerson, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public void AddAppBudgetBalanceNotification(Guid recipientId, double budgetBalance)
        {
            throw new NotImplementedException();
        }

        public void AddEndMarkingPeriodNotification(Guid toPersonId, Guid markingPeriodId, int endDays, bool isNextMpNotExist,
                                                    bool isNextMpNotAssignedToClass)
        {
            throw new NotImplementedException();
        }

        public void AddAttendanceNotification(Guid toPersonId, IList<Person> persons)
        {
            throw new NotImplementedException();
        }

        public void AddAttendanceNotificationToStudent(Guid toPersonId, Guid classAttendanceId)
        {
            throw new NotImplementedException();
        }

        public void AddAttendanceNotificationToTeacher(Guid toPersonId, ClassPeriod classPeriod, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public void MarkAsShown(int[] notificationIds)
        {
            throw new NotImplementedException();
        }
    }
}
