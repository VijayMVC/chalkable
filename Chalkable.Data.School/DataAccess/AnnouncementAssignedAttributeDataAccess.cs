using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
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

        public IList<AnnouncementAssignedAttribute> GetLastListByAnnIds(IList<int> toAnnouncementIds, int count)
        {
            var annIdsStr = toAnnouncementIds.Select(x => x.ToString()).JoinString(",");
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, "*", typeof (AnnouncementAssignedAttribute).Name)
                   .AppendFormat(" Where {0} in ({1}) ", AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, annIdsStr);

            return ReadMany<AnnouncementAssignedAttribute>(dbQuery)
                   .OrderByDescending(x => x.Id).Take(count).OrderBy(x => x.Id).ToList();
        }

        public IList<AnnouncementAssignedAttribute> GetLastListByAnnId(int announcementId, int count)
        {
            return GetLastListByAnnIds(new List<int> {announcementId}, count);
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
