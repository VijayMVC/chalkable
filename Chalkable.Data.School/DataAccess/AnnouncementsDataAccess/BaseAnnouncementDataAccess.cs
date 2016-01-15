using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
   
        public abstract AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);
        public abstract IList<TAnnouncement> GetAnnouncements(QueryCondition conds, int callerId); 
        public abstract TAnnouncement GetAnnouncement(int id, int callerId);
        public abstract bool CanAddStandard(int announcementId);
        public abstract IList<AnnouncementDetails> GetDetailses(IList<int> ids, int callerId, int? roleId, bool onlyOwner = true); 


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

        //protected AnnouncementDetails GetDetails(string procedureName, IDictionary<string, object> parameters)
        //{
        //    using (var reader = ExecuteStoredProcedureReader(procedureName, parameters))
        //    {
        //        return BuildGetDetailsResult(reader);
        //    }
        //}

        protected IList<AnnouncementDetails> GetDetailses(string procedureName, IDictionary<string, object> parameters)
        {
            using (var reader = ExecuteStoredProcedureReader(procedureName, parameters))
            {
                return BuildGetDetailsesResult(reader);
            }
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

        protected AnnouncementDetails BuildGetDetailsResult(SqlDataReader reader)
        {
            //var announcement = reader.ReadOrNull<AnnouncementDetails>();
            //if (announcement != null)
            //{
            //    announcement.AnnouncementData = ReadAnnouncementData(announcement, reader);
            //    announcement = ReadAnnouncementAdditionalData(announcement, reader);
            //}
            return BuildGetDetailsesResult(reader).FirstOrDefault();
        }

        //old method
        //protected virtual AnnouncementDetails ReadAnnouncementAdditionalData(AnnouncementDetails announcement, SqlDataReader reader)
        //{
        //    reader.NextResult();
        //    if (reader.Read())
        //        announcement.Owner = PersonDataAccess.ReadPersonData(reader);
        //    reader.NextResult();
        //    announcement.AnnouncementQnAs = AnnouncementQnADataAccess.ReadAnnouncementQnAComplexes(reader);
        //    reader.NextResult();
        //    announcement.AnnouncementAttributes = AnnouncementAssignedAttributeDataAccess.ReadAttributes(reader);
        //    reader.NextResult();
        //    announcement.AnnouncementApplications =  reader.ReadList<AnnouncementApplication>();
        //    reader.NextResult();
        //    announcement.AnnouncementStandards = reader.ReadList<AnnouncementStandardDetails>();
        //    reader.NextResult();
        //    announcement.AnnouncementAttachments = reader.ReadList<AnnouncementAttachment>(true);
        //    announcement.StudentAnnouncements = new List<StudentAnnouncementDetails>();
        //    return announcement;
        //}
        
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

    public class AnnouncementDataAccess : DataAccessBase<Announcement>
    {
        public AnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AnnouncementTypeEnum GetAnnouncementType(int announcementId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select  LessonPlan.Id as LessonPlan_Id,
		                                       AdminAnnouncement.Id as AdminAnnouncement_Id,
		                                       ClassAnnouncement.Id as ClassAnnouncement_Id
                                        from Announcement
                                        left join LessonPlan on LessonPlan.Id = Announcement.Id
                                        left join AdminAnnouncement on AdminAnnouncement.Id = Announcement.Id
                                        left join ClassAnnouncement on ClassAnnouncement.Id = Announcement.Id
                                    ");
            new AndQueryCondition{{Announcement.ID_FIELD, announcementId}}.BuildSqlWhere(dbQuery, typeof(Announcement).Name);
            return Read(dbQuery, reader =>
                {
                    reader.Read();
                    if(!reader.IsDBNull(reader.GetOrdinal("LessonPlan_Id")))
                        return AnnouncementTypeEnum.LessonPlan;
                    if(!reader.IsDBNull(reader.GetOrdinal("AdminAnnouncement_Id")))
                        return AnnouncementTypeEnum.Admin;
                    if(!reader.IsDBNull(reader.GetOrdinal("ClassAnnouncement_Id")))
                        return AnnouncementTypeEnum.Class;
                    throw new NoAnnouncementException();
                });
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

        public int? Sort { get; set; }

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
