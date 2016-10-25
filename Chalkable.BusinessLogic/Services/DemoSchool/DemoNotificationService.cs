using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.BusinessLogic.Services.School.Notifications;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoNotificationStorage : BaseDemoIntStorage<Notification>
    {
        public DemoNotificationStorage()
            : base(x => x.Id, true)
        {

        }
    }

    public class DemoNotificationService : DemoSchoolServiceBase, INotificationService
    {
        private readonly NotificationBuilder builder;
        private DemoNotificationStorage NotificationStorage { get; set; }
        public DemoNotificationService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            NotificationStorage = new DemoNotificationStorage();
            builder = new NotificationBuilder(serviceLocator);
        }

        public int GetUnshownNotificationsCount()
        {
            return GetUnshownNotifications().Count;
        }

        public IList<Notification> GetUnshownNotifications()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            return GetNotifications(new NotificationQuery
            {
                Shown = false,
                PersonId = Context.PersonId.Value,
                RoleId = Context.RoleId
            });
        }

        public IList<Notification> GetNotifications(NotificationQuery notificationQuery)
        {
            var notifications = NotificationStorage.GetData().Select(x => x.Value);
            notifications = notifications.Where(x => x.PersonRef == notificationQuery.PersonId && x.RoleRef == notificationQuery.RoleId);

            if (notificationQuery.Id.HasValue)
                notifications = notifications.Where(x => x.Id == notificationQuery.Id);
            if (notificationQuery.Shown.HasValue)
                notifications = notifications.Where(x => x.Shown == notificationQuery.Shown);

            if (notificationQuery.FromDate.HasValue)
                notifications = notifications.Where(x => x.Created >= notificationQuery.FromDate.Value);
            if (notificationQuery.ToDate.HasValue)
                notifications = notifications.Where(x => x.Created <= notificationQuery.ToDate.Value);

            if (notificationQuery.Type.HasValue)
                notifications = notifications.Where(x => x.Type == notificationQuery.Type);
            notifications = notifications.Skip(notificationQuery.Start).Take(notificationQuery.Count).OrderByDescending(x => x.Created);
            return notifications.ToList();
        }

        public PaginatedList<NotificationDetails> GetNotifications(int start, int count)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            return GetPaginatedNotificationsDetails(new NotificationQuery
            {
                PersonId = Context.PersonId.Value,
                Start = start,
                Count = count,
                SchoolId = Context.SchoolLocalId.Value
            });
        }

        public PaginatedList<NotificationDetails> GetPaginatedNotificationsDetails(NotificationQuery notificationQuery)
        {

            var notifications = GetNotifications(notificationQuery);
            var nfDetails = new List<NotificationDetails>();
            foreach (var notification in notifications)
            {
                var notificationDetails = (NotificationDetails)notification;

                notificationDetails.Person = ServiceLocator.PersonService.GetPersonDetails(notificationDetails.PersonRef);

                //TODO: impl this later
                //if (notificationDetails.AnnouncementRef.HasValue)
                //    notificationDetails.Announcement = ServiceLocator.AnnouncementService.GetAnnouncementById(notificationDetails.AnnouncementRef.Value);



                //if (notificationDetails.PrivateMessageRef.HasValue)
                //    notificationDetails.PrivateMessage =
                //        ServiceLocator.PrivateMessageService.GetMessage(notificationDetails.PrivateMessageRef.Value);

                if (notificationDetails.QuestionPersonRef.HasValue)
                    notificationDetails.QuestionPerson = ServiceLocator.PersonService.GetPersonDetails(notificationDetails.QuestionPersonRef.Value);

                if (notificationDetails.MarkingPeriodRef.HasValue)
                    notificationDetails.MarkingPeriod =
                        ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(notificationDetails.MarkingPeriodRef.Value);

                nfDetails.Add(notificationDetails);

            }
            return new PaginatedList<NotificationDetails>(nfDetails, notificationQuery.Start / notificationQuery.Count, 
                notificationQuery.Count, NotificationStorage.GetData().Count);
        }

        public IList<Notification> GetNotificationsByTypes(int personId, IList<int> types, bool? wasSent = null)
        {
            throw new System.NotImplementedException();
        }

        public void AddReportProcessedNotification(int toPersonId, int roleId, string reportName, string reportAddress,
            string errorMessage, bool processingSucced)
        {
            throw new NotImplementedException();
        }

        public void MarkAsShown(int[] notificationIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            var notifications = GetUnshownNotifications();
            foreach (var notificationId in notificationIds)
            {
                var notification = notifications.FirstOrDefault(x => x.Id == notificationId);
                if (notification == null) continue;
                if (!NotificationSecurity.CanModify(notification, Context))
                    throw new ChalkableSecurityException();
                notification.Shown = true;
            }
            NotificationStorage.Update(notifications);
        }

        public void AddAnnouncementNewAttachmentNotification(int announcementId, AnnouncementTypeEnum announcementType)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            var persons = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementRecipientPersons(announcementId, StudentFilterEnum.All);
            var notifications = new List<Notification>();
            foreach (var person in persons)
            {
                notifications.Add(builder.BuildAnnouncementNewAttachmentNotification(Context.NowSchoolTime, ann, person));
            }
            AddNotifications(notifications);
        }

        public void AddAnnouncementNewAttachmentNotificationToOwner(int announcementId, AnnouncementTypeEnum announcementType, int fromPersonId)
        {
            throw new NotImplementedException();
            //var announcement = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            //var fromPerson = ServiceLocator.PersonService.GetPerson(fromPersonId);
            //var teachers = ServiceLocator.StaffService.SearchStaff(null, announcement.ClassRef, null, null, true, 0, int.MaxValue);
            //var authors = teachers.Select(x => ServiceLocator.PersonService.GetPerson(x.Id));
            //var notifications = authors.Select(x => builder.BuildAnnouncementNewAttachmentNotificationToPerson(Context.NowSchoolTime, announcement, x, fromPerson)).ToList();
            //AddNotifications(notifications);
        }


        public void AddAnnouncementNotificationQnToAuthor(int announcementQnAId, int announcementId, AnnouncementTypeEnum announcementType)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            throw new NotImplementedException();
            //var teachers = ServiceLocator.StaffService.SearchStaff(null, ann.ClassRef, null, null, true, 0, int.MaxValue);
            //var authors = teachers.Select(x => ServiceLocator.PersonService.GetPerson(x.Id));
            //var annQnA = ann.AnnouncementQnAs.First(x => x.Id == announcementQnAId);
            //IList<Notification> notifications = authors.Select(author => builder.BuildAnnouncementQnToAuthorNotifiaction(Context.NowSchoolTime, annQnA, ann, author)).ToList();
            //AddNotifications(notifications);
        }

        public void AddAnnouncementNotificationAnswerToStudent(int announcementQnAId, int announcementId, AnnouncementTypeEnum announcementType)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            var annQnA = ann.AnnouncementQnAs.First(x => x.Id == announcementQnAId);
            var notification = builder.BuildAnnouncementAnswerToPersonNotifiaction(Context.NowSchoolTime, annQnA, ann);
            AddNotification(notification);
        }

        public void AddAnnouncementSetGradeNotificationToStudent(int announcementId, int recipientId)
        {
            var announcement = ServiceLocator.ClassAnnouncementService.GetAnnouncementDetails(announcementId);
            var recipient = ServiceLocator.PersonService.GetPerson(recipientId);
            var notification = builder.BuildAnnouncementSetGradeToStudentNotification(Context.NowSchoolTime, announcement, recipient);
            AddNotification(notification);
        }

        public void AddPrivateMessageNotification(int privateMessageId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var privateMessage = ServiceLocator.PrivateMessageService.GetSentMessage(privateMessageId);
            var notifications = privateMessage.RecipientPersons.Select( x => builder.BuildPrivateMessageNotification(Context.NowSchoolTime, privateMessage, privateMessage.Sender, x));
            NotificationStorage.Add(notifications.ToList());
        }

        public void AddEndMarkingPeriodNotification(int toPersonId, int markingPeriodId, int endDays, bool isNextMpNotExist,
                                                    bool isNextMpNotAssignedToClass)
        {
            var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            var toSchoolPerson = ServiceLocator.PersonService.GetPerson(toPersonId);
            var notification = builder.BuildEndMarkingPeriodNotification(Context.NowSchoolTime, markingPeriod, toSchoolPerson, endDays, isNextMpNotExist, isNextMpNotAssignedToClass);
            AddNotification(notification);
        }

        public void AddAttendanceNotification(int toPersonId, IList<Person> persons)
        {
            var toSchoolPerson = ServiceLocator.PersonService.GetPerson(toPersonId);
            var notification = builder.BuildAttendanceNotificationToAdmin(Context.NowSchoolTime, toSchoolPerson, persons);
            AddNotification(notification);
        }

        public void AddAttendanceNotificationToStudent(int toPersonId, int classAttendanceId)
        {
            throw new NotImplementedException();
        }

        private void AddNotification(Notification notification)
        {
            AddNotifications(new List<Notification> { notification });
        }
        private void AddNotifications(IList<Notification> notifications)
        {
            NotificationStorage.Add(notifications);
        }
    }
}
