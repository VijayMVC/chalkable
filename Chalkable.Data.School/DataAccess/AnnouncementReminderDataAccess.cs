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
            var conds = new AndQueryCondition {{"announcementRef", announcementId}};
            SimpleDelete<AnnouncementReminder>(conds);
        }

        public AnnouncementReminder GetById(Guid id, Guid personId)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            var res = GetReminders(conds, personId);
            return res.First();
        }

        private IList<AnnouncementReminder> GetReminders(Dictionary<string, object> conditions, Guid personId)
        {

            var annRType = typeof(AnnouncementReminder);
            var types = new List<Type> { annRType, typeof(Announcement) };
            var sql = string.Format(@"select {0} 
                                      from AnnouncementReminder 
                                      join Announcement  on Announcement.Id = AnnouncementReminder.AnnouncementRef "
                                      , Orm.ComplexResultSetQuery(types));
            var b = new StringBuilder();
            b.Append(sql);
            b = Orm.BuildSqlWhere(b, annRType, conditions);
            b.Append(@" and ((Announcement.PersonRef = @personId and AnnouncementReminder.PersonRef is null)
                             or (AnnouncementReminder.PersonRef = @personId))");
            conditions.Add("@personId", personId);
            using (var reader = ExecuteReaderParametrized(b.ToString(), conditions))
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
            var conds = new Dictionary<string, object> {{"AnnouncementRef", announcementId}};
            return GetReminders(conds, personId);
        }

    }
}
