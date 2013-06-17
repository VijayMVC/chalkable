using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementAttachmentDataAccess : DataAccessBase
    {
        public AnnouncementAttachmentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(AnnouncementAttachment announcementAttachment)
        {
            SimpleInsert(announcementAttachment);
        }

        public void Delete(AnnouncementAttachment announcementAttachment)
        {
            SimpleDelete(announcementAttachment);
        }

        public AnnouncementAttachment GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"Id", id}};
            return SelectOne<AnnouncementAttachment>(conds);
        }


        //private const string CALLER_ID = "@callerId"
//        private IList<AnnouncementAttachment> GetAnnouncementAttachments(Dictionary<string, object> conds, Guid callerId)
//        {
//            var sql = @"select {0} 
//                        from AnnouncementAttachment 
//                        join Announcement ";
//        } 
    }
}
