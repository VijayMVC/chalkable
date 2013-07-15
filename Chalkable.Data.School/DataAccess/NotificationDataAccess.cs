using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class NotificationDataAccess : DataAccessBase<Notification>
    {
        public NotificationDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private Dictionary<string, object> BuildConditions(NotificationQuery query)
        {
            var res = new Dictionary<string, object> {{"PersonRef", query.PersonId}};
            if (query.Id.HasValue)
                res.Add("Id", query.Id);
            if(query.Shown.HasValue)
                res.Add("Shown", query.Shown);
            return res;
        } 

        public IList<Notification> GetNotifications(NotificationQuery query)
        {
            var conds = BuildConditions(query);
            return SelectMany<Notification>(conds);
        }

        public IList<NotificationDetails> GetNotificationsDetails(NotificationQuery query)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<NotificationDetails> GetPaginatedNotificationsDetails(NotificationQuery query)
        {
            throw new NotImplementedException();
        }
    }

    public class NotificationQuery
    {
        public Guid? Id { get; set; }
        public Guid PersonId { get; set; }
        public bool? Shown { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }

        public NotificationQuery()
        {
            Start = 0;
            Count = int.MaxValue;
        }
    }
}

