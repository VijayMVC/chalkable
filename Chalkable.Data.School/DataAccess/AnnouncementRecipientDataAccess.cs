using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementRecipientDataAccess : DataAccessBase<AnnouncementRecipient>
    {
        public AnnouncementRecipientDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AnnouncementRecipient> GetList(Guid announcementId)
        {
            var conds = new AndQueryCondition { { AnnouncementRecipient.ANNOUNCEMENT_REF_FIELD, announcementId } };
            return SelectMany<AnnouncementRecipient>(conds);
        }

        public void DeleteByAnnouncementId(Guid announcementId)
        {
            var conds = new AndQueryCondition { { AnnouncementRecipient.ANNOUNCEMENT_REF_FIELD, announcementId } };
            SimpleDelete<AnnouncementRecipient>(conds);
        }
 
    }
}
