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
            throw new System.NotImplementedException();
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
