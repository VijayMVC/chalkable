using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class NotificationDataAccess : DataAccessBase<Notification>
    {
        public NotificationDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private AndQueryCondition BuildShortConditions(NotificationQuery query)
        {
            var res = new AndQueryCondition { { Notification.PERSON_REF_FIELD, query.PersonId } };
            if (query.Id.HasValue)
                res.Add(Notification.ID_FIELD, query.Id);
            if(query.Shown.HasValue)
                res.Add(Notification.SHOWN_FIELD, query.Shown);
            if(query.Type.HasValue)
                res.Add(Notification.TYPE_FIELD, query.Type);
            if(query.ClassPeriodRef.HasValue)
                res.Add(Notification.CLASS_PERIOD_REF_FIELD, query.ClassPeriodRef);
            return res;
        } 

        public IList<Notification> GetNotifications(NotificationQuery query)
        {
            var conds = BuildShortConditions(query);
            return SelectMany<Notification>(conds);
        }

        private DbQuery BuildGetNotificationDetailsDbQuery(NotificationQuery query)
        {
            var conds = BuildShortConditions(query);
            //TODO: think how to rewrtite this
            var sql = @"select {0}, 
                               vwPrivateMessage.*,
                               toPerson.Id as ToPerson_Id,
                               toPerson.FirstName as ToPerson_FirstName,
                               toPerson.LastName as ToPerson_LastName,
                               toPerson.Gender as ToPerson_Gender,
                               toPerson.Salutation as ToPerson_Salutation,
                               toPerson.RoleRef as ToPerson_RoleRef,
                               QuestionPerson.FirstName as QuestionPerson_FirstName,
                               QuestionPerson.LastName as QuestionPerson_LastName,
                               QuestionPerson.Gender as QuestionPerson_Gender,
                               QuestionPerson.Salutation as QuestionPerson_Salutation,
                               QuestionPerson.RoleRef as QuestionPerson_RoleRef,
                               QuestionPerson.Id as QuestionPerson_Id
                        from [Notification]
                        left join Announcement on Announcement.Id  = [Notification].AnnouncementRef
                        left join AnnouncementType on AnnouncementType.Id = Announcement.AnnouncementTypeRef
                        left join MarkingPeriod on MarkingPeriod.Id = [Notification].MarkingPeriodRef
                        left join ClassPeriod on ClassPeriod.Id = [Notification].ClassPeriodRef
                        left join vwPrivateMessage on vwPrivateMessage.PrivateMessage_Id = [Notification].PrivateMessageRef
                        join Person toPerson on toPerson.Id = [Notification].PersonRef
                        left join Person QuestionPerson on QuestionPerson.Id = [Notification].QuestionPersonRef";

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
            var res = new DbQuery(b, new Dictionary<string, object>());
            conds.BuildSqlWhere(res, tables[0].Name);
            return res;
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
            res.Person = ReadNotificationPerson(reader, "ToPerson");
            if (res.Type == NotificationType.Message)
                res.PrivateMessage = PrivateMessageDataAccess.ReadPrivateMessageDetails(reader);
            if (res.Type == NotificationType.Question)
                res.QuestionPerson = ReadNotificationPerson(reader, "QuestionPerson");
            if (res.Type == NotificationType.Announcement || res.Type == NotificationType.Question || res.Type == NotificationType.ItemToGrade)
            {
                res.Announcement = reader.Read<Announcement>(true);
                res.AnnouncementType = reader.Read<AnnouncementType>(true);
            }
            if (res.Type == NotificationType.MarkingPeriodEnding)
                res.MarkingPeriod = reader.Read<MarkingPeriod>(true);
            if (res.Type == NotificationType.NoTakeAttendance)
                res.ClassPeriod = reader.Read<ClassPeriod>(true);
            return res;
        }
        private Person ReadNotificationPerson(DbDataReader reader, string prefix)
        {
            var template = prefix + "_{0}";
            return new Person
                {
                    Id = SqlTools.ReadGuid(reader, string.Format(template, Person.ID_FIELD)),
                    FirstName = SqlTools.ReadStringNull(reader, string.Format(template, Person.FIRST_NAME_FIELD)),
                    LastName = SqlTools.ReadStringNull(reader, string.Format(template, Person.LAST_NAME_FIELD)),
                    RoleRef = SqlTools.ReadInt32(reader, string.Format(template, Person.ROLE_REF_FIELD)),
                    Gender = SqlTools.ReadStringNull(reader, string.Format(template, Person.GENDER_FIELD)),
                    Salutation = SqlTools.ReadStringNull(reader, string.Format(template, Person.SALUTATION_FIELD)),
                };
        }

        public IList<NotificationDetails> GetNotificationsDetails(NotificationQuery query)
        {
            var q = BuildGetNotificationDetailsDbQuery(query);
            using (var reader = ExecuteReaderParametrized(q.Sql.ToString(), q.Parameters))
            {
                return ReadListNotifcationDetails(reader);
            }
        }

        public PaginatedList<NotificationDetails> GetPaginatedNotificationsDetails(NotificationQuery query)
        {
            var innerQuery = BuildGetNotificationDetailsDbQuery(query);
            var orderBy = "Notification_" + Notification.CREATED_FIELD;
            var q = Orm.PaginationSelect(innerQuery, orderBy, Orm.OrderType.Desc, query.Start, query.Count);
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
        public NotificationType? Type { get; set; }
        public Guid? ClassPeriodRef { get; set; }

        public NotificationQuery()
        {
            Start = 0;
            Count = int.MaxValue;
        }
    }
}

