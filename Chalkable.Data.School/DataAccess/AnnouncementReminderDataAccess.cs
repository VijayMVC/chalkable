using System;
using System.Collections.Generic;
using System.Text;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementReminderDataAccess : DataAccessBase
    {
        public AnnouncementReminderDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(AnnouncementReminder announcementReminder)
        {
            SimpleInsert(announcementReminder);
        }
        public void Update(AnnouncementReminder announcementReminder)
        {
            SimpleUpdate(announcementReminder);
        }
        
        public void Delete(AnnouncementReminder announcementReminder)
        {
            SimpleDelete(announcementReminder);
        }
        public void Delete(Guid announcementId)
        {
            var conds = new Dictionary<string, object> {{"announcementRef", announcementId}};
            SimpleDelete<AnnouncementReminder>(conds);
        }

        public AnnouncementReminder GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            var annRType = typeof (AnnouncementReminder);
            var types = new List<Type> { annRType, typeof(Announcement) };
            var sql = string.Format(@"select {0} 
                                      from AnnouncementReminder 
                                      join Announcement a on a.Id = AnnouncementReminder.AnnouncementRef "
                                      , Orm.ComplexResultSetQuery(types));
            var b = new StringBuilder();
            b.Append(sql);
            b = Orm.BuildSqlWhere(b, annRType, conds);
            using (var reader = ExecuteReaderParametrized(b.ToString(), conds))
            {
                var res = reader.ReadOrNull<AnnouncementReminder>(true);
                res.Announcement = reader.ReadOrNull<Announcement>(true);
                return res;
            }
        }

        public IList<AnnouncementReminder> GetList(Guid announcementId)
        {
            var conds = new Dictionary<string, object> {{"announcementRef", announcementId}};
            return SelectMany<AnnouncementReminder>(conds);
        } 
    }
}
