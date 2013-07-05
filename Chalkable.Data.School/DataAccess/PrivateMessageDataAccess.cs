using System;
using System.Collections.Generic;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PrivateMessageDataAccess : DataAccessBase<PrivateMessage>
    {
        public PrivateMessageDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private DbQuery BuildGetMessagesQuery(IList<int> roles, string keyword, bool? read, Guid personId, bool isIncome)
        {
            var field1 = "FromPersonRef";
            var field2 = "DeletedBySender";
            if (isIncome)
            {
                field1 = "ToPersonRef";
                field2 = "DeletedByRecipient";
            }

            var b = new StringBuilder();
            b.AppendFormat(@"select PrivateMessage.* from PrivateMessage 
                           join Person on Person.Id = PrivateMessage.{0}
                           where PrivateMessage.{0} = @personId and PrivateMessage.{1} = 0", field1, field2);

            if (isIncome && read.HasValue)
            {
                b.Append(" and Read = @read");
            }
            if (roles != null && roles.Count > 0)
            {
                var rolesString = roles.JoinString(",");
                b.AppendFormat("Person.RoleRef in ({0})", rolesString);
            }
            if (string.IsNullOrEmpty(keyword))
            {
                b.AppendFormat(@" and (PrivateMessage.Subject like '%{0}%' or PrivateMessage.Body like '%{0}%'
                                        or lower(Person.FirstName) like '%{0}%' or lower(Person.LastName) like '%{0}%')", keyword);
            }

            var conds = new Dictionary<string, object> {{"read", read}, {"personId", personId}};
            return new DbQuery {Parameters = conds, Sql = b.ToString()};
        }

        public PaginatedList<PrivateMessage> GetIncomeMessages(IList<int> roles, string keyword, bool? read,
                                                               Guid personId, int start, int count)
        {
            var query = BuildGetMessagesQuery(roles, keyword, read, personId, true);
            return PaginatedSelect<PrivateMessage>(query, "Id", start, count,  Orm.OrderType.Desc);
        }

        public PaginatedList<PrivateMessage> GetOutComeMessage(IList<int> roles, string keyword, Guid personId,
                                                               int start, int count)
        {
            var query = BuildGetMessagesQuery(roles, keyword, null, personId, false);
            return PaginatedSelect<PrivateMessage>(query, "Id", start, count, Orm.OrderType.Desc);
        } 

    }
}
