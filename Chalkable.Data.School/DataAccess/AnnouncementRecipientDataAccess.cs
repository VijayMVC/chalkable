using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementRecipientDataAccess : DataAccessBase
    {
        public AnnouncementRecipientDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string ANNOUNCEMENT_REF_FIELD = "announcementRef";

        public void Create(AnnouncementRecipient announcementRecipient)
        {
            SimpleInsert(announcementRecipient);
        }

        public void Create(IList<AnnouncementRecipient> announcementRecipients)
        {
            SimpleInsert(announcementRecipients);
        }

        public void Delete(Guid announcementId)
        {
            var conds = new Dictionary<string, object> {{ANNOUNCEMENT_REF_FIELD, announcementId}};
            SimpleDelete<AnnouncementRecipient>(conds);
        }

        public IList<AnnouncementRecipient> GetList(Guid announcementId)
        {
            var conds = new Dictionary<string, object> {{ANNOUNCEMENT_REF_FIELD, announcementId}};
            return SelectMany<AnnouncementRecipient>(conds);
        }
    }
}
