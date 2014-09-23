﻿using System;
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
            return Storage.NotificationStorage.GetNotifications(new NotificationQuery { Shown = false, PersonId = Context.PersonId });
        }

        public PaginatedList<NotificationDetails> GetNotifications(int start, int count)
        {
            return Storage.NotificationStorage.GetPaginatedNotificationsDetails(new NotificationQuery
                    {
                        PersonId = Context.PersonId, 
                        Start = start, 
                        Count = count,
                        SchoolId = Context.SchoolLocalId.Value
                    });
           
        }


        public IList<Notification> GetNotificationsByTypes(int personId, IList<int> types, bool? wasSent = null)
        {
            throw new NotImplementedException();
        }

        public void MarkAsShown(int[] notificationIds)
        {
            var notifications = Storage.NotificationStorage.GetNotifications(new NotificationQuery { Shown = false, PersonId = Context.PersonId });
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

        public void AddAnnouncementNewAttachmentNotificationToTeachers(int announcementId, int fromPersonId)
        {
            var announcement = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var fromPerson = ServiceLocator.PersonService.GetPerson(fromPersonId);
            var teachers = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
                {
                    RoleId = CoreRoles.TEACHER_ROLE.Id,
                    ClassId = announcement.ClassRef
                });
            var notifications = teachers.Select(x => builder.BuildAnnouncementNewAttachmentNotificationToPerson(announcement, x, fromPerson)).ToList();
            AddNotifications(notifications);
        }

        
        public void AddAnnouncementNotificationQnToAuthor(int announcementQnAId, int announcementId)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var authors = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery { ClassId = ann.ClassRef, RoleId = CoreRoles.TEACHER_ROLE.Id });
            var annQnA = ann.AnnouncementQnAs.First(x => x.Id == announcementQnAId);
            IList<Notification> notifications = authors.Select(author => builder.BuildAnnouncementQnToAuthorNotifiaction(annQnA, ann, author)).ToList();
            AddNotifications(notifications);
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
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();


            var privateMessage = Storage.PrivateMessageStorage.GetDetailsById(privateMessageId, Context.PersonId.Value);
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
