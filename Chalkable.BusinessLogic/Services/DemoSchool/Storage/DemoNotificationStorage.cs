using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoNotificationStorage:BaseDemoStorage<int, Notification>
    {
        private int index = 0;

        public DemoNotificationStorage(DemoStorage storage) : base(storage)
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


            //todo: class period ref filter
            //if (notificationQuery.ClassPeriodRef.HasValue)
             //   notifications = notifications.Where(x => x.)
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


                //
                //if (notificationDetails.PrivateMessageRef.HasValue)
                //    notificationDetails.PrivateMessage = Storage.PrivateMessageStorage.GetDetailsById()

                if (notificationDetails.QuestionPersonRef.HasValue)
                    notificationDetails.QuestionPerson = Storage.PersonStorage.GetById(notificationDetails.QuestionPersonRef.Value);

                if (notificationDetails.MarkingPeriodRef.HasValue)
                    notificationDetails.MarkingPeriod =
                        Storage.MarkingPeriodStorage.GetById(notificationDetails.MarkingPeriodRef.Value);
                
                nfDetails.Add(notificationDetails);

            }
            return new PaginatedList<NotificationDetails>(nfDetails, 1, 10);
        }

        public void Update(IList<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                var item = data.First(x => x.Value == notification);
                data[item.Key] = notification;
            }
        }

        public void Add(IList<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                Add(notification);
            }
        }

        public void Add(Notification notification)
        {
            data.Add(index++ , notification);
        }
    }
}
