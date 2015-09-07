﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model.Announcements.Sis;
using Chalkable.Data.School.Model.Chlk;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Data.School.DataAccess
{
    public class NotificationDataAccess : DataAccessBase<Notification, int>
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
            if(query.FromDate.HasValue)
                res.Add(Notification.CREATED_FIELD, "fromDate", query.FromDate, ConditionRelation.GreaterEqual);
            if(query.ToDate.HasValue)
                res.Add(Notification.CREATED_FIELD, "toDate", query.ToDate, ConditionRelation.LessEqual);
            return res;
        } 

        public IList<Notification> GetNotifications(NotificationQuery query)
        {
            var conds = BuildShortConditions(query);
            return SelectMany<Notification>(conds);
        }

        private DbQuery BuildGetNotificationDetailsDbQuery(NotificationQuery query, bool includeMessages)
        {
            var conds = BuildShortConditions(query);
            if(!includeMessages)
                conds.Add(Notification.TYPE_FIELD, NotificationType.Message, ConditionRelation.NotEqual);
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
                        left join MarkingPeriod on MarkingPeriod.Id = [Notification].MarkingPeriodRef
                        left join vwPrivateMessage on vwPrivateMessage.PrivateMessage_Id = [Notification].PrivateMessageRef
                        join vwPerson toPerson on toPerson.Id = [Notification].PersonRef
                        left join vwPerson QuestionPerson on QuestionPerson.Id = [Notification].QuestionPersonRef";

            var b = new StringBuilder();
            var tables = new List<Type>
                {
                    typeof (Notification),
                    typeof (Announcement),
                    typeof (MarkingPeriod)
                };
            b.AppendFormat(sql, Orm.ComplexResultSetQuery(tables));
            var res = new DbQuery(b, new Dictionary<string, object>());
            conds.BuildSqlWhere(res, Orm.TableName(tables[0]));
            res.Sql.AppendFormat(
                " and (toPerson.[{0}] =@{0} and (QuestionPerson.[{0}] is null or QuestionPerson.[{0}] =@{0}))"
                , SchoolPerson.SCHOOL_REF_FIELD);
            res.Sql.AppendFormat(" and (PrivateMessage_Id is null or (PrivateMessage_SenderSchoolRef = @{0} and PrivateMessage_RecipientSchoolRef = @{0}))"
                , SchoolPerson.SCHOOL_REF_FIELD);
            res.Parameters.Add(SchoolPerson.SCHOOL_REF_FIELD, query.SchoolId);
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
            }
            if (res.Type == NotificationType.MarkingPeriodEnding)
                res.MarkingPeriod = reader.Read<MarkingPeriod>(true);
            //todo this later 
            //if (res.Type == NotificationType.NoTakeAttendance)
            //    res.ClassPeriod = reader.Read<ClassPeriod>(true);
            return res;
        }
        private Person ReadNotificationPerson(DbDataReader reader, string prefix)
        {
            var template = prefix + "_{0}";
            return new Person
                {
                    Id = SqlTools.ReadInt32(reader, string.Format(template, Person.ID_FIELD)),
                    FirstName = SqlTools.ReadStringNull(reader, string.Format(template, Person.FIRST_NAME_FIELD)),
                    LastName = SqlTools.ReadStringNull(reader, string.Format(template, Person.LAST_NAME_FIELD)),
                    RoleRef = SqlTools.ReadInt32(reader, string.Format(template, Person.ROLE_REF_FIELD)),
                    Gender = SqlTools.ReadStringNull(reader, string.Format(template, Person.GENDER_FIELD)),
                    Salutation = SqlTools.ReadStringNull(reader, string.Format(template, Person.SALUTATION_FIELD)),
                };
        }

        public IList<NotificationDetails> GetNotificationsDetails(NotificationQuery query, bool includeMessages = true)
        {
            var q = BuildGetNotificationDetailsDbQuery(query, includeMessages);
            using (var reader = ExecuteReaderParametrized(q.Sql.ToString(), q.Parameters))
            {
                return ReadListNotifcationDetails(reader);
            }
        }

        public PaginatedList<NotificationDetails> GetPaginatedNotificationsDetails(NotificationQuery query, bool includeMessages = true)
        {
            var innerQuery = BuildGetNotificationDetailsDbQuery(query, includeMessages);
            var orderBy = new Dictionary<string, Orm.OrderType>
                {
                    {"Notification_" + Notification.CREATED_FIELD,Orm.OrderType.Desc},
                    { "Notification_" + Notification.ID_FIELD, Orm.OrderType.Desc}
                };
            var q = Orm.PaginationSelect(innerQuery, orderBy, query.Start, query.Count);
            return ReadPaginatedResult(q, query.Start, query.Count, ReadListNotifcationDetails);
        }

        public int GetUnshownNotificationsCount(int personId)
        {
            var sql = string.Format("select count(*) as UnshownCount from Notification where {0} =@{0} and {1} = @{1}"
                , Notification.SHOWN_FIELD, Notification.PERSON_REF_FIELD);
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {Notification.SHOWN_FIELD, false},
                {Notification.PERSON_REF_FIELD, personId},
            };
            using (var reader = ExecuteReaderParametrized(sql, ps))
            {
                reader.Read();
                var res = SqlTools.ReadInt32(reader, "UnshownCount");
                return res;
            }
        }
    }

    public class NotificationQuery
    {
        public int? Id { get; set; }
        public int? PersonId { get; set; }
        public bool? Shown { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }
        public NotificationType? Type { get; set; }
        public int? ClassPeriodRef { get; set; }
        public int SchoolId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public NotificationQuery()
        {
            Start = 0;
            Count = int.MaxValue;
        }
    }
}

