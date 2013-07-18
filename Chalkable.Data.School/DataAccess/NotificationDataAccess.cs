using System;
using System.Collections.Generic;
using System.Data.Common;
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

        private DbQuery BuildGetNotificationDetailsDbQuery(NotificationQuery query)
        {
            var conds = BuildConditions(query);
            //TODO: think how to rewrtite this
            var sql = @"select {0}, 
                               vwPrivateMessage.*,
                               toPerson.FirstName as ToPerson_FirstName,
                               toPerson.LastName as ToPerson_LastName,
                               toPerson.Gender as ToPerson_Gender,
                               toPerson.Salutation as ToPerson_Salutation,
                               toPerson.RoleRef as ToPerson_RoleRef,
                               quetionPerson.FirstName as QuetionPerson_FirstName,
                               quetionPerson.LastName as QuetionPerson_LastName,
                               quetionPerson.Gender as QuetionPerson_Gender,
                               quetionPerson.Salutation as QuetionPerson_Salutation,
                               quetionPerson.RoleRef as QuetionPerson_RoleRef
                        from [Notification]
                        left join Announcement on Announcement.Id  = [Notification].AnnouncementRef
                        left join AnnouncementType on AnnouncementType.Id = Announcement.AnnouncementTypeRef
                        left join MarkingPeriod on MarkingPeriod.Id = [Notification].MarkingPeriodRef
                        left join ClassPeriod on ClassPeriod.Id = [Notification].ClassPeriodRef
                        left join vwPrivateMessage on vwPrivateMessage.PrivateMessage_Id = [Notification].PrivateMessageRef
                        join Person toPerson on toPerson.Id = [Notification].PersonRef
                        left join Person quetionPerson on quetionPerson.Id = [Notification].QuestionPersonRef";

            var b = new StringBuilder();
            var tables = new List<Type>
                {
                    typeof (Notification),
                    typeof (Announcement),
                    typeof (AnnouncementType),
                    typeof (MarkingPeriod),
                    typeof (ClassPeriod)
                };
            b.AppendFormat(sql, Orm.ComplexResultSetQuery(tables));
            b = Orm.BuildSqlWhere(b, tables[0], conds);
            return new DbQuery {Parameters = conds, Sql = b.ToString()};
        }

        private IList<NotificationDetails> ReadListNotifcationDetails(DbDataReader reader)
        {
            var res = new List<NotificationDetails>();
            while (reader.Read())
            {
                res.Add(ReadNotificationDetails(reader));
            }
            return res;
        }
        private NotificationDetails ReadNotificationDetails(DbDataReader reader)
        {
            var res = reader.Read<NotificationDetails>(true);
            res.PrivateMessage = PrivateMessageDataAccess.ReadPrivateMessageDetails(reader);
            res.Person = ReadNotificationPerson(reader, "ToPerson");
            res.QuestionPerson = ReadNotificationPerson(reader, "QuetionPerson");
            return res;
        }
        private Person ReadNotificationPerson(DbDataReader reader, string prefix)
        {
            var template = prefix + "_{0}";
            return new Person
                {
                    FirstName = SqlTools.ReadStringNull(reader, string.Format(template, "FirstName")),
                    LastName = SqlTools.ReadStringNull(reader, string.Format(template, "LastName")),
                    RoleRef = SqlTools.ReadInt32(reader, string.Format(template, "RoleRef")),
                    Gender = SqlTools.ReadStringNull(reader, string.Format(template, "Gender")),
                    Salutation = SqlTools.ReadStringNull(reader, string.Format(template, "Salutation")),
                };
        }

        public IList<NotificationDetails> GetNotificationsDetails(NotificationQuery query)
        {
            var q = BuildGetNotificationDetailsDbQuery(query);
            using (var reader = ExecuteReaderParametrized(q.Sql, q.Parameters))
            {
                return ReadListNotifcationDetails(reader);
            }
        }

        public PaginatedList<NotificationDetails> GetPaginatedNotificationsDetails(NotificationQuery query)
        {
            var innerQuery = BuildGetNotificationDetailsDbQuery(query);
            var orderBy = "Notification_" + Notification.CREATED_FIELD;
            var q = Orm.PaginationSelect(innerQuery, orderBy, Orm.OrderType.Asc, query.Start, query.Count);
            return ReadPaginatedResult(q, query.Start, query.Count, ReadListNotifcationDetails);
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

