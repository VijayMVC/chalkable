using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School.Notifications
{
    public interface INotificationService
    {
        IList<Notification> GetUnshownNotifications();
        PaginatedList<NotificationDetails> GetNotifications(int start, int count);
        IList<Notification> GetNotificationsByTypes(int personId, IList<int> types, bool? wasSent = null);

        void AddAnnouncementNewAttachmentNotification(int announcementId);
        void AddAnnouncementNewAttachmentNotificationToTeachers(int announcementId, int fromPersonId);
        void AddAnnouncementNotificationQnToAuthor(int announcementQnAId, int announcementId);
        void AddAnnouncementNotificationAnswerToPerson(int announcementQnAId, int announcementId);
        void AddAnnouncementSetGradeNotificationToPerson(int announcement, int recipient);
        void AddPrivateMessageNotification(int privateMessageId);
        void AddApplicationNotification(IList<Person> toPerson, Person fromPerson, Guid applicationId);
        void AddAppBudgetBalanceNotification(int recipientId, double budgetBalance);
        void AddEndMarkingPeriodNotification(int toPersonId, int markingPeriodId, int endDays, bool isNextMpNotExist, bool isNextMpNotAssignedToClass);
        void AddAttendanceNotification(int toPersonId, IList<Person> persons);
        void AddAttendanceNotificationToStudent(int toPersonId, int classAttendanceId);
        void MarkAsShown(int[] notificationIds);
    }
    
    public class NotificationService : SchoolServiceBase, INotificationService
    {
        NotificationBuilder builder;

        public NotificationService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            builder = new NotificationBuilder(serviceLocator);
        }

        public IList<Notification> GetUnshownNotifications()
        {
            if(!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Read())
            {
                var da = new NotificationDataAccess(uow);
                return da.GetNotifications(new NotificationQuery
                    {
                        Shown = false,
                        PersonId = Context.PersonId, 
                        SchoolId = Context.SchoolLocalId.Value
                    });
            }
        }

        public PaginatedList<NotificationDetails> GetNotifications(int start, int count)
        {
            return GetNotifications(new NotificationQuery
                {
                    Start = start,
                    Count = count,
                });
        }

        private PaginatedList<NotificationDetails> GetNotifications(NotificationQuery query)
        {
            if (!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();

            using (var uow = Read())
            {
                query.SchoolId = Context.SchoolLocalId.Value;
                query.PersonId = Context.PersonId;
                var notifications = new NotificationDataAccess(uow).GetPaginatedNotificationsDetails(query);
                var classIds = notifications.Where(x => x.AnnouncementRef.HasValue && x.Announcement != null)
                                   .Select(x => x.Announcement.ClassRef)
                                   .ToList();
                IList<ClassAnnouncementType> classAnnouncementTypes = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classIds);
                foreach (var notification in notifications)
                {
                    if (notification.Announcement != null && notification.Announcement.ClassAnnouncementTypeRef.HasValue)
                    {
                        var classAnnType = classAnnouncementTypes.First(x => x.Id == notification.Announcement.ClassAnnouncementTypeRef);
                        notification.AnnouncementType = classAnnType;
                    }
                }
                return notifications;
            }
        } 

        
        public IList<Notification> GetNotificationsByTypes(int personId, IList<int> types, bool? wasSent = null)
        {
            throw new NotImplementedException();
        }

        public void MarkAsShown(int[] notificationIds)
        {
            if (!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();

            using (var uow = Update())
            {
                var da = new NotificationDataAccess(uow);
                var notifications = da.GetNotifications(new NotificationQuery
                        {
                            Shown = false,
                            PersonId = Context.PersonId,
                            SchoolId = Context.SchoolLocalId.Value
                        });
                foreach (var notificationId in notificationIds)
                {
                    var notification = notifications.FirstOrDefault(x => x.Id == notificationId);
                    if (notification == null) continue;
                    if (!NotificationSecurity.CanModify(notification, Context))
                        throw new ChalkableSecurityException();
                    notification.Shown = true;
                }
                da.Update(notifications);
                uow.Commit();
            }
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
            var teachers = ServiceLocator.StaffService.SearchStaff(null, announcement.ClassRef, null, null, false, 0, int.MaxValue);
            var persons = teachers.Select(x => ServiceLocator.PersonService.GetPerson(x.Id));
            var notification = persons.Select(x => builder.BuildAnnouncementNewAttachmentNotificationToPerson(announcement, x, fromPerson)).ToList();
            AddNotifications(notification);
        }


        public void AddAnnouncementNotificationQnToAuthor(int announcementQnAId, int announcementId)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var teachers = ServiceLocator.StaffService.SearchStaff(null, ann.ClassRef, null, null, false, 0, int.MaxValue);
            var authors = teachers.Select(x => ServiceLocator.PersonService.GetPerson(x.Id));
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
            using (var uow = Update())
            {
                new NotificationDataAccess(uow).Insert(notifications);
                uow.Commit();
            }
        }

        public void AddPrivateMessageNotification(int privateMessageId)
        {
            using (var uow = Update())
            {
                if (!Context.PersonId.HasValue)
                    throw new UnassignedUserException();

                var privateMessage = new PrivateMessageDataAccess(uow).GetDetailsById(privateMessageId, Context.PersonId.Value);
                var notification = builder.BuildPrivateMessageNotification(privateMessage, privateMessage.Sender, privateMessage.Recipient);
                new NotificationDataAccess(uow).Insert(notification);
                uow.Commit();
            }
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
