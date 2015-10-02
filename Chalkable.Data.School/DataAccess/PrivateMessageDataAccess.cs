using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PrivateMessageDataAccess : DataAccessBase<PrivateMessage, int>
    {
        public PrivateMessageDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        private const string SENDER_PREFIX = "Sender";
        private const string RECIPIENT_PREFIX = "Recipient";
        
        public IList<PrivateMessage> GetNotDeleted(IList<int> ids, int callerId)
        {
            var conds = new AndQueryCondition
            {
                {PrivateMessage.FROM_PERSON_REF_FIELD, callerId},
                {PrivateMessage.DELETED_BY_SENDER_FIELD, false}
            };
            var dbQuery = Orm.SimpleSelect<PrivateMessageRecipient>(conds);
            var messagesIdsStr = ids.JoinString(",");
            dbQuery.Sql.AppendFormat($" And {PrivateMessage.ID_FIELD} in ({messagesIdsStr})");
            return ReadMany<PrivateMessage>(dbQuery);
        } 

        public PaginatedList<IncomePrivateMessage> GetIncomeMessages(int personId, int? messageId, IList<int> roles, string keyword, bool? read,  int start, int count)
        {
            var dbQuery = BuildIncomeMessageQuery();
            var conds = new AndQueryCondition { {PrivateMessageRecipient.REPICENT_REF_FIELD, personId}};
            if(read.HasValue)
                conds.Add(PrivateMessageRecipient.READ_FIELD, read);
            if(messageId.HasValue)
                conds.Add(PrivateMessageRecipient.PRIVATE_MESSAGE_REF_FIELD, messageId);
            conds.BuildSqlWhere(dbQuery, nameof(PrivateMessageRecipient));

            if (roles != null && roles.Count > 0)
                dbQuery.Sql.Append($" And {Person.ROLE_REF_FIELD} in ({roles.JoinString(",")})");

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword + "%";
                var paramName = "@keyword";
                dbQuery.Sql.Append($@" And ({PrivateMessage.SUBJECT_FIELD} like {paramName} Or {PrivateMessage.BODY_FIELD} like {paramName}
                                        or Lower({Person.FIRST_NAME_FIELD}) like {paramName} Or Lower({Person.LAST_NAME_FIELD}) like {paramName})");
                dbQuery.Parameters.Add(paramName, keyword);
            }
            
            dbQuery = Orm.PaginationSelect(dbQuery, PrivateMessage.SENT_FIELD, Orm.OrderType.Desc, start, count);
            return ReadPaginatedResult(dbQuery, start, count, r=> ReadList(r, ReadIncomePrivateMessage));
        }


        public IncomePrivateMessage GetIncomePrivateMessage(int id, int callerId)
        {
            return GetIncomeMessages(callerId, id, null, null, null, 0, 1).First();
        }

        public SentPrivateMessage GetSentPrivateMessage(int id, int callerId)
        {
            return GetSentMessages(callerId, id, null, null, 0, 1).First();
        }

        public PaginatedList<SentPrivateMessage> GetSentMessages(int personId, int? messageId, IList<int> roles, string keyword,
                                                               int start, int count)
        {
            //var query = BuildGetMessagesQuery(roles, keyword, null, personId, false, schoolId);
            //return ReadPaginatedPrivateMessage(query, start, count);
            throw new NotImplementedException();
        }

        private DbQuery BuildSentMessageQuery()
        {

            var dbQuery = new DbQuery();
            var privateMsgT = typeof (PrivateMessage);
            var privateMessageRecipientT = typeof(PrivateMessageRecipient);
            dbQuery.Sql.Append("Select ").Append(" {0} ")
                .Append($" From {privateMsgT.Name} ")
                .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, Person.VW_PERSON, Person.ID_FIELD, privateMsgT.Name, PrivateMessage.FROM_PERSON_REF_FIELD)
                .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, privateMessageRecipientT.Name, PrivateMessageRecipient.PRIVATE_MESSAGE_REF_FIELD, privateMsgT.Name, PrivateMessage.ID_FIELD);
                       
            throw new NotImplementedException();
            //var msgQuery = new DbQuery();
            //var msgRecipientQuery = new DbQuery();
            //var privateMsgT = nameof(PrivateMessage);
            //msgQuery.Sql.Append($"Select {privateMsgT}.* From {privateMsgT}");
            //msgRecipientQuery.Sql.Append($"Select ")
        }

        private DbQuery BuildIncomeMessageQuery()
        {
            var dbQuery = new DbQuery();
            var privateMessageT = typeof(PrivateMessage);
            var privateMessageRecipientT = typeof(PrivateMessageRecipient);
            dbQuery.Sql.Append($@"Select distinct {privateMessageT.Name}.*, 
                                         [{PrivateMessageRecipient.READ_FIELD}], 
                                         {PrivateMessageRecipient.DELETED_BY_RECIPIENT_FIELD},
                                         {Person.VW_PERSON}.{Person.ID_FIELD} as {SENDER_PREFIX}{Person.ID_FIELD},
                                         {Person.VW_PERSON}.{Person.FIRST_NAME_FIELD} as {SENDER_PREFIX}{Person.FIRST_NAME_FIELD},
                                         {Person.VW_PERSON}.{Person.LAST_NAME_FIELD} as {SENDER_PREFIX}{Person.LAST_NAME_FIELD},
                                         {Person.VW_PERSON}.{Person.SALUTATION_FIELD} as {SENDER_PREFIX}{Person.SALUTATION_FIELD},
                                         {Person.VW_PERSON}.{Person.ROLE_REF_FIELD} as {SENDER_PREFIX}{Person.ROLE_REF_FIELD},
                                         {Person.VW_PERSON}.{Person.GENDER_FIELD} as {SENDER_PREFIX}{Person.GENDER_FIELD}
                                  ")
                .Append($" From {privateMessageT.Name} ")
                .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, Person.VW_PERSON, Person.ID_FIELD, privateMessageT.Name, PrivateMessage.FROM_PERSON_REF_FIELD)
                .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, privateMessageRecipientT.Name, PrivateMessageRecipient.PRIVATE_MESSAGE_REF_FIELD, privateMessageT.Name, PrivateMessage.ID_FIELD);
            return dbQuery;
        }

        public static IncomePrivateMessage ReadIncomePrivateMessage(DbDataReader reader)
        {
            var res = reader.Read<IncomePrivateMessage>();
            res.Sender = ReadPrivateMessagePerson(reader, true);
            return res;
        }

        private static IList<T> ReadList<T>(DbDataReader reader, Func<DbDataReader, T> readItemAction) 
        {
            var res = new List<T>();
            while (reader.Read()) res.Add(readItemAction(reader));
            return res;
        } 
        

        private static Person ReadPrivateMessagePerson(DbDataReader reader, bool isSender)
        {
            var template = (isSender ? SENDER_PREFIX : RECIPIENT_PREFIX) + "{0}";
            return new Person
            {
                Id = SqlTools.ReadInt32(reader, string.Format(template, Person.ID_FIELD)),
                FirstName = SqlTools.ReadStringNull(reader, string.Format(template, Person.FIRST_NAME_FIELD)),
                LastName = SqlTools.ReadStringNull(reader, string.Format(template, Person.LAST_NAME_FIELD)),
                Gender = SqlTools.ReadStringNull(reader, string.Format(template, Person.GENDER_FIELD)),
                Salutation = SqlTools.ReadStringNull(reader, string.Format(template, Person.SALUTATION_FIELD)),
                RoleRef = SqlTools.ReadInt32(reader, string.Format(template, Person.ROLE_REF_FIELD))
            };
        }
        
    }



    public class PrivateMessageRecipientDataAccess : DataAccessBase<PrivateMessageRecipient, int>
    {
        public PrivateMessageRecipientDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<PrivateMessageRecipient> GetNotDelatedMessageRecpients(IList<int> messagesIds, int callerId)
        {
            var conds = new AndQueryCondition
            {
                {PrivateMessageRecipient.REPICENT_REF_FIELD, callerId},
                {PrivateMessageRecipient.DELETED_BY_RECIPIENT_FIELD, false}
            };
            var dbQuery = Orm.SimpleSelect<PrivateMessageRecipient>(conds);
            var messagesIdsStr = messagesIds.JoinString(",");
            dbQuery.Sql.AppendFormat($" And {PrivateMessageRecipient.PRIVATE_MESSAGE_REF_FIELD} in ({messagesIdsStr})");
            return ReadMany<PrivateMessageRecipient>(dbQuery);
        } 
    }
}
