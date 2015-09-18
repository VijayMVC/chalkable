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

        private const string SP_UPDATE_ADMIN_ANNOUNCEMENT_DATA = "spUpdateAnnouncementRecipientData";
        private const string ANNOUNCEMENT_ID_PARAM = "announcementId";
        private const string PERSON_ID_PARAM = "personId";
        private const string COMPLETE_PARAM = "complete";
        private const string DATE_PARAM = "tillDate";
        private const string ROLE_ID_PARAM = "roleId";
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string ANNOUCEMENT_TYPE = "annType";
        private const string CLASS_ID = "classId";

        public void UpdateAnnouncementRecipientData(int? announcementId, int annType, int?schoolYearId, int? personId, int? roleId, bool complete, DateTime? tillDate, int? classId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ANNOUNCEMENT_ID_PARAM, announcementId},
                    {PERSON_ID_PARAM, personId},
                    {ROLE_ID_PARAM, roleId },
                    {COMPLETE_PARAM, complete},
                    {DATE_PARAM, tillDate },
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId },
                    {ANNOUCEMENT_TYPE, annType },
                    {CLASS_ID, classId }
                };
            ExecuteStoredProcedure(SP_UPDATE_ADMIN_ANNOUNCEMENT_DATA, parameters);
        }
    }
}
