﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School.Notifications
{
    public interface INotificationService
    {
        IList<NotificationDetails> GetUnshownNotifications();
        PaginatedList<NotificationDetails> GetNotifications(int start, int count);
        IList<Notification> GetNotificationsByTypes(Guid personId, IList<int> types, bool? wasSent = null);

        void AddAnnouncementNewAttachmentNotification(Guid announcementId);
        void AddAnnouncementNewAttachmentNotificationToPerson(Guid announcementId, Guid fromPersonId);
        void AddAnnouncementReminderNotification(AnnouncementReminder announcementReminder);
        void AddAnnouncementNotificationQnToAuthor(AnnouncementQnAComplex announcementQnA, AnnouncementComplex announcement);
        void AddAnnouncementNotificationAnswerToPerson(AnnouncementQnAComplex announcementQnA, AnnouncementComplex announcement);
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

        public IList<NotificationDetails> GetUnshownNotifications()
        {
            using (var uow = Read())
            {
                var da = new NotificationDataAccess(uow);
                return da.GetNotificationsDetails(new NotificationQuery {Shown = false, PersonId = Context.UserId});
            }
        }

        public PaginatedList<NotificationDetails> GetNotifications(int start, int count)
        {
            using (var uow = Read())
            {
                var da = new NotificationDataAccess(uow);
                return da.GetPaginatedNotificationsDetails(new NotificationQuery
                    {
                        PersonId = Context.UserId, 
                        Start = start, 
                        Count = count
                    });
            }
        }

        public IList<Notification> GetNotificationsByTypes(Guid personId, IList<int> types, bool? wasSent = null)
        {
            throw new NotImplementedException();
        }

        public void MarkAsShown(int[] notificationIds)
        {
            using (var uow = Update())
            {
                var da = new NotificationDataAccess(uow);
                var notifications = da.GetNotifications(new NotificationQuery {Shown = false, PersonId = Context.UserId});
                foreach (var notification in notifications)
                {
                    if(!NotificationSecurity.CanModify(notification, Context))
                        throw new ChalkableSecurityException();
                    notification.Shown = true;
                }
                da.Update(notifications);
                uow.Commit();
            }
        }

        public void AddAnnouncementNewAttachmentNotification(Guid announcementId)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            //var persons = ServiceLocator.AnnouncementService.
            //builder.BuildAnnouncementNewAttachmentNotification(announcementId)
            throw new NotImplementedException();
        }

        public void AddAnnouncementNewAttachmentNotificationToPerson(Guid announcementId, Guid fromPersonId)
        {
            var announcement = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var fromPerson = ServiceLocator.PersonService.GetPerson(fromPersonId);
            var notification = builder.BuildAnnouncementNewAttachmentNotificationToPerson(announcement, fromPerson);
            AddNotification(notification);
        }

        public void AddAnnouncementReminderNotification(AnnouncementReminder announcementReminder)
        {

            //var announcement = announcementReminder.Announcement;
            //if (!NotificationSecurity.CanCreateAnnouncementNotification(announcement, Context))
            //    throw new SecurityException();
            //var recipients = ServiceLocator.AnnouncementService.GetAnnouncementRecipientPersons(announcement.Id);
            //foreach (var schoolPerson in recipients)
            //{
            //    if (!announcementReminder.SchoolPersonRef.HasValue || schoolPerson.Id == announcementReminder.SchoolPersonRef)
            //    {
            //        var notification = builder.BuildAnnouncementReminderNotification(announcement, schoolPerson);
            //        Entities.Notifications.AddObject(notification);
            //    }
            //}
            //Entities.SaveChanges();

            throw new NotImplementedException();
        }

        public void AddAnnouncementNotificationQnToAuthor(AnnouncementQnAComplex announcementQnA, AnnouncementComplex announcement)
        {
            var notification = builder.BuildAnnouncementQnToAuthorNotifiaction(announcementQnA, announcement, announcementQnA.Answerer);
            AddNotification(notification);
        }

        public void AddAnnouncementNotificationAnswerToPerson(AnnouncementQnAComplex announcementQnA, AnnouncementComplex announcement)
        {
            var notification = builder.BuildAnnouncementAnswerToPersonNotifiaction(announcementQnA, announcement, announcementQnA.Answerer);
            AddNotification(notification);
        }

        public void AddAnnouncementSetGradeNotificationToPerson(Guid announcementId, Guid recipientId)
        {
            var announcement = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var recipient = ServiceLocator.PersonService.GetPerson(recipientId);
            var notification = builder.BuildAnnouncementSetGradeToPersonNotifiaction(announcement, recipient);
            AddNotification(notification);
        }

        private void AddNotification(Notification notification)
        {
            using (var uow = Update())
            {
                new NotificationDataAccess(uow).Insert(notification);
                uow.Commit();
            }
        }

        public void AddPrivateMessageNotification(Guid privateMessageId)
        {
            using (var uow = Update())
            {
                var privateMessage = new PrivateMessageDataAccess(uow).GetDetailsById(privateMessageId, Context.UserId);
                var notification = builder.BuildPrivateMessageNotification(privateMessage, privateMessage.Sender, privateMessage.Recipient);
                new NotificationDataAccess(uow).Insert(notification);
                uow.Commit();
            }
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
            var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            var toSchoolPerson = ServiceLocator.PersonService.GetPerson(toPersonId);
            var notification = builder.BuildEndMarkingPeriodNotification(markingPeriod, toSchoolPerson, endDays, isNextMpNotExist, isNextMpNotAssignedToClass);
            AddNotification(notification);
        }

        public void AddAttendanceNotification(Guid toPersonId, IList<Person> persons)
        {
            //TODO: think about security
            var toSchoolPerson = ServiceLocator.PersonService.GetPerson(toPersonId);
            var notification = builder.BuildAttendanceNotificationToAdmin(toSchoolPerson, persons);
            AddNotification(notification);
        }

        public void AddAttendanceNotificationToStudent(Guid toPersonId, Guid classAttendanceId)
        {
            var recipient = ServiceLocator.PersonService.GetPerson(toPersonId);
            var classAtt = ServiceLocator.AttendanceService.GetClassAttendanceComplexById(classAttendanceId);
            var notification = builder.BuildAttendanceNotificationToStudent(recipient, classAtt);
            AddNotification(notification);
        }

        public void AddAttendanceNotificationToTeacher(Guid toPersonId, ClassPeriod classPeriod, DateTime dateTime)
        {
            var recipient = ServiceLocator.PersonService.GetPerson(toPersonId);
            var classComplex = ServiceLocator.ClassService.GetClassById(classPeriod.ClassRef);
            var notification = builder.BuildAttendanceNotificationToTeacher(recipient, classPeriod, classComplex.Name, dateTime);
            AddNotification(notification);
        }


    }
}
