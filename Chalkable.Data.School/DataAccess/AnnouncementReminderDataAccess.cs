using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private IList<AnnouncementReminder> GetReminders(QueryCondition conds, Guid personId)
        {

            var annRType = typeof(AnnouncementReminder);
            var types = new List<Type> { annRType, typeof(Announcement) };
            var sql = string.Format(@"select {0} 
                                      from AnnouncementReminder 
                                      join Announcement  on Announcement.Id = AnnouncementReminder.AnnouncementRef "
                                      , Orm.ComplexResultSetQuery(types));
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(sql);
            conds.BuildSqlWhere(dbQuery, annRType.Name);

            //var andConds = new AndQueryCondition{{}}

            dbQuery.Sql.Append(@" and ((Announcement.PersonRef = @personId and AnnouncementReminder.PersonRef is null)
                             or (AnnouncementReminder.PersonRef = @personId))");
            dbQuery.Parameters.Add("@personId", personId);
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
        
        public IList<AnnouncementReminder> GetList(Guid announcementId, Guid personId)
        {
            var conds = new AndQueryCondition {{AnnouncementReminder.ANNOUNCEMENT_REF_FIELD, announcementId}};
            return GetReminders(conds, personId);
        }

    }
}
