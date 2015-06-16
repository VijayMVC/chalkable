using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public abstract class AnnouncementDataAccess : DataAccessBase<Announcement, int>
    {
        protected AnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
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

        public abstract AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime created, DateTime expiresDate, int personId);
        public abstract AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);
        public abstract Announcement GetAnnouncement(int id, int callerId);
        public abstract Announcement GetLastDraft(int personId);
        
        public abstract void ReorderAnnouncements(int schoolYearId, int announcementTypeId, int classId);
        public abstract IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds);
        public abstract IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int count);
        public abstract bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId);
        public abstract bool Exists(int sisActivityId);
        public abstract bool Exists(IList<int> sisActivitiesIds);
        public abstract bool CanAddStandard(int announcementId);


        public void Delete(int? id, int? personId, int? classId, int? classAnnouncementTypeId, AnnouncementState? state)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, id},
                    {PERSON_ID_PARAM, personId},
                    {CLASS_ID_PARAM, classId},
                    {CLASS_ANNOUNCEMENT_TYPE_ID_PARAM, classAnnouncementTypeId},
                    {STATE_PARAM, state}
                };
            ExecuteStoredProcedureReader(DELETE_PROCEDURE, parameters).Dispose();
        }

        public virtual AnnouncementDetails GetDetails(int id, int callerId, int? roleId)
        {
            return GetDetails(id, callerId, roleId, null);
        }

        protected AnnouncementDetails GetDetails(int id, int callerId, int? roleId, int? schoolId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, id},
                    {CALLER_ID_PARAM, callerId},
                    {CALLER_ROLE_PARAM, roleId},
                    {SCHOOL_ID, schoolId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_DETAILS_PROCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        protected AnnouncementDetails BuildGetDetailsResult(SqlDataReader reader)
        {
            var announcement = reader.ReadOrNull<AnnouncementDetails>();
            if (announcement != null)
            {
                announcement.IsScored = announcement.MaxScore > 0;
                reader.NextResult();
                announcement.AnnouncementQnAs = AnnouncementQnADataAccess.ReadAnnouncementQnAComplexes(reader);
                reader.NextResult();
                announcement.AnnouncementAttachments = reader.ReadList<AnnouncementAttachment>();
                reader.NextResult();
                announcement.AnnouncementApplications = reader.ReadList<AnnouncementApplication>();
                reader.NextResult();
                if(reader.Read())
                    announcement.Owner = PersonDataAccess.ReadPersonData(reader);
                reader.NextResult();
                announcement.AnnouncementStandards = reader.ReadList<AnnouncementStandardDetails>();
                reader.NextResult();
                announcement.AnnouncementRecipients = reader.ReadList<AdminAnnouncementRecipient>(true);
                announcement.StudentAnnouncements = new List<StudentAnnouncementDetails>();
            }
            return announcement;
        }
        
        protected AnnouncementQueryResult ReadAnnouncementsQueryResult(DbDataReader reader, AnnouncementsQuery query)
        {
            var res = new AnnouncementQueryResult { Query = query };
            bool first = true;
            while (reader.Read())
            {
                res.Announcements.Add(reader.Read<AnnouncementComplex>());
                if (first)
                {
                    res.SourceCount = SqlTools.ReadInt32(reader, AnnouncementQueryResult.ALL_COUNT);
                    first = false;
                }
            }
            return res;
        }
        
        //TODO: thinks about this 
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
        public bool AdminOnly { get; set; }
        public int? StudentId { get; set; }

        public AnnouncementsQuery()
        {
            Start = 0;
            Count = int.MaxValue;
            AdminOnly = false;
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
