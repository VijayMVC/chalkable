using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public abstract class AdminAnnouncementDataAccess : BaseAnnouncementDataAccess<AdminAnnouncement>
    {
        public AdminAnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
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
        private const string COMPLETE = "complete";
        private const string STUDENT_ID = "studentId";



        public override void Insert(AdminAnnouncement entity)
        {
            throw new NotImplementedException();
            //SimpleInsert<Announcement>(entity);
            //base.Insert(entity);
        }

        public override void Update(AdminAnnouncement entity)
        {
            SimpleUpdate<Announcement>(entity);
            base.Update(entity);
        }
        
        public AnnouncementDetails Create(DateTime created, DateTime expiresDate, int personId)
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

        public override AnnouncementDetails GetDetails(int id, int callerId, int? roleId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"adminAnnouncementId", id},
                    {"@callerId", callerId},
                    {"callerRole", roleId}
                };
            return GetDetails("spGetAdminAnnouncementDetails", parameters);
        }

        protected override AdminAnnouncement ReadAnnouncementData(AnnouncementComplex announcement, System.Data.SqlClient.SqlDataReader reader)
        {
            return reader.Read<AdminAnnouncement>();
        }

        protected override AnnouncementDetails ReadAnnouncementAdditionalData(AnnouncementDetails announcement, System.Data.SqlClient.SqlDataReader reader)
        {
            var res = base.ReadAnnouncementAdditionalData(announcement, reader);
            reader.NextResult();
            res.AnnouncementGroups = reader.ReadList<AnnouncementGroup>();
            return res;
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
                    {COMPLETE, query.Complete},
                    {STUDENT_ID, query.StudentId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_ADMIN_ANNOUNCEMENT_PROCEDURE, parameters))
            {
                return ReadAnnouncementsQueryResult(reader, query);
            }
        }

        public IList<AdminAnnouncement> GetAdminAnnouncementsByFilter(string filter, int callerId)
        {
            var res = GetAdminAnnouncements(new AndQueryCondition(), callerId);
            return res;
        } 

        public override AdminAnnouncement GetAnnouncement(int adminAnnouncementId, int callerId)
        {
            var conds = new AndQueryCondition
                {
                    {AdminAnnouncement.ADMIN_REF_FIELD, callerId},
                    {Announcement.ID_FIELD, adminAnnouncementId}
                };
            return GetAdminAnnouncements(conds, callerId).FirstOrDefault();
        }
        
        public IList<AdminAnnouncement> GetAdminAnnouncements(QueryCondition conds, int callerId)
        {
            var dbQuery = SelectAdminAnnouncement(callerId);
            conds.BuildSqlWhere(dbQuery, AdminAnnouncement.VW_ADMIN_ANNOUNCEMENT_NAME);
            dbQuery = FilterAdminAnnouncementByCaller(dbQuery, callerId);
            return ReadMany<AdminAnnouncement>(dbQuery);
        }

        protected abstract DbQuery SelectAdminAnnouncement(int callerId);
        protected abstract DbQuery FilterAdminAnnouncementByCaller(DbQuery dbQuery, int callerId);


        public IList<string> GetLastFieldValues(int personId, int count)
        {
            var conds = new AndQueryCondition {{AdminAnnouncement.ADMIN_REF_FIELD, personId}};
            var dbQuery = Orm.SimpleSelect(AdminAnnouncement.VW_ADMIN_ANNOUNCEMENT_NAME, conds);
            return ReadMany<AdminAnnouncement>(dbQuery).Select(x => x.Content).Distinct().ToList();
        }

        public bool Exists(string title, int adminId, int? excludeAnnouncementId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.TITLE_FIELD, title},
                    {AdminAnnouncement.ADMIN_REF_FIELD, adminId}
                };
            if(excludeAnnouncementId.HasValue)
                conds.Add(Announcement.ID_FIELD, excludeAnnouncementId, ConditionRelation.NotEqual);
            var dbQuery = Orm.SimpleSelect(AdminAnnouncement.VW_ADMIN_ANNOUNCEMENT_NAME, conds);
            return Exists(dbQuery);
        }
        
        public override bool CanAddStandard(int announcementId)
        {
            return false;
        }
    }


    public class AdminAnnouncementForAdminDataAccess : AdminAnnouncementDataAccess
    {
        public AdminAnnouncementForAdminDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override DbQuery SelectAdminAnnouncement(int callerId)
        {
            var dbQuery = new DbQuery();
            var selectSet = string.Format("[{0}].*, cast(1 as bit) as IsOwner", AdminAnnouncement.VW_ADMIN_ANNOUNCEMENT_NAME);
            dbQuery.Sql.AppendFormat("select {0} from [{1}] ", selectSet, AdminAnnouncement.VW_ADMIN_ANNOUNCEMENT_NAME);
            return dbQuery;
        }
        protected override DbQuery FilterAdminAnnouncementByCaller(DbQuery dbQuery, int callerId)
        {
            var callerIdParam = "callerId";
            dbQuery.Parameters.Add(callerIdParam, callerId);
            dbQuery.Sql.AppendFormat(" and {0}=@{1}", AdminAnnouncement.ADMIN_REF_FIELD, callerIdParam);
            return dbQuery;
        }

        public override AdminAnnouncement GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition {{Announcement.STATE_FIELD, AnnouncementState.Draft}};
            return GetAdminAnnouncements(conds, personId).OrderByDescending(x=>x.Created).FirstOrDefault();
        }

    }

    public class AdminAnnouncementForStudentDataAccess : AdminAnnouncementDataAccess
    {
        public AdminAnnouncementForStudentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override DbQuery SelectAdminAnnouncement(int callerId)
        {
            var dbQuery = new DbQuery();
            var selectSet = string.Format("[{0}].*, cast(0 as bit) as IsOwner", AdminAnnouncement.VW_ADMIN_ANNOUNCEMENT_NAME);
            dbQuery.Sql.AppendFormat("select {0} from [{1}] ", selectSet, AdminAnnouncement.VW_ADMIN_ANNOUNCEMENT_NAME);
            return dbQuery;
        }
        protected override DbQuery FilterAdminAnnouncementByCaller(DbQuery dbQuery, int callerId)
        {
            var callerIdParam = "callerId";
            dbQuery.Parameters.Add(callerIdParam, callerId);
            dbQuery.Sql.AppendFormat(@" and Id in 
                                        (
                                            select AnnouncementRef from AnnouncementGroup ar
				                            join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
				                            where StudentGroup.StudentRef = @{0}
                                        )", callerIdParam);
            return dbQuery;
        }

        public override AdminAnnouncement GetLastDraft(int personId)
        {
            throw new NotImplementedException();
        }
    }
}
