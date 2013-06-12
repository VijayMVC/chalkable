using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using Chalkable.Common;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementQnADataAccess : DataAccessBase
    {
        public AnnouncementQnADataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(AnnouncementQnA announcementQnA)
        {
            SimpleInsert(announcementQnA);
        }

        public void Update(AnnouncementQnA announcementQnA)
        {
            SimpleUpdate(announcementQnA);
        }

        public void Delete(AnnouncementQnA announcementQnA)
        {
            SimpleDelete(announcementQnA);
        }

        public AnnouncementQnA GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            return SelectOne<AnnouncementQnA>(conds);
        }
        
        public AnnouncementQnA GetWithAnnouncement(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            var format = " {0}.{1} as {0}_{1}";
            var fields = Orm.Fields<AnnouncementQnA>().Select(x => string.Format(format, typeof(AnnouncementQnA).Name, x)).ToList();
            fields.AddRange(Orm.Fields<AnnouncementQnA>().Select(x => string.Format(format, typeof(Announcement).Name, x)));
            
            var b = new StringBuilder();
            b.AppendFormat(@"select {0}
                           from [AnnouncementQnA] 
                           join left Announcement a.Id = [AnnouncementQnA].AnnouncementRef
                           where [AnnouncementQnA].Id = @id", fields.JoinString(","));
            var sql = b.ToString();
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                return reader.ReadOrNull<AnnouncementQnA>();
            }
        }

    }
}
