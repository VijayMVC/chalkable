using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForAdminDataAccess : AnnouncementDataAccess
    {
        public AnnouncementForAdminDataAccess(UnitOfWork unitOfWork, int schoolId) : base(unitOfWork, schoolId)
        {
        }

        private const string GET_ADMIN_ANNOUNCEMENT_PROCEDURE = "spGetAdminAnnouncements";       
        private const string CREATE_ADMIN_ANNOUNCEMENT_PROCEDURE = "spCreateAdminAnnouncement";

        private const string PERSON_ID_PARAM = "personId";
        private const string CREATED_PARAM = "created";
        private const string EXPIRES_PARAM = "expires";
        private const string STATE_PARAM = "state";
     
            
        private const string ID_PARAM = "id";
        private const string ROLE_ID_PARAM = "roleId";
        private const string OWNED_ONLY_PARAM = "ownedOnly";
        private const string FROM_DATE_PARAM = "fromDate";
        private const string TO_DATE_PARAM = "toDate";
        private const string START_PARAM = "start";
        private const string COUNT_PARAM = "count";
        private const string NOW_PARAM = "now";
        private const string GRADE_LEVELS_IDS_PARAM = "gradeLevelsIds";

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, query.Id },
                    {ROLE_ID_PARAM, query.RoleId },
                    {OWNED_ONLY_PARAM, query.OwnedOnly },
                    {FROM_DATE_PARAM, query.FromDate },
                    {TO_DATE_PARAM, query.ToDate },
                    {START_PARAM, query.Start },
                    {COUNT_PARAM, query.Count },
                    {NOW_PARAM, query.Now },
                    {GRADE_LEVELS_IDS_PARAM, query.GradeLevelsIds != null ? query.GradeLevelsIds.Select(x => x.ToString()).JoinString(",") : null},
                };
            using (var reader = ExecuteStoredProcedureReader(GET_ADMIN_ANNOUNCEMENT_PROCEDURE, parameters))
            {
                return ReadAnnouncementsQueryResult(reader, query);
            }
        }

        protected override void BuildConditionForGetSimpleAnnouncement(DbQuery dbQuery, int role, int callerId)
        {
            throw new NotImplementedException();
        }

        public override Announcement GetLastDraft(int personId)
        {
            throw new NotImplementedException();
            //return base.GetLastDraft(personId);
        }

        
        public override AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime created, DateTime expiresDate, int personId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {PERSON_ID_PARAM, personId},
                    {CREATED_PARAM, created},
                    {EXPIRES_PARAM, expiresDate},
                    {STATE_PARAM, AnnouncementState.Draft},
                };

            using (var reader = ExecuteStoredProcedureReader(CREATE_ADMIN_ANNOUNCEMENT_PROCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }
    }
}
