using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementRecipientDataDataAccess : DataAccessBase<AnnouncementRecipientData, int>
    {
        public AnnouncementRecipientDataDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Update(int announcementId, int personId, bool starred, int? starredAutomatically, DateTime modifiedDate)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"@announcementId", announcementId},
                    {"@personId", personId},
                    {"@starred", starred},
                    {"@starredAutomatically", starredAutomatically},
                    {"@currentDate", modifiedDate}
                };
            ExecuteStoredProcedureReader("spUpdateAnnouncemetRecipientData", parameters).Dispose();
        }
    }
}
