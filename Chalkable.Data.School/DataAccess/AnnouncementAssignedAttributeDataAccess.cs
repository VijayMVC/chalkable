using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementAssignedAttributeDataAccess : DataAccessBase<AnnouncementAssignedAttribute, int>
    {
        public AnnouncementAssignedAttributeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AnnouncementAssignedAttribute> GetLastListByAnnId(int announcementId, int count)
        {
            return GetListByAnntId(announcementId).OrderByDescending(x => x.Id)
                                                  .Take(count).OrderBy(x => x.Id).ToList();
        }

        public IList<AnnouncementAssignedAttribute> GetListByAnntId(int announcementId)
        {
            var conds = new AndQueryCondition {{AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, announcementId}};
            return GetAll(conds);
        }

        public AnnouncementAssignedAttribute GetByAttachmentId(int attachmentId)
        {
            var conds = new AndQueryCondition
                {
                    {AnnouncementAssignedAttribute.SIS_ATTRIBUTE_ATTACHMENT_ID, attachmentId}
                };
            return GetAll(conds).First();
        } 
    }
}
