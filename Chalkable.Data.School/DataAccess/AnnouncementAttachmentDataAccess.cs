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
    public class AnnouncementAttachmentDataAccess : DataAccessBase<AnnouncementAttachment>
    {


        public AnnouncementAttachmentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AnnouncementAttachment GetById(Guid id, Guid callerId, int roleId)
        {
            var conds = new Dictionary<string, object> {{"Id", id}};
            return GetAnnouncementAttachments(conds, callerId, roleId).First();
        }

        public IList<AnnouncementAttachment> GetList(Guid callerId, int roleId, string filter = null)
        {
            var conds = new Dictionary<string, object> ();
            return GetAnnouncementAttachments(conds, callerId, roleId, filter);
        }

        public PaginatedList<AnnouncementAttachment> GetPaginatedList(Guid announcementId, Guid callerId, int roleId, int start, int count, bool needsAllAttachments = true)
        {
            var conds = new Dictionary<string, object> {{"announcementRef", announcementId}};
            var query = BuildGetAttachmentQuery(conds, callerId, roleId, needsAllAttachments);
            if (query == null)
            {
                return new PaginatedList<AnnouncementAttachment>(new List<AnnouncementAttachment>(), start, count);
            }
            return PaginatedSelect<AnnouncementAttachment>(query, "Id", start, count);
        }

        //private const string CALLER_ID = "@callerId"
        private IList<AnnouncementAttachment> GetAnnouncementAttachments(Dictionary<string, object> conds, Guid callerId, int roleId, string filter = null)
        {
            var query = BuildGetAttachmentQuery(conds, callerId, roleId, true, filter);
            return query == null ? new List<AnnouncementAttachment>() : ReadMany<AnnouncementAttachment>(query);
        }

        private DbQuery BuildGetAttachmentQuery(Dictionary<string, object> conds,  Guid callerId, int roleId, bool needsAllAttachments = true, string filter = null)
        {

            var sql = @"select {0}
                        from AnnouncementAttachment 
                        join Announcement on Announcement.Id = AnnouncementAttachment.AnnouncementRef ";
            var b = new StringBuilder();
            b.AppendFormat(sql, "AnnouncementAttachment.*");
            b = Orm.BuildSqlWhere(b, typeof (AnnouncementAttachment), conds);

            if (conds.Count == 0)
                b.Append(" where ");
            if (!needsAllAttachments)
                b.Append(" and AnnouncementAttachment.PersonRef = @callerId");
            
            if (!string.IsNullOrEmpty(filter))
            {
                string[] sl = filter.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var filters = new List<string>();
                if (sl.Length > 0)
                {
                    filters.Add("@filter1");
                    conds.Add("@filter1", string.Format(FILTER_FORMAT, sl[0]));
                }
                if (sl.Length > 1)
                {
                    filters.Add("@filter3");
                    conds.Add("@filter2", string.Format(FILTER_FORMAT, sl[1]));
                }
                if (sl.Length > 2)
                {
                    filters.Add("@filter2");
                    conds.Add("@filter3", string.Format(FILTER_FORMAT, sl[2]));
                }
                b.AppendFormat(" and (LOWER(Name) like {0})", filters.JoinString(" or LOWER(Name) like "));
            }

            conds.Add("@callerId", callerId);
            conds.Add("@roleId", roleId);
            if (CoreRoles.SUPER_ADMIN_ROLE.Id == roleId)
            {
                return new DbQuery { Parameters = conds, Sql = b.ToString() };
            }
            if (CoreRoles.ADMIN_EDIT_ROLE.Id == roleId || CoreRoles.ADMIN_GRADE_ROLE.Id == roleId ||
                CoreRoles.ADMIN_VIEW_ROLE.Id == roleId)
            {
                b.Append("and AnnouncementAttachment.PersonRef =@callerId");
                return new DbQuery { Parameters = conds, Sql = b.ToString() };
            }
            if (CoreRoles.TEACHER_ROLE.Id == roleId)
            {
                b.Append(@" and (Announcement.PersonRef = @callerId or AnnouncementAttachment.PersonRef = Announcement.PersonRef 
                                    or (Announcement.Id in (select AnnouncementRef from AnnouncementRecipient 
                                                           where RoleRef = @roleId or PersonRef = @callerId or ToAll = 1) 
                                         and AnnouncementAttachment.PersonRef = Announcement.PersonRef)
                                 )");
                return new DbQuery { Parameters = conds, Sql = b.ToString() };
            
            }
            if (CoreRoles.STUDENT_ROLE.Id == roleId)
            {
                b.Append(@" and (AnnouncementAttachment.PersonRef = @callerId 
                                   or (Announcement.MarkingPeriodClassRef in (select mpc.Id from MarkingPeriodClass mpc
                                                                              join ClassPerson cp on cp.ClassRef = mpc.ClassRef and cp.PersonRef = @callerId)
                                       and AnnouncementAttachment.PersonRef = Announcement.PersonRef)
                                )");
                return new DbQuery { Parameters = conds, Sql = b.ToString() };
            }
            return null;
        }
    }
}
