﻿using System;
using System.Collections.Generic;
using System.Data.Common;
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

        private const string SENDER_PREFIX = "Sender";
        private const string RECIPIENT_PREFIX = "Recipient";
    

        private DbQuery BuildGetMessagesQuery(IList<int> roles, string keyword, bool? read, Guid personId, bool isIncome)
        {

            var field1 = "PrivateMessage_FromPersonRef";
            var field2 = "PrivateMessage_DeletedBySender";
            var prefix = SENDER_PREFIX;
            if (isIncome)
            {
                field1 = "PrivateMessage_ToPersonRef";
                field2 = "PrivateMessage_DeletedByRecipient";
                prefix = RECIPIENT_PREFIX;
            }
            var conds = new Dictionary<string, object> { { "read", read }, { "personId", personId } };
            var b = new StringBuilder();
            b.AppendFormat(@"select vwPrivateMessage.* from vwPrivateMessage 
                             where {0} = @personId and {1} = 0", field1, field2);

            if (isIncome && read.HasValue)
            {
                b.Append(" and PrivateMessage_Read = @read");
            }
            if (roles != null && roles.Count > 0)
            {
                var rolesString = roles.JoinString(",");
                b.AppendFormat("PrivateMessage_{1}RoleId in ({0})", rolesString, prefix);
            }
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword + "%";
                b.AppendFormat(@" and (PrivateMessage_Subject like @keyword or PrivateMessage_Body like @keyword
                                        or lower(PrivateMessage_{0}FirstName) like @keyword or lower(PrivateMessage_{0}LastName) like @keyword)"
                                        , prefix);
                conds.Add("keyword", keyword);
            }
            return new DbQuery {Parameters = conds, Sql = b.ToString()};
        }

        public static IList<PrivateMessageDetails> ReadListPrivateMessageDetails(DbDataReader reader)
        {
            var res = new List<PrivateMessageDetails>();
            while (reader.Read())
            {
                res.Add(ReadPrivateMessageDetails(reader));
            }
            return res;
        } 
        public static PrivateMessageDetails ReadPrivateMessageDetails(DbDataReader reader)
        {
            var res = reader.Read<PrivateMessageDetails>(true);
            res.Sender = ReadPrivateMessagePerson(reader, true);
            res.Recipient = ReadPrivateMessagePerson(reader, false);
            return res;
        }
        private static Person ReadPrivateMessagePerson(DbDataReader reader, bool isSender)
        {
            var template = "PrivateMessage_" + (isSender ? SENDER_PREFIX : RECIPIENT_PREFIX) + "{0}";
            return new Person
                {
                    FirstName = SqlTools.ReadStringNull(reader, string.Format(template, Person.FIRST_NAME_FIELD)),
                    LastName = SqlTools.ReadStringNull(reader, string.Format(template, Person.LAST_NAME_FIELD)),
                    Gender = SqlTools.ReadStringNull(reader, string.Format(template, Person.GENDER_FIELD)),
                    Salutation = SqlTools.ReadStringNull(reader, string.Format(template, Person.SALUTATION_FIELD)),
                    RoleRef = SqlTools.ReadInt32(reader, string.Format(template, Person.ROLE_REF_FIELD))
                };
        }

        public PrivateMessageDetails GetDetailsById(Guid id, Guid callerId)
        {
            var sql = @"select * from vwPrivateMessage where PrivateMessage_Id = @Id 
                        and (PrivateMessage_FromPersonRef = @callerId or PrivateMessage_ToPersonRef = @callerId)";
            var conds = new Dictionary<string, object> {{"Id", id}, {"callerId", callerId}};
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                reader.Read();
                return ReadPrivateMessageDetails(reader);
            }
        }

        public IList<PrivateMessage> GetNotDeleted(Guid callerId)
        {
            var sql = @"select * from PrivateMessage 
                        where (FromPersonRef = @callerId and DeletedBySender = 0) 
                               or (ToPersonRef = @callerId and DeletedByRecipient = 0)";
            var conds = new Dictionary<string, object> {{"callerId", callerId}};
            return ReadMany<PrivateMessage>(new DbQuery {Sql = sql, Parameters = conds});
        } 

        public PaginatedList<PrivateMessageDetails> GetIncomeMessages(IList<int> roles, string keyword, bool? read,
                                                               Guid personId, int start, int count)
        {
            var query = BuildGetMessagesQuery(roles, keyword, read, personId, true);
            return ReadPaginatedPrivateMessage(query, start, count);
        }
        public PaginatedList<PrivateMessageDetails> GetOutComeMessage(IList<int> roles, string keyword, Guid personId,
                                                               int start, int count)
        {
            var query = BuildGetMessagesQuery(roles, keyword, null, personId, false);
            return ReadPaginatedPrivateMessage(query, start, count);
        } 

        private PaginatedList<PrivateMessageDetails> ReadPaginatedPrivateMessage(DbQuery query, int start, int count)
        {
            var orderBy = "PrivateMessage_Id";
            query = Orm.PaginationSelect(query, orderBy, Orm.OrderType.Desc, start, count);
            return ReadPaginatedResult(query, start, count, ReadListPrivateMessageDetails);
        }
    }
}
