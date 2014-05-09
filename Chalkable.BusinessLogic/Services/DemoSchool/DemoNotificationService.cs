using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.BusinessLogic.Services.School.Notifications;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
 //TODO: implement service
    public class DemoNotificationService : DemoSchoolServiceBase, INotificationService
    {
        NotificationBuilder builder;


        public DemoNotificationService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
            builder = new NotificationBuilder(serviceLocator);
        }

        public IList<Notification> GetUnshownNotifications()
        {
            return Storage.NotificationStorage.GetNotifications(new NotificationQuery {Shown = false, PersonId = Context.UserLocalId});
        }

        public PaginatedList<NotificationDetails> GetNotifications(int start, int count)
        {
            return Storage.NotificationStorage.GetPaginatedNotificationsDetails(new NotificationQuery
                    {
                        PersonId = Context.UserLocalId, 
                        Start = start, 
                        Count = count
                    });
           
        }

        public IList<Notification> GetNotificationsByTypes(int personId, IList<int> types, bool? wasSent = null)
        {
            throw new NotImplementedException();
        }

        public void MarkAsShown(int[] notificationIds)
        {
            var notifications = Storage.NotificationStorage.GetNotifications(new NotificationQuery { Shown = false, PersonId = Context.UserLocalId });
            foreach (var notificationId in notificationIds)
            {
                var notification = notifications.FirstOrDefault(x => x.Id == notificationId);
                if (notification == null) continue;
                if (!NotificationSecurity.CanModify(notification, Context))
                    throw new ChalkableSecurityException();
                notification.Shown = true;
            }
            Storage.NotificationStorage.Update(notifications);
        }

        public void AddAnnouncementNewAttachmentNotification(int announcementId)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var persons = ServiceLocator.AnnouncementService.GetAnnouncementRecipientPersons(announcementId);
            var notifications = new List<Notification>();
            foreach (var person in persons)
            {
                notifications.Add(builder.BuildAnnouncementNewAttachmentNotification(ann, person));
            }
            AddNotifications(notifications);
        }

        public void AddAnnouncementNewAttachmentNotificationToPerson(int announcementId, int fromPersonId)
        {
            var announcement = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var fromPerson = ServiceLocator.PersonService.GetPerson(fromPersonId);
            var notification = builder.BuildAnnouncementNewAttachmentNotificationToPerson(announcement, fromPerson);
            AddNotification(notification);
        }

        public void AddAnnouncementReminderNotification(AnnouncementReminder announcementReminder, AnnouncementComplex announcement)
        {
            if (!NotificationSecurity.CanCreateAnnouncementNotification(announcement, Context))
                throw new ChalkableSecurityException();
            var recipients = ServiceLocator.AnnouncementService.GetAnnouncementRecipientPersons(announcement.Id);
            var notifications = new List<Notification>(); 
            foreach (var schoolPerson in recipients)
            {
                if (!announcementReminder.PersonRef.HasValue || schoolPerson.Id == announcementReminder.PersonRef)
                {
                    notifications.Add(builder.BuildAnnouncementReminderNotification(announcement, schoolPerson));
                }
            }
            AddNotifications(notifications);
        }

        public void AddAnnouncementNotificationQnToAuthor(int announcementQnAId, int announcementId)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var annQnA = ann.AnnouncementQnAs.First(x => x.Id == announcementQnAId);
            var notification = builder.BuildAnnouncementQnToAuthorNotifiaction(annQnA, ann);
            AddNotification(notification);
        }

        public void AddAnnouncementNotificationAnswerToPerson(int announcementQnAId, int announcementId)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var annQnA = ann.AnnouncementQnAs.First(x => x.Id == announcementQnAId);
            var notification = builder.BuildAnnouncementAnswerToPersonNotifiaction(annQnA, ann);
            AddNotification(notification);
        }

        public void AddAnnouncementSetGradeNotificationToPerson(int announcementId, int recipientId)
        {
            var announcement = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var recipient = ServiceLocator.PersonService.GetPerson(recipientId);
            var notification = builder.BuildAnnouncementSetGradeToPersonNotifiaction(announcement, recipient);
            AddNotification(notification);
        }

        private void AddNotification(Notification notification)
        {
            AddNotifications(new List<Notification>{notification});
        }
        private void AddNotifications(IList<Notification> notifications)
        {
            Storage.NotificationStorage.Add(notifications);
        }

        public void AddPrivateMessageNotification(int privateMessageId)
        {
            if (!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();


            var privateMessage = Storage.PrivateMessageStorage.GetDetailsById(privateMessageId, Context.UserLocalId.Value);
            var notification = builder.BuildPrivateMessageNotification(privateMessage, privateMessage.Sender, privateMessage.Recipient);
            Storage.NotificationStorage.Add(notification);
        }

        public void AddApplicationNotification(IList<Person> toPerson, Person fromPerson, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public void AddAppBudgetBalanceNotification(int recipientId, double budgetBalance)
        {
            throw new NotImplementedException();
        }

        public void AddEndMarkingPeriodNotification(int toPersonId, int markingPeriodId, int endDays, bool isNextMpNotExist,
                                                    bool isNextMpNotAssignedToClass)
        {
            var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            var toSchoolPerson = ServiceLocator.PersonService.GetPerson(toPersonId);
            var notification = builder.BuildEndMarkingPeriodNotification(markingPeriod, toSchoolPerson, endDays, isNextMpNotExist, isNextMpNotAssignedToClass);
            AddNotification(notification);
        }

        public void AddAttendanceNotification(int toPersonId, IList<Person> persons)
        {
            //TODO: think about security
            var toSchoolPerson = ServiceLocator.PersonService.GetPerson(toPersonId);
            var notification = builder.BuildAttendanceNotificationToAdmin(toSchoolPerson, persons);
            AddNotification(notification);
        }

        public void AddAttendanceNotificationToStudent(int toPersonId, int classAttendanceId)
        {
            throw new NotImplementedException();
            //var recipient = ServiceLocator.PersonService.GetPerson(toPersonId);
            //var classAtt = ServiceLocator.AttendanceService.GetClassAttendanceDetailsById(classAttendanceId);
            //var notification = builder.BuildAttendanceNotificationToStudent(recipient, classAtt);
            //AddNotification(notification);
        }
        
    }
}
