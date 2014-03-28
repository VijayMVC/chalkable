using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoNotificationStorage:BaseDemoStorage<int, Notification>
    {
        public DemoNotificationStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<Notification> GetNotifications(NotificationQuery notificationQuery)
        {
            throw new System.NotImplementedException();
        }

        public PaginatedList<NotificationDetails> GetPaginatedNotificationsDetails(NotificationQuery notificationQuery)
        {
            throw new System.NotImplementedException();
        }

        public void Update(IList<Notification> notifications)
        {
            throw new System.NotImplementedException();
        }

        public void Add(IList<Notification> notifications)
        {
            throw new System.NotImplementedException();
        }

        public void Add(Notification notifications)
        {
            throw new System.NotImplementedException();
        }
    }
}
