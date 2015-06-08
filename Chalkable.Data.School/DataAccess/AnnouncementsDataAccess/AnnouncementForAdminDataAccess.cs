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
        public AnnouncementForAdminDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
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
                    {PERSON_ID_PARAM, query.PersonId},
                    {GRADE_LEVELS_IDS_PARAM, query.GradeLevelsIds != null ? query.GradeLevelsIds.Select(x => x.ToString()).JoinString(",") : null},
                };
            using (var reader = ExecuteStoredProcedureReader(GET_ADMIN_ANNOUNCEMENT_PROCEDURE, parameters))
            {
                return ReadAnnouncementsQueryResult(reader, query);
            }
        }

        public override Announcement GetAnnouncement(int id, int callerId)
        {
            //TODO: think can admin get teacher items ? 
            var annTName = typeof(Announcement).Name;
            var conds = new AndQueryCondition
                {
                    {Announcement.ADMIN_REF_FIELD, callerId},
                    {Announcement.ID_FIELD, id}
                };
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat("select [{0}].*, cast(1 as bit) as IsOwner from [{0}] ", annTName);
            conds.BuildSqlWhere(dbQuery, annTName);
            return ReadOneOrNull<Announcement>(dbQuery);
        }

        public override Announcement GetLastDraft(int personId)
        {
            throw new NotImplementedException();
        }

        public override void ReorderAnnouncements(int schoolYearId, int announcementTypeId, int classId)
        {
            throw new NotImplementedException();
        }

        public override IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds)
        {
            throw new NotImplementedException();
        }

        public override IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int count)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(int sisActivityId)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(IList<int> sisActivitiesIds)
        {
            throw new NotImplementedException();
        }

        public override bool CanAddStandard(int announcementId)
        {
            //Todo: discuss with Geka about adding standard to admin Announcement
            return false;
        }
    }
}
