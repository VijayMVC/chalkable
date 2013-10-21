using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementReminderDataAccess : DataAccessBase<AnnouncementReminder>
    {
        public AnnouncementReminderDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(AnnouncementReminder announcementReminder)
        {
            SimpleDelete(announcementReminder);
        }

        public void DeleteByAnnouncementId(Guid announcementId)
        {
            var conds = new AndQueryCondition {{AnnouncementReminder.ANNOUNCEMENT_REF_FIELD, announcementId}};
            SimpleDelete<AnnouncementReminder>(conds);
        }

        public AnnouncementReminder GetById(Guid id, Guid personId)
        {
            var conds = new AndQueryCondition {{AnnouncementReminder.ID_FIELD, id}};
            var res = GetReminders(conds, personId);
            return res.First();
        }

        private DbQuery BuildBasicSelect(int? top = null)
        {
            var types = new List<Type> {typeof (AnnouncementReminder), typeof (Announcement)};
            var sql = string.Format((top.HasValue ? "select top " + top + " {0} " : "select {0} ") +
                                      @"from AnnouncementReminder 
                                      join Announcement  on Announcement.Id = AnnouncementReminder.AnnouncementRef "
                                    , Orm.ComplexResultSetQuery(types));
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(sql);
            return dbQuery;
        }

        private IList<AnnouncementReminder> Read(DbQuery dbQuery)
        {
            using (var reader = ExecuteReaderParametrized(dbQuery.Sql.ToString(), dbQuery.Parameters))
            {
                var res = new List<AnnouncementReminder>();
                while (reader.Read())
                {
                    var reminder = reader.Read<AnnouncementReminder>(true);
                    reminder.Announcement = reader.Read<Announcement>(true);
                    res.Add(reminder);
                }
                return res;
            }
        }

        private IList<AnnouncementReminder> GetReminders(QueryCondition conds, Guid personId)
        {

            var annRType = typeof(AnnouncementReminder);
            var dbQuery = BuildBasicSelect();
            
            conds.BuildSqlWhere(dbQuery, annRType.Name);
            //TODO: use querycondition
            dbQuery.Sql.Append(@" and ((Announcement.PersonRef = @personId and AnnouncementReminder.PersonRef is null)
                             or (AnnouncementReminder.PersonRef = @personId))");
            dbQuery.Parameters.Add("@personId", personId);
            return Read(dbQuery);
        } 
        
        public IList<AnnouncementReminder> GetList(Guid announcementId, Guid personId)
        {
            var conds = new AndQueryCondition {{AnnouncementReminder.ANNOUNCEMENT_REF_FIELD, announcementId}};
            return GetReminders(conds, personId);
        }

        public IList<AnnouncementReminder> GetRemindersToProcess(DateTime schoolTimeNow, int count)
        {
            var dbQuery = BuildBasicSelect(count);

            var conds = new AndQueryCondition();
            conds.Add(new SimpleQueryCondition(AnnouncementReminder.REMIND_DATE_FIELD,
                          AnnouncementReminder.REMIND_DATE_FIELD, null, ConditionRelation.NotEqual));
            conds.Add(new SimpleQueryCondition(AnnouncementReminder.REMIND_DATE_FIELD,
                          AnnouncementReminder.REMIND_DATE_FIELD, schoolTimeNow, ConditionRelation.LessEqual));
            conds.Add(new SimpleQueryCondition(AnnouncementReminder.PROCESSED_FIELD,
                          AnnouncementReminder.PROCESSED_FIELD, false, ConditionRelation.Equal));

            conds.BuildSqlWhere(dbQuery, "AnnouncementReminder");
            dbQuery.Sql.Append(" and Announcement.State <> ").Append((int) AnnouncementState.Draft);
            
            //TODO: apply count
            return Read(dbQuery);
        }
    }
}
