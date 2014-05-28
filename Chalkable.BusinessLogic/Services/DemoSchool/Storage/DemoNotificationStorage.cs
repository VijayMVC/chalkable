using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoNotificationStorage:BaseDemoIntStorage<Notification>
    {
        public DemoNotificationStorage(DemoStorage storage) : base(storage, x => x.Id, true)
        {
        }

        public IList<Notification> GetNotifications(NotificationQuery notificationQuery)
        {
            var notifications = data.Select(x => x.Value);
            if (notificationQuery.Id.HasValue)
                notifications = notifications.Where(x => x.Id == notificationQuery.Id);
            if (notificationQuery.PersonId.HasValue)
                notifications = notifications.Where(x => x.PersonRef == notificationQuery.PersonId);
            if (notificationQuery.Shown.HasValue)
                notifications = notifications.Where(x => x.Shown == notificationQuery.Shown);

            if (notificationQuery.Type.HasValue)
                notifications = notifications.Where(x => x.Type == notificationQuery.Type);
            notifications = notifications.Skip(notificationQuery.Start).Take(notificationQuery.Count);
            return notifications.ToList();
        }

        public PaginatedList<NotificationDetails> GetPaginatedNotificationsDetails(NotificationQuery notificationQuery)
        {
            
            var notifications = GetNotifications(notificationQuery);
            var nfDetails = new List<NotificationDetails>();
            foreach (var notification in notifications)
            {
                var notificationDetails = (NotificationDetails) notification;

                notificationDetails.Person = Storage.PersonStorage.GetById(notificationDetails.PersonRef);

                if (notificationDetails.AnnouncementRef.HasValue)
                    notificationDetails.Announcement = Storage.AnnouncementStorage.GetById(notificationDetails.AnnouncementRef.Value);



                if (notificationDetails.PrivateMessageRef.HasValue)
                    notificationDetails.PrivateMessage =
                        Storage.PrivateMessageStorage.GetDetailsById(notificationDetails.PrivateMessageRef.Value, Storage.Context.UserLocalId.Value);

                if (notificationDetails.QuestionPersonRef.HasValue)
                    notificationDetails.QuestionPerson = Storage.PersonStorage.GetById(notificationDetails.QuestionPersonRef.Value);

                if (notificationDetails.MarkingPeriodRef.HasValue)
                    notificationDetails.MarkingPeriod =
                        Storage.MarkingPeriodStorage.GetById(notificationDetails.MarkingPeriodRef.Value);
                
                nfDetails.Add(notificationDetails);

            }
            return new PaginatedList<NotificationDetails>(nfDetails, notificationQuery.Start / notificationQuery.Count, notificationQuery.Count, data.Count);
        }

        public override void Setup()
        {
        }
    }
}
