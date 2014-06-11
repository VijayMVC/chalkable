using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class NotificationsByDateViewData
    {
        public DateTime Created { get; set; }
        public IList<NotificationViewData> Notifications { get; set; }

        public static NotificationsByDateViewData Create(IList<NotificationDetails> notifications, DateTime date)
        {
            var res = new NotificationsByDateViewData
            {
                Created = date,
                Notifications = NotificationViewData.Create(notifications),
            };
            return res;
        }

        public static IList<NotificationsByDateViewData> Create(IDictionary<DateTime, List<NotificationDetails>> notificationsByDate)
        {
            var res = notificationsByDate.Select(x => Create(x.Value, x.Key));
            return res.ToList();
        }
    }
}