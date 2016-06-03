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

        public AnnouncementAttachment GetById(int id, int callerId, int roleId, bool hasAdminClassPermission = false)
        {
            var idField = $"{nameof (AnnouncementAttachment)}_{nameof(AnnouncementAttachment.Id)}";
            var conds = new AndQueryCondition { { idField, id } };
            return GetAnnouncementAttachments(conds, callerId, roleId, hasAdminClassPermission : hasAdminClassPermission).FirstOrDefault();
        }

        public IList<AnnouncementAttachment> GetLastAttachments(IList<int> announcementIds, int count = int.MaxValue)
        {
            var annIdsStr = announcementIds.Select(x => x.ToString()).JoinString(",");
            var dbQuery = new DbQuery();
            var annRefField = $"{nameof (AnnouncementAttachment)}_{nameof(AnnouncementAttachment.AnnouncementRef)}";
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, "*", AnnouncementAttachment.VW_ANNOUNCEMENT_ATTACHMENT)
                       .AppendFormat(" Where {0} in ({1})", annRefField, annIdsStr);

            return ReadMany<AnnouncementAttachment>(dbQuery, true).OrderByDescending(x => x.Id).Take(count).OrderBy(x => x.Id).ToList();
        }

        public IList<AnnouncementAttachment> GetByAnnouncementIds(IList<int> announcementIds, IList<int> owners)
        {
            var query = new DbQuery();
            query.Sql.Append(
                $@"Select * 
                   From vw{nameof(AnnouncementAttachment)}
                   Where  {nameof(AnnouncementAttachment)}_{nameof(AnnouncementAttachment.AnnouncementRef)} in (Select * From @announcementIds)");

            if (owners != null && owners.Count != 0)
                query.Sql.Append($" And {nameof(Attachment)}_{nameof(Attachment.PersonRef)} in (Select * From @owners)");

            query.Parameters.Add("announcementIds", announcementIds);
            query.Parameters.Add("owners", owners);

            return ReadMany<AnnouncementAttachment>(query, true);
        } 

        public IList<AnnouncementAttachment> GetLastAttachments(int announcementId, int count = int.MaxValue)
        {
            return GetLastAttachments(new List<int> {announcementId},  count);
        } 

        public IList<AnnouncementAttachment> GetList(int callerId, int roleId, string filter = null)
        {
            return GetAnnouncementAttachments(new AndQueryCondition(), callerId, roleId, filter);
        }

        public PaginatedList<AnnouncementAttachment> GetPaginatedList(int announcementId, int callerId, int roleId, int start, int count, bool needsAllAttachments = true)
        {
            var annRefField = $"{nameof (AnnouncementAttachment)}_{nameof(AnnouncementAttachment.AnnouncementRef)}";
            var conds = new AndQueryCondition { { annRefField, announcementId } };
            var query = BuildGetAttachmentQuery(conds, callerId, roleId, needsAllAttachments);
            if (query == null)
            {
                return new PaginatedList<AnnouncementAttachment>(new List<AnnouncementAttachment>(), start, count);
            }
            return PaginatedSelect<AnnouncementAttachment>(query, nameof(AnnouncementAttachment.Id), start, count, Orm.OrderType.Asc, true);
        }

        //private const string CALLER_ID = "@callerId"
        private IList<AnnouncementAttachment> GetAnnouncementAttachments(QueryConditionSet conds, int callerId, int roleId, string filter = null, bool hasAdminClassPermission = false)
        {
            var query = BuildGetAttachmentQuery(conds, callerId, roleId, true, filter, hasAdminClassPermission);
            return query == null ? new List<AnnouncementAttachment>() : ReadMany<AnnouncementAttachment>(query, true);
        }

        //TODO: refactor this ... probably move this to stored procedure
        private DbQuery BuildGetAttachmentQuery(QueryConditionSet queryCondition, int callerId, int roleId, bool needsAllAttachments = true, string filter = null, bool hasAdminClassPermission = false)
        {
            var res = new DbQuery();
            var annRefField = $"{nameof(AnnouncementAttachment)}_{nameof(AnnouncementAttachment.AnnouncementRef)}";
            res.Sql.AppendFormat(@"select [{0}].* from [{0}] 
                                   join [{2}] on [{2}].[{3}] = [{0}].[{1}]"
                              , AnnouncementAttachment.VW_ANNOUNCEMENT_ATTACHMENT, annRefField
                              , nameof(Announcement), nameof(Announcement.Id));

            res.Sql.AppendFormat(@"		
                                    left join LessonPlan on LessonPlan.Id = Announcement.Id
		                            left join ClassAnnouncement on ClassAnnouncement.Id = Announcement.Id
		                            left join AdminAnnouncement on AdminAnnouncement.Id = Announcement.Id
                                ");

            queryCondition.BuildSqlWhere(res, AnnouncementAttachment.VW_ANNOUNCEMENT_ATTACHMENT);
            if (!needsAllAttachments)
                res.Sql.AppendFormat(" and Attachment_PersonRef = @callerId");
            
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
                    res.Sql.AppendFormat(" and (LOWER(Attachment_Name) like {0})", filters.JoinString(" or LOWER(Attachment_Name) like "));
            }

            res.Parameters.Add("@callerId", callerId);
            res.Parameters.Add("@roleId", roleId);
            if (CoreRoles.SUPER_ADMIN_ROLE.Id == roleId)
            {
                return res;
            }

            if (CoreRoles.DISTRICT_ADMIN_ROLE.Id == roleId || CoreRoles.TEACHER_ROLE.Id == roleId)
            {
                if (CoreRoles.DISTRICT_ADMIN_ROLE.Id == roleId)
                {
                    res.Sql.Append("and (AdminAnnouncement.Id is null or Attachment_PersonRef = @callerId)");
                }
                if (CoreRoles.TEACHER_ROLE.Id == roleId)
                {
                    res.Sql.Append(" and AdminAnnouncement.Id is null ");
                }
                if (!hasAdminClassPermission)
                {
                    res.Sql.Append("and (AdminAnnouncement.Id is not null or ");
                    res.Sql.Append(@" exists(select * from ClassTeacher 
                                             where (ClassTeacher.PersonRef = @callerId or Attachment_PersonRef = ClassTeacher.PersonRef)
                                                    and (ClassTeacher.ClassRef = LessonPlan.ClassRef or ClassTeacher.ClassRef = ClassAnnouncement.ClassRef))");
                    res.Sql.Append(")");
                }
                return res;
            }
            
            if (CoreRoles.STUDENT_ROLE.Id == roleId)
            {
                res.Sql.Append(@" and (Attachment_PersonRef = @callerId 
                                       or (
                                                exists(select * from ClassPerson cp where cp.PersonRef = @callerId and (cp.ClassRef = LessonPlan.ClassRef or cp.ClassRef = ClassAnnouncement.ClassRef))
                                            and 
                                                exists(select ct.ClassRef from ClassTeacher ct where ct.PersonRef = Attachment_PersonRef and (ct.ClassRef = LessonPlan.ClassRef or ct.ClassRef = ClassAnnouncement.ClassRef))
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
