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

            //TODO: return PrivateMessage with sender and recipient data 

            var field1 = "FromPersonRef";
            var field2 = "DeletedBySender";
            if (isIncome)
            {
                field1 = "ToPersonRef";
                field2 = "DeletedByRecipient";
            }
            var conds = new Dictionary<string, object> { { "read", read }, { "personId", personId } };
            var b = new StringBuilder();
            //var type = typeof (PrivateMessage);
            //var modelNames = new Dictionary<Type, string>()
            //    {
            //        {type, type.Name},
            //        {typeof (Person), "Sender"},
            //        {typeof (Person), "Recipient"}
            //    };
            //var complextResultSet = Orm.ComplexResultSetQuery(modelNames);
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
                keyword = "%" + keyword + "%";
                b.AppendFormat(@" and (PrivateMessage.Subject like {0} or PrivateMessage.Body like {0}
                                        or lower(Person.FirstName) like {0} or lower(Person.LastName) like {0})", keyword);
                conds.Add("keyword", keyword);
            }

            return new DbQuery {Parameters = conds, Sql = b.ToString()};
        }

        public PrivateMessageDetails GetDetailsById(Guid id, Guid callerId)
        {
            var sql = @"select * from PrivateMessage where PrivateMessage.Id = @Id 
                        and (FromPersonRef = @callerId or ToPersonRef = @callerId)";
            var conds = new Dictionary<string, object> {{"Id", id}, {"callerId", callerId}};
            return  ReadOne<PrivateMessageDetails>(new DbQuery {Sql = sql, Parameters = conds});
        }

        public PaginatedList<PrivateMessageDetails> GetIncomeMessages(IList<int> roles, string keyword, bool? read,
                                                               Guid personId, int start, int count)
        {
            var query = BuildGetMessagesQuery(roles, keyword, read, personId, true);
            return PaginatedSelect<PrivateMessageDetails>(query, "Id", start, count, Orm.OrderType.Desc);
        }

        public PaginatedList<PrivateMessageDetails> GetOutComeMessage(IList<int> roles, string keyword, Guid personId,
                                                               int start, int count)
        {
            var query = BuildGetMessagesQuery(roles, keyword, null, personId, false);
            return PaginatedSelect<PrivateMessageDetails>(query, "Id", start, count, Orm.OrderType.Desc);
        } 

    }
}
