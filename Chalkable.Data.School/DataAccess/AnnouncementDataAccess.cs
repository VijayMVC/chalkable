using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementDataAccess : DataAccessBase
    {
        public AnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        private const string CREATE_PORCEDURE = "spCreateAnnouncement";
        private const string GET_DETAILS_PROCEDURE = "spGetAnnouncementDetails";
        private const string GET_STUDENT_ANNOUNCEMENTS = "spGetStudentAnnouncements";
        private const string GET_TEACHER_ANNOUNCEMENTS = "spGetTeacherAnnouncements";
        private const string GET_ADMIN_ANNOUNCEMENTS = "spGetAdminAnnouncements";
        private const string DELETE_PROCEDURE = "spDeleteAnnouncement";
        private const string REORDER_PROCEDURE = "spReorderAnnouncements";

        private const string ANNOUNCEMENT_TYPE_ID_PARAM = "announcementTypeId";
        private const string CREATED_PARAM = "created";
        private const string EXPIRES_PARAM = "expires";
        private const string PERSON_ID_PARAM = "personId";
        private const string STATE_PARAM = "state";
        private const string GRADING_STYLE_PARAM = "gradingStyle";
        private const string CLASS_ID_PARAM = "classId";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";

        private const string OWNER_ID_PARAM = "ownerId";
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string ID_PARAM = "id";
        private const string CALLER_ID_PARAM = "callerId";
        private const string ROLE_ID_PARAM = "roleId";
        private const string FROM_DATE_PARAM = "fromDate";
        private const string TO_DATE_PARAM = "toDate";
        
        private AnnouncementDetails BuildGetDetailsResult(SqlDataReader reader)
        {
            var announcement = reader.ReadOrNull<AnnouncementDetails>();
            reader.NextResult();
            announcement.StudentAnnouncements = new List<StudentAnnouncementDetails>();
            while (reader.Read())
            {
                var studenAnn = reader.Read<StudentAnnouncementDetails>();
                studenAnn.Person = reader.Read<Person>();
                studenAnn.Person.StudentInfo = reader.Read<StudentInfo>();
                announcement.StudentAnnouncements.Add(studenAnn);
            }
            reader.NextResult();
            announcement.AnnouncementAttachments = reader.ReadList<AnnouncementAttachment>();
            reader.NextResult();
            announcement.AnnouncementReminders = reader.ReadList<AnnouncementReminder>();
            reader.NextResult();
            announcement.Owner = reader.ReadOrNull<Person>();
            return announcement;
        }
        private AnnouncementQueryResult GetAnnouncementsComplex(string procedureName, Dictionary<string, object> parameters, AnnouncementsQuery query)
        {
            parameters.Add(ROLE_ID_PARAM, query.RoleId);
            parameters.Add(PERSON_ID_PARAM, query.PersonId);
            parameters.Add(MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId);
            parameters.Add(FROM_DATE_PARAM, query.FromDate);
            parameters.Add(TO_DATE_PARAM, query.ToDate);
            parameters.Add(CLASS_ID_PARAM, query.ClassId);
            parameters.Add("start", query.Start);
            parameters.Add("count", query.Count);
            parameters.Add("now", query.Now);
            parameters.Add("ownedOnly", query.OwnedOnly);
            parameters.Add("staredOnly", query.StarredOnly);
            using (var reader = ExecuteStoredProcedureReader(procedureName, parameters))
            {
                var res = new AnnouncementQueryResult { Query = query };
                bool first = true;
                while (reader.Read())
                {
                    res.Announcements.Add(reader.Read<AnnouncementComplex>());
                    if (first)
                    {
                        res.SourceCount = SqlTools.ReadInt32(reader, "AllCount");
                        first = false;
                    }
                }
                return res;
            }
        }
   
        public AnnouncementDetails Create(int announcementTypeId, Guid? classId, Guid markingPeriodId, DateTime created, Guid personId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ANNOUNCEMENT_TYPE_ID_PARAM, announcementTypeId},
                    {PERSON_ID_PARAM, personId},
                    {CREATED_PARAM, created},
                    {EXPIRES_PARAM, DateTime.MinValue},
                    {STATE_PARAM, AnnouncementState.Draft},
                    {GRADING_STYLE_PARAM, GradingStyleEnum.Numeric100},
                    {CLASS_ID_PARAM, classId},
                    {MARKING_PERIOD_ID_PARAM, markingPeriodId}
                };

            using (var reader = ExecuteStoredProcedureReader(CREATE_PORCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }
        
        public void Update(Announcement announcement)
        {
            SimpleUpdate(announcement);
        }

        public void Delete(Guid? id, Guid? personId, Guid? classId, int? announcementTypeId, AnnouncementState? state)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, id},
                    {PERSON_ID_PARAM, personId},
                    {CLASS_ID_PARAM, classId},
                    {ANNOUNCEMENT_TYPE_ID_PARAM, announcementTypeId},
                    {STATE_PARAM, state}
                };
            ExecuteStoredProcedureReader(DELETE_PROCEDURE, parameters);
        }
        
        public Announcement GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{ID_PARAM, id}};
            return  SelectOne<Announcement>(conds);
        }

        public AnnouncementQueryResult GetStudentAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>{{"gradedOnly", query.GradedOnly}};
            return GetAnnouncementsComplex(GET_STUDENT_ANNOUNCEMENTS, parameters, query);
        }
        public AnnouncementQueryResult GetTeacherAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"gradedOnly", query.GradedOnly},
                    {"allSchoolItems", query.AllSchoolItems}
                };
            return GetAnnouncementsComplex(GET_TEACHER_ANNOUNCEMENTS, parameters, query);
        }
        public AnnouncementQueryResult GetAdminAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object> { { "gradeLevelsIds", query.GradeLevelIds } };
            return GetAnnouncementsComplex(GET_ADMIN_ANNOUNCEMENTS, parameters, query);
        }
        public void ReorderAnnouncements(Guid schoolYearId, int announcementTypeId, Guid ownerId, Guid recipientId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId},
                    {ANNOUNCEMENT_TYPE_ID_PARAM, announcementTypeId},
                    {OWNER_ID_PARAM, ownerId},
                    {CLASS_ID_PARAM, recipientId}
                };
            using (ExecuteStoredProcedureReader(REORDER_PROCEDURE, parameters))
            {
            }
        }
        public AnnouncementDetails GetDetails(Guid id, Guid callerId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, id},
                    {CALLER_ID_PARAM, callerId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_DETAILS_PROCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }
        
        
        
        private bool IsAdmin(int roleId)
        {
            return CoreRoles.SUPER_ADMIN_ROLE.Id == roleId
                   || CoreRoles.ADMIN_EDIT_ROLE.Id == roleId
                   || CoreRoles.ADMIN_GRADE_ROLE.Id == roleId
                   || CoreRoles.ADMIN_VIEW_ROLE.Id == roleId;
        }
        public Announcement GetAnnouncement(Guid id, int roleId, Guid callerId)
        {
            var b = new StringBuilder();
            b.Append("select Announcement.* from Announcement ");
            bool firstConds = true;
            var conds = new Dictionary<string, object>();
            if (IsAdmin(roleId))
                b.Append(" where ");
            if (CoreRoles.TEACHER_ROLE.Id == roleId)
            {
                b.Append(@" where Announcement.PersonRef = @callerId 
                            or (AnnouncementTypeRef == @adminType and Announcement.Id in (select ar.AnnouncementRef from AnnouncementRecipient ar 
                                                                                          where ar.ToAll = 1 or ar.PersonRef = @callerId or ar.RoleRef = @roleId))");
                conds.Add("callerId", callerId);
                conds.Add("adminType", (int)SystemAnnouncementType.Admin);
                conds.Add("@roleId", roleId);
                firstConds = false;
            }
            if (CoreRoles.STUDENT_ROLE.Id == roleId)
            {
                b.Append(@" where ((Announcement.MarkingPeriodClassRef in (select mpc.Id from MarkingPeriodClass mpc
                                                                         join ClassPerson cp on cp.ClassRef = mpc.ClassRef
                                                                         where cp.PersonRef = @callerId)) 
                            or (AnnouncementTypeRef == @adminType 
                                     and Announcement.Id in (select ar.AnnouncementRef from AnnouncementRecipient ar 
                                                             where ar.ToAll = 1 or ar.PersonRef = @callerId or ar.RoleRef = @roleId
                                                                   or ar.GradeLevelRef in (select GradeLevelRef from StudentInfo where Id = @callerId)
                                                            )
                                ))");

                conds.Add("callerId", callerId);
                conds.Add("adminType", (int)SystemAnnouncementType.Admin);
                conds.Add("@roleId", roleId);
                firstConds = false;
            }

            if (!firstConds) b.Append(" and ");
            b.Append(" Announcement.Id =@id");

            conds.Add("id", id);
            using (var reader = ExecuteReaderParametrized(b.ToString(), conds))
            {
                return reader.ReadOrNull<Announcement>();
            }
        }
    }

    public class AnnouncementsQuery
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public Guid? Id { get; set; }
        public int? RoleId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? Now { get; set; }
        public bool StarredOnly { get; set; }
        public bool OwnedOnly { get; set; }
        public bool GradedOnly { get; set; }
        public bool AllSchoolItems { get; set; }

        public IEnumerable<Guid> GradeLevelIds { get; set; }

        public AnnouncementsQuery()
        {
            Start = 0;
            Count = int.MaxValue;
        }
    }
    public class AnnouncementQueryResult
    {
        public List<AnnouncementComplex> Announcements { get; set; }
        public int SourceCount { get; set; }
        public AnnouncementsQuery Query { get; set; }
    }

}
