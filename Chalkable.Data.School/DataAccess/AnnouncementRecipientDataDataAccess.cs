using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementRecipientDataDataAccess : DataAccessBase<AnnouncementRecipientData>
    {
        public AnnouncementRecipientDataDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Update(Guid announcementId, Guid personId, bool starred, int? starredAutomatically)
        {
            var parameters = new Dictionary<string, object>()
                {
                    {"@announcementId", announcementId},
                    {"@personId", personId},
                    {"@starred", starred},
                    {"@starredAutomatically", starredAutomatically}
                };
            ExecuteStoredProcedureReader("spUpdateAnnouncemetRecipientData", parameters).Dispose();
        }
    }
}
