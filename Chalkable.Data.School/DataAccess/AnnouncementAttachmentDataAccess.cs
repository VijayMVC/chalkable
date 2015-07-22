using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementAttachmentDataAccess : DataAccessBase<AnnouncementAttachment, int>
    {


        public AnnouncementAttachmentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AnnouncementAttachment GetById(int id, int callerId, int roleId)
        {
            var conds = new AndQueryCondition { { AnnouncementAttachment.ID_FIELD, id } };
            return GetAnnouncementAttachments(conds, callerId, roleId).FirstOrDefault();
        }

        public IList<AnnouncementAttachment> TakeLastAttachments(int announcementId, int count = int.MaxValue)
        {
            var conds = new AndQueryCondition {{AnnouncementAttachment.ANNOUNCEMENT_REF_FIELD, announcementId}};
            return GetAll(conds).OrderByDescending(x => x.Id).Take(count).OrderBy(x => x.Id).ToList();
        } 

        public IList<AnnouncementAttachment> GetList(int callerId, int roleId, string filter = null)
        {
            return GetAnnouncementAttachments(new AndQueryCondition(), callerId, roleId, filter);
        }

        public PaginatedList<AnnouncementAttachment> GetPaginatedList(int announcementId, int callerId, int roleId, int start, int count, bool needsAllAttachments = true)
        {
            var conds = new AndQueryCondition { { AnnouncementAttachment.ANNOUNCEMENT_REF_FIELD, announcementId } };
            var query = BuildGetAttachmentQuery(conds, callerId, roleId, needsAllAttachments);
            if (query == null)
            {
                return new PaginatedList<AnnouncementAttachment>(new List<AnnouncementAttachment>(), start, count);
            }
            return PaginatedSelect<AnnouncementAttachment>(query, AnnouncementAttachment.ID_FIELD, start, count);
        }

        //private const string CALLER_ID = "@callerId"
        private IList<AnnouncementAttachment> GetAnnouncementAttachments(QueryConditionSet conds, int callerId, int roleId, string filter = null)
        {
            var query = BuildGetAttachmentQuery(conds, callerId, roleId, true, filter);
            return query == null ? new List<AnnouncementAttachment>() : ReadMany<AnnouncementAttachment>(query);
        }


        //TODO: refactor this ... probably move this to stored procedure
        private DbQuery BuildGetAttachmentQuery(QueryConditionSet queryCondition, int callerId, int roleId, bool needsAllAttachments = true, string filter = null)
        {
            var res = new DbQuery();
            var type = typeof(AnnouncementAttachment);
            res.Sql.AppendFormat(@"select [{0}].* from [{0}] 
                                   join [{2}] on [{2}].[{3}] = [{0}].[{1}]"
                              , type.Name, AnnouncementAttachment.ANNOUNCEMENT_REF_FIELD, "Announcement", Announcement.ID_FIELD);

            res.Sql.AppendFormat(@"		
                                    left join LessonPlan on LessonPlan.Id = Announcement.Id
		                            left join ClassAnnouncement on ClassAnnouncement.Id = Announcement.Id
		                            left join AdminAnnouncement on AdminAnnouncement.Id = Announcement.Id
                                ");

            queryCondition.BuildSqlWhere(res, type.Name);
            if (!needsAllAttachments)
                res.Sql.AppendFormat(" and AnnouncementAttachment.PersonRef = @callerId");
            
            if (!string.IsNullOrEmpty(filter))
            {
                string[] sl = filter.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var filters = new List<string>();
                if (sl.Length > 0)
                {
                    filters.Add("@filter1");
                    res.Parameters.Add("@filter1", string.Format(FILTER_FORMAT, sl[0]));
                }
                if (sl.Length > 1)
                {
                    filters.Add("@filter2");
                    res.Parameters.Add("@filter2", string.Format(FILTER_FORMAT, sl[1]));
                }
                if (sl.Length > 2)
                {
                    filters.Add("@filter3");
                    res.Parameters.Add("@filter3", string.Format(FILTER_FORMAT, sl[2]));
                }
                if(filters.Count>0)
                    res.Sql.AppendFormat(" and (LOWER(Name) like {0})", filters.JoinString(" or LOWER(Name) like "));
            }

            res.Parameters.Add("@callerId", callerId);
            res.Parameters.Add("@roleId", roleId);
            if (CoreRoles.SUPER_ADMIN_ROLE.Id == roleId)
            {
                return res;
            }
            if (CoreRoles.DISTRICT_ADMIN_ROLE.Id == roleId)
            {
                res.Sql.Append("and AnnouncementAttachment.PersonRef =@callerId");
                return res;
            }
            if (CoreRoles.TEACHER_ROLE.Id == roleId)
            {
                res.Sql.Append(@" and exists(select * from ClassTeacher 
                                             where (ClassTeacher.PersonRef = @callerId or AnnouncementAttachment.PersonRef = ClassTeacher.PersonRef)
                                                    and (ClassTeacher.ClassRef = LessonPlan.ClassRef or ClassTeacher.ClassRef = ClassAnnouncement.ClassRef))");
                return res;

            }
            if (CoreRoles.STUDENT_ROLE.Id == roleId)
            {
                res.Sql.Append(@" and (AnnouncementAttachment.PersonRef = @callerId 
                                       or (
                                                exists(select * from ClassPerson cp where cp.PersonRef = @callerId and (cp.ClassRef = LessonPlan.ClassRef or cp.ClassRef = ClassAnnouncement.ClassRef))
                                            and 
                                                exists(select ct.ClassRef from ClassTeacher ct where ct.PersonRef = AnnouncementAttachment.PersonRef and (ct.ClassRef = LessonPlan.ClassRef or ct.ClassRef = ClassAnnouncement.ClassRef))
                                          )
                                       or (AdminAnnouncement.Id is not null and exists(select * from AnnouncementGroup aar 
                                                                                        join StudentGroup st on st.GroupRef = aar.GroupRef
																			            where st.StudentRef = @callerId and aar.AnnouncementRef = Announcement.Id)
                                          )
                                       )");
                return res;
            }
            return null;
        }
    }
}
