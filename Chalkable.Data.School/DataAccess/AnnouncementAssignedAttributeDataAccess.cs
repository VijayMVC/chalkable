using System.Collections.Generic;
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

        public IList<AnnouncementAssignedAttribute> GetListByAnnouncementId(int announcementId)
        {
            var conds = new AndQueryCondition {{AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, announcementId}};
            return GetAll(conds);
        }
    }
}
