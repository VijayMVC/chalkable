using System.Collections.Generic;
using System.Data.Common;
using System.Text;
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


        private DbQuery BuildGetMessagesQuery(IList<int> roles, string keyword, bool? read, int personId, bool isIncome, int schoolId)
        {

            var field1 = "PrivateMessage_FromPersonRef";
            var field2 = "PrivateMessage_DeletedBySender";
            var prefix = RECIPIENT_PREFIX;
            if (isIncome)
            {
                field1 = "PrivateMessage_ToPersonRef";
                field2 = "PrivateMessage_DeletedByRecipient";
                prefix = SENDER_PREFIX;
            }
            var conds = new Dictionary<string, object> { { "read", read }, { "personId", personId } };
            var b = new StringBuilder();
            b.AppendFormat(@"select vwPrivateMessage.* from vwPrivateMessage 
                             where {0} = @personId and {1} = 0", field1, field2);
            b.AppendFormat(" and PrivateMessage_SenderSchoolRef = @schoolId and PrivateMessage_RecipientSchoolRef = @schoolId");
            conds.Add("@schoolId", schoolId);
            if (isIncome && read.HasValue)
            {
                b.Append(" and PrivateMessage_Read = @read");
            }
            if (roles != null && roles.Count > 0)
            {
                var rolesString = roles.JoinString(",");
                b.AppendFormat(" and PrivateMessage_{1}RoleRef in ({0})", rolesString, prefix);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword + "%";
                b.AppendFormat(@" and (PrivateMessage_Subject like @keyword or PrivateMessage_Body like @keyword
                                        or lower(PrivateMessage_{0}FirstName) like @keyword or lower(PrivateMessage_{0}LastName) like @keyword)"
                                        , prefix);
                conds.Add("keyword", keyword);
            }
            return new DbQuery (b, conds);
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

        public PrivateMessageDetails GetDetailsById(int id, int callerId)
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

        public IList<PrivateMessage> GetNotDeleted(int callerId)
        {
            var sql = @"select * from PrivateMessage 
                        where (FromPersonRef = @callerId and DeletedBySender = 0) 
                               or (ToPersonRef = @callerId and DeletedByRecipient = 0)";
            var conds = new Dictionary<string, object> {{"callerId", callerId}};
            return ReadMany<PrivateMessage>(new DbQuery (sql, conds));
        } 

        public PaginatedList<PrivateMessageDetails> GetIncomeMessages(IList<int> roles, string keyword, bool? read,
                                                               int personId, int schoolId, int start, int count)
        {
            var query = BuildGetMessagesQuery(roles, keyword, read, personId, true, schoolId);
            return ReadPaginatedPrivateMessage(query, start, count);
        }
        public PaginatedList<PrivateMessageDetails> GetOutComeMessage(IList<int> roles, string keyword, int personId, int schoolId,
                                                               int start, int count)
        {
            var query = BuildGetMessagesQuery(roles, keyword, null, personId, false, schoolId);
            return ReadPaginatedPrivateMessage(query, start, count);
        } 

        private PaginatedList<PrivateMessageDetails> ReadPaginatedPrivateMessage(DbQuery query, int start, int count)
        {
            var orderBy = "PrivateMessage_Sent";
            query = Orm.PaginationSelect(query, orderBy, Orm.OrderType.Desc, start, count);
            return ReadPaginatedResult(query, start, count, ReadListPrivateMessageDetails);
        }
    }
}
