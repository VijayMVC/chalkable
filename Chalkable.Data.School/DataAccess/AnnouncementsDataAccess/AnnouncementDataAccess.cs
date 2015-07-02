using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public abstract class BaseAnnouncementDataAccess<TAnnouncement> : DataAccessBase<TAnnouncement, int> where TAnnouncement : Announcement, new()
    {
        protected BaseAnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        private const string GET_DETAILS_PROCEDURE = "spGetAnnouncementDetails";
        private const string GET_ANNOUNCEMENT_RECIPIENT_PERSON = "spGetAnnouncementRecipientPersons";
        private const string DELETE_PROCEDURE = "spDeleteAnnouncement";
        
        private const string CLASS_ANNOUNCEMENT_TYPE_ID_PARAM = "classAnnouncementTypeId";
        private const string PERSON_ID_PARAM = "personId";
        private const string STATE_PARAM = "state";
        private const string CLASS_ID_PARAM = "classId";
        
        private const string ID_PARAM = "id";
        private const string CALLER_ID_PARAM = "callerId";
        private const string CALLER_ROLE_PARAM = "@callerRole";
        private const string SCHOOL_ID = "schoolId";

        public abstract AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);
        public abstract TAnnouncement GetAnnouncement(int id, int callerId);
        public abstract TAnnouncement GetLastDraft(int personId);
        
        public abstract bool CanAddStandard(int announcementId);
        public abstract AnnouncementDetails GetDetails(int id, int callerId, int? roleId);


        public override void Update(TAnnouncement announcement)
        {
            var conds = new AndQueryCondition { { Announcement.ID_FIELD, "annId", announcement.Id, ConditionRelation.Equal} };
            var t = typeof(TAnnouncement);
            var fields = Orm.Fields(t, false, false, true);
            var ps = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                var fieldValue = t.GetProperty(field).GetValue(announcement);
                ps.Add(field, fieldValue);
            }
            SimpleUpdate<TAnnouncement>(ps, conds);
        }

        private SqlParameter PrepareTIdsParams(IEnumerable<int> announcementIds, string paramName)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof (int));
            foreach (var announcementId in announcementIds)
            {
                table.Rows.Add(announcementId);
            }
            return new SqlParameter
                {
                    ParameterName = "@" + paramName,
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "TIntId",
                    Value = table,
                };
        }
        
        public override void Delete(int announcementId)
        {
            Delete(new List<int> {announcementId});
        }
        
        public void Delete(IList<int> announcementIds)
        {
            var parameters = PrepareTIdsParams(announcementIds, "announcementIdList");
            ExcuteStoredProcedure("spDeleteAnnouncements", new []{parameters});
        }

        protected AnnouncementDetails GetDetails(string procedureName, IDictionary<string, object> parameters)
        {
            using (var reader = ExecuteStoredProcedureReader(procedureName, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        protected abstract TAnnouncement ReadAnnouncementData(AnnouncementComplex announcement, SqlDataReader reader);


        protected AnnouncementDetails BuildGetDetailsResult(SqlDataReader reader)
        {
            var announcement = reader.ReadOrNull<AnnouncementDetails>();
            if (announcement != null)
            {
                announcement.AnnouncementData = ReadAnnouncementData(announcement, reader);
                announcement = ReadAnnouncementAdditionalData(announcement, reader);
            }
            return announcement;
        }
        protected virtual AnnouncementDetails ReadAnnouncementAdditionalData(AnnouncementDetails announcement, SqlDataReader reader)
        {
            reader.NextResult();
            if (reader.Read())
                announcement.Owner = PersonDataAccess.ReadPersonData(reader);
            reader.NextResult();
            announcement.AnnouncementQnAs = AnnouncementQnADataAccess.ReadAnnouncementQnAComplexes(reader);
            reader.NextResult();
                announcement.AnnouncementAttributes = reader.ReadList<AnnouncementAssignedAttribute>();
                reader.NextResult();
            announcement.AnnouncementApplications = reader.ReadList<AnnouncementApplication>();
            reader.NextResult();
            announcement.AnnouncementStandards = reader.ReadList<AnnouncementStandardDetails>();
            reader.NextResult();
            announcement.AnnouncementAttachments = reader.ReadList<AnnouncementAttachment>();
            announcement.StudentAnnouncements = new List<StudentAnnouncementDetails>();
            return announcement;
        }
        
        protected AnnouncementQueryResult ReadAnnouncementsQueryResult(SqlDataReader reader, AnnouncementsQuery query)
        {
            var res = new AnnouncementQueryResult { Query = query };
            bool first = true;
            while (reader.Read())
            {
                var ann = reader.Read<AnnouncementComplex>();
                ann.AnnouncementData = ReadAnnouncementData(ann, reader);
                res.Announcements.Add(ann);
                if (first)
                {
                    res.SourceCount = SqlTools.ReadInt32(reader, AnnouncementQueryResult.ALL_COUNT);
                    first = false;
                }
            }
            return res;
        }
        
        public IList<Person> GetAnnouncementRecipientPersons(int announcementId, int callerId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {CALLER_ID_PARAM, callerId},
                    {"announcementId", announcementId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_ANNOUNCEMENT_RECIPIENT_PERSON, parameters))
            {
                var res = new List<Person>();
                while (reader.Read())
                {
                    res.Add(PersonDataAccess.ReadPersonData(reader));
                }
                return res;
            }
        }
    }

    public class AnnouncementsQuery
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public int? ClassId { get; set; }
        public int? PersonId { get; set; }
        public int? MarkingPeriodId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? Now { get; set; }
        public bool? Complete { get; set; }
        public bool OwnedOnly { get; set; }
        public bool GradedOnly { get; set; }
        public bool AllSchoolItems { get; set; }

        public IList<int> SisActivitiesIds { get; set; }

        public bool? Graded { get; set; }
        public IList<int> GradeLevelsIds { get; set; }
        public int? StudentId { get; set; }

        public int? GalleryCategoryId { get; set; }

        public AnnouncementsQuery()
        {
            Start = 0;
            Count = int.MaxValue;
        }
    }
    public class AnnouncementQueryResult
    {
        public List<AnnouncementComplex> Announcements { get; set; }
        public const string ALL_COUNT = "AllCount";
        public int SourceCount { get; set; }
        public AnnouncementsQuery Query { get; set; }

        public AnnouncementQueryResult()
        {
            Announcements = new List<AnnouncementComplex>();
            SourceCount = 0;
        }
    }
}
