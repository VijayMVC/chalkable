using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
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
        
        private const string GET_ANNOUNCEMENT_RECIPIENT_PERSON = "spGetAnnouncementRecipientPersons";
        private const string CALLER_ID_PARAM = "callerId";
   

        //public abstract AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);
        public abstract IList<TAnnouncement> GetAnnouncements(QueryCondition conds, int callerId); 
        public abstract TAnnouncement GetAnnouncement(int id, int callerId);
        public abstract bool CanAddStandard(int announcementId);
        public abstract IList<AnnouncementDetails> GetDetailses(IList<int> ids, int callerId, int? roleId, bool onlyOwner = true);

        protected abstract bool CanGetAllItems { get; }

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

        
        public override void Delete(int announcementId)
        {
            Delete(new List<int> {announcementId});
        }
        
        public void Delete(IList<int> announcementIds)
        {
            var paraments = new Dictionary<string, object>()
            {
                {"announcementIdList", announcementIds}
            };
            ExecuteStoredProcedure("spDeleteAnnouncements", paraments);
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

        //protected virtual AnnouncementQueryResult InternalGetAnnouncements<TAnnouncementsQuery>(string procedureName, TAnnouncementsQuery query) where TAnnouncementsQuery : AnnouncementsQuery
        //{
        //    return InternalGetAnnouncements(procedureName, query, null);
        //}

        protected AnnouncementQueryResult InternalGetAnnouncements<TAnnouncementsQuery>(string procedureName, TAnnouncementsQuery query, IDictionary<string, object> additionalParameters) where TAnnouncementsQuery : AnnouncementsQuery
        {
            var ps = new Dictionary<string, object>
            {
                ["Id"] = query.Id,
                ["personId"] = query.PersonId,
                ["roleId"] = query.RoleId,
                ["fromDate"] = query.FromDate,
                ["toDate"] = query.ToDate,
                ["start"] = query.Start,
                ["count"] = query.Count,
                ["complete"] = query.Complete,
                ["ownedOnly"] = !CanGetAllItems,
                ["includeFrom"] = query.IncludeFrom,
                ["includeTo"] = query.IncludeTo,
                ["sort"] = query.Sort
            };
            if (additionalParameters != null)
                foreach (var parameter in additionalParameters)
                {
                    if(ps.ContainsKey(parameter.Key)) continue;
                    ps.Add(parameter.Key, parameter.Value);
                }
            using (var reader = ExecuteStoredProcedureReader(procedureName, ps))
            {
                return ReadAnnouncementsQueryResult(reader, query);
            }
        }


        protected IList<AnnouncementDetails> GetDetailses(string procedureName, IDictionary<string, object> parameters)
        {
            using (var reader = ExecuteStoredProcedureReader(procedureName, parameters))
            {
                return BuildGetDetailsesResult(reader);
            }
        }

        protected AnnouncementDetails BuildGetDetailsResult(SqlDataReader reader)
        {
            return BuildGetDetailsesResult(reader).FirstOrDefault();
        }
        protected virtual IList<AnnouncementDetails> BuildGetDetailsesResult(SqlDataReader reader)
        {
            IList<AnnouncementDetails> res = new List<AnnouncementDetails>();
            while (reader.Read())
            {
                var ann = reader.Read<AnnouncementDetails>();
                ann.AnnouncementData = ReadAnnouncementData(ann, reader);
                ann.StudentAnnouncements = new List<StudentAnnouncementDetails>();
                res.Add(ann);
            }

            reader.NextResult();
            var annAtts = reader.ReadList<AnnouncementAttachment>(true);

            foreach (var ann in res)
            {
                ann.AttachmentNames = annAtts.Where(x => x.AnnouncementRef == ann.Id).Select(x => x.Attachment.Name).ToList();
            }

            reader.NextResult();
            var owners = reader.ReadList<Person>();
            reader.NextResult();
            var announcementQnAs = AnnouncementQnADataAccess.ReadAnnouncementQnAComplexes(reader);
            reader.NextResult();
            var announcementAttributes = AnnouncementAssignedAttributeDataAccess.ReadAttributes(reader);
            reader.NextResult();
            var announcementApplications = reader.ReadList<AnnouncementApplication>();
            reader.NextResult();
            var announcementStandards = reader.ReadList<AnnouncementStandardDetails>();
            reader.NextResult();
            var announcementAttachments = reader.ReadList<AnnouncementAttachment>(true);
            
            foreach (var ann in res)
            {
                ann.Owner = owners.FirstOrDefault(x => ann.OwnereId == x.Id);
                ann.AnnouncementQnAs = announcementQnAs.Where(x => x.AnnouncementRef == ann.Id).ToList();
                ann.AnnouncementAttributes = announcementAttributes.Where(x => x.AnnouncementRef == ann.Id).ToList();
                ann.AnnouncementApplications = announcementApplications.Where(x => x.AnnouncementRef == ann.Id).ToList();
                ann.AnnouncementStandards = announcementStandards.Where(x => x.AnnouncementRef == ann.Id).ToList();
                ann.AnnouncementAttachments = announcementAttachments.Where(x => x.AnnouncementRef == ann.Id).ToList();
            }
            return res;
        }

        protected abstract TAnnouncement ReadAnnouncementData(AnnouncementComplex announcement, SqlDataReader reader);
           
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
            reader.NextResult();
            var annAtts = reader.ReadList<AnnouncementAttachment>(true);
            foreach (var ann in res.Announcements)
                ann.AttachmentNames = annAtts.Where(x => x.AnnouncementRef == ann.Id).Select(x => x.Attachment.Name).ToList();
            return res;
        }

        protected class AnnouncementCopyResult
        {
            public int FromAnnouncementId { get; set; }
            public int ToAnnouncementId { get; set; }
        }
    }
    
    public class AnnouncementsQuery
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public int? PersonId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? Now { get; set; }
        public bool? Complete { get; set; }
       // public bool OwnedOnly { get; set; }
        public bool Sort { get; set; }
        public bool IncludeFrom { get; set; }
        public bool IncludeTo { get; set; }

        public string FromTitle { get; set; }
        public string ToTitle { get; set; }

        public AnnouncementsQuery()
        {
            Start = 0;
            Count = int.MaxValue;
            IncludeFrom = true;
            IncludeTo = true;
        }
    }

    public enum AnnouncementSortOption
    {
        DueDateAscending = 0,
        DueDateDescending = 1,
        NameAscending = 2,
        NameDescending = 3,
        SectionNameAscending = 4,
        SectionNameDescending = 5
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
