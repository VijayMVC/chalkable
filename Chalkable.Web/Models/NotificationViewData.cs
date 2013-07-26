using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class NotificationViewData
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public bool Shown { get; set; }
        public Guid? AnnouncementId { get; set; }
        public Guid? PrivateMessageId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public Guid? PersonId { get; set; }
        public string PersonDisplayName { get; set; }
        public Guid? ApplicationId { get; set; }
        public string ApplcicationName { get; set; }
        public string ApplicationIcon47Url { get; set; }
        public int? AnnouncementType { get; set; }
        public string AnnouncementTypeName { get; set; }
        public DateTime Created { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? ClassPeriodId { get; set; }

        //private NotificationViewData(){}

        public NotificationViewData(Notification notification)
        {
            Id = notification.Id;
            Type = (int) notification.Type;
            Message = notification.Message;
            Shown = notification.Shown;
            AnnouncementId = notification.AnnouncementRef;
            PrivateMessageId = notification.PrivateMessageRef;
            ApplicationId = notification.ApplicationRef;
            ClassPeriodId = notification.ClassPeriodRef;
            Created = notification.Created;
            MarkingPeriodId = notification.MarkingPeriodRef;
        }

        public static NotificationViewData Create(NotificationDetails notification)
        {
            return PrepareBuilders()[notification.Type].Build(notification);
        }

        public static IList<NotificationViewData> Create(IList<NotificationDetails> notifications)
        {
            Dictionary<NotificationType, INotificationViewBuilder> builders = PrepareBuilders();
            return notifications.Select(notification => builders[notification.Type].Build(notification)).ToList();
        }

        private static Dictionary<NotificationType, INotificationViewBuilder> PrepareBuilders()
        {
            var res = new Dictionary<NotificationType, INotificationViewBuilder>();
            res.Add(NotificationType.Simple, new SimpleNotificationBuilder(NotificationType.Simple));
            res.Add(NotificationType.Announcement, new SimpleNotificationBuilder(NotificationType.Announcement));
            res.Add(NotificationType.Message, new MessageNotificationBuilder());
            res.Add(NotificationType.Question, new QuestionNotificationBuilder());
            res.Add(NotificationType.ItemToGrade, new SimpleNotificationBuilder(NotificationType.ItemToGrade));
            res.Add(NotificationType.AppBudgetBallance, new SimpleNotificationBuilder(NotificationType.AppBudgetBallance));
            res.Add(NotificationType.Application, new SimpleNotificationBuilder(NotificationType.Application));
            res.Add(NotificationType.MarkingPeriodEnding, new SimpleNotificationBuilder(NotificationType.MarkingPeriodEnding));
            res.Add(NotificationType.Attendance, new SimpleNotificationBuilder(NotificationType.Attendance));
            res.Add(NotificationType.NoTakeAttendance, new SimpleNotificationBuilder(NotificationType.NoTakeAttendance));
            return res;
        }
    }

    public interface INotificationViewBuilder
    {
        NotificationViewData Build(NotificationDetails notification);
    }

    public class SimpleNotificationBuilder : INotificationViewBuilder
    {
        private readonly NotificationType type;
        public SimpleNotificationBuilder(NotificationType type)
        {
            this.type = type;
        }

        public NotificationViewData Build(NotificationDetails notification)
        {
            if (notification.Type != type)
                throw new ChalkableException(ChlkResources.ERR_INVALID_NOTIFICATION_BUILDER_FOR_TYPE);
            var res = new NotificationViewData(notification);
            
            //TODO: think about applications
            //if (notification.ApplicationRef.HasValue)
            //{
            //    res.ApplcicationName = notification.Application.Name;
            //    res.ApplicationIcon47Url = AppTools.BuildIconUrl(notification.Application, true);
            //}
            if (notification.AnnouncementRef.HasValue)
            {
                res.AnnouncementType = notification.AnnouncementType.Id;
                res.AnnouncementTypeName = notification.AnnouncementType.Name;
            }
            if (notification.ClassPeriodRef.HasValue)
            {
                res.ClassId = notification.ClassPeriod.ClassRef;
            }
            return res;
        }
    }

    public class QuestionNotificationBuilder : INotificationViewBuilder
    {
        public NotificationViewData Build(NotificationDetails notification)
        {
            if (notification.Type != NotificationType.Question)
                throw new ChalkableException(ChlkResources.ERR_INVALID_NOTIFICATION_BUILDER_FOR_TYPE);
            var res = new NotificationViewData(notification)
                {
                    AnnouncementType = notification.AnnouncementType.Id,
                    AnnouncementTypeName = notification.AnnouncementType.Name,
                    PersonId = notification.QuestionPersonRef,
                    PersonDisplayName = notification.QuestionPerson.FullName
                };
            return res;
        }
    }

    public class MessageNotificationBuilder : INotificationViewBuilder
    {
        public NotificationViewData Build(NotificationDetails notification)
        {
            if (notification.Type != NotificationType.Message)
                throw new ChalkableException(ChlkResources.ERR_INVALID_NOTIFICATION_BUILDER_FOR_TYPE);
            var res = new NotificationViewData(notification)
            {
                PersonId = notification.PrivateMessage.Sender.Id,
                PersonDisplayName = notification.PrivateMessage.Sender.FullName
            };
            return res;
        }
    }
}