using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Services.Client;
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
            var dbQuery = Orm.SimpleSelect<PrivateMessage>(conds);
            var messagesIdsStr = ids.JoinString(",");
            dbQuery.Sql.AppendFormat($" And {PrivateMessage.ID_FIELD} in ({messagesIdsStr})");
            return ReadMany<PrivateMessage>(dbQuery);
        } 

        private const string SP_GET_INCOME_MESSAGES = "spGetIncomeMessages";
        private const string SP_GET_INCOME_MESSAGE_BY_ID = "spGetIncomeMessageById";

        public PaginatedList<IncomePrivateMessage> GetIncomeMessages(int personId, IList<int> roles, string keyword, bool? read, int start, int count, DateTime? fromDate, DateTime? toDate)
        {
            var param = new Dictionary<string, object>()
            {
                ["personId"] = personId,
                ["roles"] = roles ?? new List<int>(),
                ["filter"] = keyword,
                ["read"] = read,
                ["start"] = start,
                ["count"] = count,
                ["fromDate"] = fromDate,
                ["toDate"] = toDate
            };
            return ExecuteStoredProcedurePaginated(SP_GET_INCOME_MESSAGES, param, r => ReadList(r, ReadIncomePrivateMessage), start, count);
        }

        public IncomePrivateMessage GetIncomePrivateMessage(int id, int callerId)
        {
            var param = new Dictionary<string, object>()
            {
                ["personId"] = callerId,
                ["messageId"] = id
            };
            using (var reader = ExecuteStoredProcedureReader(SP_GET_INCOME_MESSAGE_BY_ID, param))
            {
                if (reader.Read())
                {
                    var res = ReadIncomePrivateMessage(reader);
                    return res;
                }
                return null;
            }
        }

        private const string SP_GET_SENT_MESSAGE_BY_ID = "spGetSentMessageById";

        public SentPrivateMessage GetSentPrivateMessage(int id, int callerId)
        {
            var param = new Dictionary<string, object>()
            {
                ["personId"] = callerId,
                ["messageId"] = id
            };
            using (var reader = ExecuteStoredProcedureReader(SP_GET_SENT_MESSAGE_BY_ID, param))
            {
                    var res = ReadSentMessages(reader);
                    return res.FirstOrDefault();
            }
        }

        private const string SP_GET_SENT_PRIVATE_MESSAGES = "spGetSentMessages";
        public PaginatedList<SentPrivateMessage> GetSentMessages(int callerId, IList<int> roles, string keyword, int start, int count, bool? classOnly, DateTime? fromDate, DateTime? toDate)
        {
            var param = new Dictionary<string, object>()
            {
                ["personId"] = callerId,
                ["roles"] = roles ?? new List<int>(),
                ["filter"] = keyword,
                ["start"] = start,
                ["count"] = count,
                ["classOnly"] = classOnly,
                ["fromDate"] = fromDate,
                ["toDate"] = toDate
            };
            return ExecuteStoredProcedurePaginated(SP_GET_SENT_PRIVATE_MESSAGES, param, ReadSentMessages, start, count);
        }



        private IList<SentPrivateMessage> ReadSentMessages(DbDataReader reader)
        {
            var res = reader.ReadList<SentPrivateMessage>();
            reader.NextResult();
            while (reader.Read())
            {
                var messageRecipient = reader.Read<PrivateMessageRecipient>();
                var message = res.FirstOrDefault(x => x.Id == messageRecipient.PrivateMessageRef);
                if(message == null) continue;
                if (message.RecipientPersons == null)
                {
                    message.RecipientPersons = new List<Person>();
                    message.Sender = ReadPrivateMessagePerson(reader, true);
                    if (messageRecipient.RecipientClassRef.HasValue)
                    {
                        message.RecipientClass = new Class
                        {
                            Id = messageRecipient.RecipientClassRef.Value,
                            Name = SqlTools.ReadStringNull(reader, Class.NAME_FIELD),
                            ClassNumber = SqlTools.ReadStringNull(reader, Class.CLASS_NUMBER_FIELD)
                        };
                    }
                }
                message.RecipientPersons.Add(ReadPrivateMessagePerson(reader, false));
            }
            return res;
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
                //Salutation = SqlTools.ReadStringNull(reader, string.Format(template, Person.SALUTATION_FIELD)),
                RoleRef = SqlTools.ReadInt32(reader, string.Format(template, Person.ROLE_REF_FIELD))
            };
        }


        public PossibleMessageRecipients GetPossibleMessageRecipients(int callerId, int callerRoleId, int schoolYearId, bool teacherMessagingEnabled, 
            bool teacherClassOnly, bool studentMessagingEnabled, bool studentClassmatesOnly, string filter1, string filter2, string filter3)
        {
            var parameters = new Dictionary<string, object>
            {
                { "callerId", callerId },
                { "callerRoleId", callerRoleId},
                { "schoolYearId", schoolYearId },

                {"filter1", !string.IsNullOrWhiteSpace(filter1) ? string.Format(FILTER_FORMAT,filter1) : null},
                {"filter2", !string.IsNullOrWhiteSpace(filter2) ? string.Format(FILTER_FORMAT,filter2) : null},
                {"filter3", !string.IsNullOrWhiteSpace(filter3) ? string.Format(FILTER_FORMAT,filter3) : null},

                { "teacherStudentMessagingEnabled", teacherMessagingEnabled },
                { "studentMessagingEnabled", studentMessagingEnabled },
                { "teacherClassOnly", teacherClassOnly },
                { "studentClassmatesOnly", studentClassmatesOnly }
            };
            using (var reader = ExecuteStoredProcedureReader("spGetPossibleMessageRecipients", parameters))
            {
                var res = new PossibleMessageRecipients();
                res.Persons = reader.ReadList<Person>();
                reader.NextResult();
                res.Classes = reader.ReadList<Class>();
                return res;
            }
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
