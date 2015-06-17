using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AdminAnnouncementDataDataAccess : DataAccessBase<AdminAnnouncementData, int>
    {
        public AdminAnnouncementDataDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string SP_UPDATE_ADMIN_ANNOUNCEMENT_DATA = "spUpdateAdminAnnouncementData";
        private const string ANNOUNCEMENT_ID_PARAM = "announcementId";
        private const string PERSON_ID_PARAM = "personId";
        private const string COMPLETE_PARAM = "complete";

        public void UpdateAdminAnnouncementData(int announcementId, int personId, bool complete)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ANNOUNCEMENT_ID_PARAM, announcementId},
                    {PERSON_ID_PARAM, personId},
                    {COMPLETE_PARAM, complete}
                };
            ExecuteStoredProcedure(SP_UPDATE_ADMIN_ANNOUNCEMENT_DATA, parameters);
        }
    }
}
