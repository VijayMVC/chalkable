﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Notifications
{
    public interface INotificationService
    {
        int GetUnshownNotificationsCount();
        IList<Notification> GetUnshownNotifications();
        PaginatedList<NotificationDetails> GetNotifications(int start, int count);
        IList<Notification> GetNotificationsByTypes(int personId, IList<int> types, bool? wasSent = null);

        void AddAnnouncementNewAttachmentNotification(int announcementId, AnnouncementType announcementType);
        void AddAnnouncementNewAttachmentNotificationToOwner(int announcementId, AnnouncementType announcementType, int fromPersonId);
        void AddAnnouncementNotificationQnToAuthor(int announcementQnAId, int announcementId, AnnouncementType announcementType);
        void AddAnnouncementNotificationAnswerToStudent(int announcementQnAId, int announcementId, AnnouncementType announcementType);
        void AddAnnouncementSetGradeNotificationToStudent(int announcement, int recipient);
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

        public int GetUnshownNotificationsCount()
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(uow => new NotificationDataAccess(uow).GetUnshownNotificationsCount(Context.PersonId.Value));
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
            Trace.Assert(Context.SchoolLocalId.HasValue);
            
            using (var uow = Read())
            {
                query.SchoolId = Context.SchoolLocalId.Value;
                query.PersonId = Context.PersonId;
                var notifications = new NotificationDataAccess(uow).GetPaginatedNotificationsDetails(query, !Context.MessagingDisabled);
                var classIds = notifications.Where(x => x.AnnouncementRef.HasValue && x.Announcement is ClassAnnouncement)
                                   .Select(x => (x.Announcement as ClassAnnouncement).ClassRef)
                                   .ToList();
                IList<ClassAnnouncementType> classAnnouncementTypes = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classIds);
                foreach (var notification in notifications)
                {
                    var classAnn = notification.Announcement as ClassAnnouncement;
                    if (classAnn != null && classAnn.ClassAnnouncementTypeRef.HasValue)
                    {
                        var classAnnType = classAnnouncementTypes.First(x => x.Id == classAnn.ClassAnnouncementTypeRef);
                        notification.ClassAnnouncementType = classAnnType;
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

        public void AddAnnouncementNewAttachmentNotification(int announcementId, AnnouncementType announcementType)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            var persons = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementRecipientPersons(announcementId);
            var notifications = new List<Notification>();
            foreach (var person in persons)
            {
                notifications.Add(builder.BuildAnnouncementNewAttachmentNotification(Context.NowSchoolTime, ann, person));
            }
            AddNotifications(notifications);
        }

        public void AddAnnouncementNewAttachmentNotificationToOwner(int announcementId, AnnouncementType announcementType, int fromPersonId)
        {
            List<Person> peopleToNotify = new List<Person>();

            var fromPerson = ServiceLocator.PersonService.GetPerson(fromPersonId);
            var announcement = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);

            if (announcement.AdminRef.HasValue)
            {
                var admin = ServiceLocator.PersonService.GetPerson(announcement.AdminRef.Value);
                admin.RoleRef = CoreRoles.DISTRICT_ADMIN_ROLE.Id;
                peopleToNotify.Add(admin);
            }
            else if(announcement.ClassRef.HasValue)
            {
                var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
                var teachers = ServiceLocator.StaffService.SearchStaff(syId, announcement.ClassRef, null, null, false, 0, int.MaxValue);
                peopleToNotify.AddRange(teachers.Select(x =>
                {
                    var res = ServiceLocator.PersonService.GetPerson(x.Id);
                    res.RoleRef = CoreRoles.TEACHER_ROLE.Id;
                    return res;
                })); 
            }


            var notification = peopleToNotify.Select(x => builder.BuildAnnouncementNewAttachmentNotificationToPerson(Context.NowSchoolTime, announcement, x, fromPerson)).ToList();
            AddNotifications(notification);
        }


        public void AddAnnouncementNotificationQnToAuthor(int announcementQnAId, int announcementId, AnnouncementType announcementType)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            IList<Person> authors = new List<Person>();
            int? classId = null, adminId = null;
            if (ann.LessonPlanData != null) classId = ann.LessonPlanData.ClassRef;
            if (ann.ClassAnnouncementData != null) classId = ann.ClassAnnouncementData.ClassRef;
            if (ann.AdminAnnouncementData != null) adminId = ann.AdminAnnouncementData.AdminRef;

            if (classId.HasValue)
            {
                var teachers = ServiceLocator.StaffService.SearchStaff(syId, classId.Value, null, null, false, 0,
                                                                       int.MaxValue);
                authors = teachers.Select(x =>
                    {
                        var res = ServiceLocator.PersonService.GetPerson(x.Id);
                        res.RoleRef = CoreRoles.TEACHER_ROLE.Id;
                        return res;
                    }).ToList();
            }
            else if(adminId.HasValue)
            {
                var admin = ServiceLocator.PersonService.GetPerson(adminId.Value);
                admin.RoleRef = CoreRoles.DISTRICT_ADMIN_ROLE.Id;
                authors = new List<Person> {admin};
            }
            var annQnA = ann.AnnouncementQnAs.First(x => x.Id == announcementQnAId);
            IList<Notification> notifications = authors.Select(author => builder.BuildAnnouncementQnToAuthorNotifiaction(Context.NowSchoolTime, annQnA, ann, author)).ToList();
            AddNotifications(notifications);
        }

        public void AddAnnouncementNotificationAnswerToStudent(int announcementQnAId, int announcementId, AnnouncementType announcementType)
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
            recipient.RoleRef = CoreRoles.STUDENT_ROLE.Id;
            var notification = builder.BuildAnnouncementSetGradeToStudentNotification(Context.NowSchoolTime, announcement, recipient);
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
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var msg = new PrivateMessageDataAccess(uow).GetSentPrivateMessage(privateMessageId, Context.PersonId.Value);
                var notifications = msg.RecipientPersons.Select(x=> builder.BuildPrivateMessageNotification(Context.NowSchoolTime, msg, msg.Sender, x)).ToList();
                new NotificationDataAccess(uow).Insert(notifications);
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
            var notification = builder.BuildEndMarkingPeriodNotification(Context.NowSchoolTime, markingPeriod, toSchoolPerson, endDays, 
                isNextMpNotExist, isNextMpNotAssignedToClass);
            AddNotification(notification);
        }

        public void AddAttendanceNotification(int toPersonId, IList<Person> persons)
        {
            //TODO: think about security
            var toSchoolPerson = ServiceLocator.PersonService.GetPerson(toPersonId);
            var notification = builder.BuildAttendanceNotificationToAdmin(Context.NowSchoolTime, toSchoolPerson, persons);
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
