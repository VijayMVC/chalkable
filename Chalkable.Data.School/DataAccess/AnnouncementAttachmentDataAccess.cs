using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

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
            return GetAnnouncementAttachments(conds, callerId, roleId).First();
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

        private DbQuery BuildGetAttachmentQuery(QueryConditionSet queryCondition, int callerId, int roleId, bool needsAllAttachments = true, string filter = null)
        {
            var res = new DbQuery();
            var type = typeof(AnnouncementAttachment);
            res.Sql.AppendFormat(@"select {0}.*
                                   from AnnouncementAttachment 
                                   join Announcement on Announcement.Id = AnnouncementAttachment.AnnouncementRef"
                              , type.Name);


            queryCondition.BuildSqlWhere(res, type.Name);

            if (queryCondition.Count == 0)
                res.Sql.Append(" where 1 = 1 ");
            if (!needsAllAttachments)
                res.Sql.Append(" and AnnouncementAttachment.PersonRef = @callerId");
            
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
                res.Sql.AppendFormat(" and (LOWER(Name) like {0})", filters.JoinString(" or LOWER(Name) like "));
            }

            res.Parameters.Add("@callerId", callerId);
            res.Parameters.Add("@roleId", roleId);
            if (CoreRoles.SUPER_ADMIN_ROLE.Id == roleId)
            {
                return res;
            }
            if (CoreRoles.ADMIN_EDIT_ROLE.Id == roleId || CoreRoles.ADMIN_GRADE_ROLE.Id == roleId ||
                CoreRoles.ADMIN_VIEW_ROLE.Id == roleId)
            {
                res.Sql.Append("and AnnouncementAttachment.PersonRef =@callerId");
                return res;
            }
            if (CoreRoles.TEACHER_ROLE.Id == roleId)
            {
                res.Sql.Append(@" and (Announcement.PersonRef = @callerId or AnnouncementAttachment.PersonRef = Announcement.PersonRef 
                                    or (Announcement.Id in (select AnnouncementRef from AnnouncementRecipient 
                                                           where RoleRef = @roleId or PersonRef = @callerId or ToAll = 1) 
                                         and AnnouncementAttachment.PersonRef = Announcement.PersonRef)
                                 )");
                return res;

            }
            if (CoreRoles.STUDENT_ROLE.Id == roleId)
            {
                res.Sql.Append(@" and (AnnouncementAttachment.PersonRef = @callerId 
                                   or (Announcement.MarkingPeriodClassRef in (select mpc.Id from MarkingPeriodClass mpc
                                                                              join ClassPerson cp on cp.ClassRef = mpc.ClassRef and cp.PersonRef = @callerId)
                                       and AnnouncementAttachment.PersonRef = Announcement.PersonRef)
                                )");
                return res;
            }
            return null;
        }
    }
}
