using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess
{
    public class SupplementalAnnouncementDataAccess : BaseAnnouncementDataAccess<SupplementalAnnouncement>
    {
        protected int SchoolYearId;

        public SupplementalAnnouncementDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork)
        {
            SchoolYearId = schoolYearId;
        }

        public override IList<SupplementalAnnouncement> GetByIds(IList<int> keys)
        {
            var query = $@"Select * 
                           From  {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}
                           Where {Announcement.ID_FIELD} in (Select * From @keys)";
            var @params = new Dictionary<string, object>
            {
                ["keys"] = keys
            };

            var dbQuery = new DbQuery(query, @params);

            return ReadMany<SupplementalAnnouncement>(dbQuery);
        }

        public override IList<SupplementalAnnouncement> GetAnnouncements(QueryCondition conds, int callerId)
        {
            var condition = new AndQueryCondition {conds, {nameof(SupplementalAnnouncement.SchoolYearRef), SchoolYearId} };
            var isOwnerScript = 
                $@"Select
                        Cast(Case When Count(*) > 0 Then 1 Else 0 End As bit)
                   From 
                        {nameof(ClassTeacher)} 
                   Where
                        {nameof(ClassTeacher)}.{ClassTeacher.PERSON_REF_FIELD} = @callerId
                    And {nameof(ClassTeacher)}.{ClassTeacher.CLASS_REF_FIELD}  = {nameof(SupplementalAnnouncement)}.{SupplementalAnnouncement.CLASS_REF_FIELD}";

            var query = 
                $@"Select {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}.*, {isOwnerScript} as IsOwner
                   From   {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}";

            var @params = new Dictionary<string, object>
            {
                ["callerId"] = callerId
            };

            var dbQuery = new DbQuery(query, @params);
            condition.BuildSqlWhere(dbQuery, SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT);

            return ReadMany<SupplementalAnnouncement>(dbQuery);
        }

        public override SupplementalAnnouncement GetAnnouncement(int id, int callerId)
        {
            return GetAnnouncements(new AndQueryCondition {{nameof(SupplementalAnnouncement.Id), id}}, callerId)
                    .FirstOrDefault();
        }

        public override bool CanAddStandard(int announcementId)
        {
            var query = $@"Select {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}.Id 
                           From {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}
                                Join {nameof(Class)} 
                                    on {nameof(Class)}.{Class.ID_FIELD} = {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}.{SupplementalAnnouncement.CLASS_REF_FIELD}
                                Join {nameof(ClassStandard)} 
                                    on    {nameof(ClassStandard)}.{ClassStandard.CLASS_REF_FIELD} = {nameof(Class)}.{Class.ID_FIELD}
                                       Or {nameof(ClassStandard)}.{ClassStandard.CLASS_REF_FIELD} = {nameof(Class)}.{Class.COURSE_REF_FIELD}";

            var conds = new AndQueryCondition
            {
                {Announcement.ID_FIELD, announcementId },
                {SupplementalAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, SchoolYearId }
            };

            var dbQuery = new DbQuery(query, new Dictionary<string, object>());

            conds.BuildSqlWhere(dbQuery, SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT);
            return Exists(dbQuery);
        }

        public override IList<AnnouncementDetails> GetDetailses(IList<int> ids, int callerId, int? roleId, bool onlyOwner = true)
        {
            var @params = new Dictionary<string, object>
            {
                ["announcementIds"] = ids,
                ["callerId"] = callerId,
                ["callerRole"] = roleId,
                ["schoolYearId"] = SchoolYearId
            };

            using (var reader = ExecuteStoredProcedureReader("spGetListOfSupplementalAnnouncementDetailsByIds", @params))
            {
                return BuildGetDetailsesResult(reader);
            }
        }

        protected override bool CanGetAllItems => false;

        protected override SupplementalAnnouncement ReadAnnouncementData(AnnouncementComplex announcement, SqlDataReader reader)
        {
            var res = reader.Read<SupplementalAnnouncement>();
            return res;
        }

        public AnnouncementDetails Create(int classId, DateTime created, DateTime expiresDate, int personId, int classAnnouncementTypeId)
        {
            var @params = new Dictionary<string, object>
            {
                ["created"] = created,
                ["expires"] = expiresDate,
                ["classId"] = classId,
                ["classAnnouncementTypeId"] = classAnnouncementTypeId,
                ["personId"] = personId,
                ["state"] = AnnouncementState.Draft
            };

            using (var reader = ExecuteStoredProcedureReader("spCreateSupplementalAnnouncement", @params))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        public override void Update(SupplementalAnnouncement entity)
        {
            SimpleUpdate<Announcement>(entity);
            base.Update(entity);
        }
    }
}
