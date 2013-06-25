using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
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

        public IList<AnnouncementAttachment> GetList(Guid callerId, int roleId)
        {
            var conds = new Dictionary<string, object> ();
            return GetAnnouncementAttachments(conds, callerId, roleId);
        }

        public PaginatedList<AnnouncementAttachment> GetPaginatedList(Guid announcementId, Guid callerId, int roleId, int start, int count, bool needsAllAttachments = true)
        {
            var conds = new Dictionary<string, object> {{"announcementRef", announcementId}};
            var query = BuildGetAttachmentQuery(conds, callerId, roleId);
            if (query == null)
            {
                return new PaginatedList<AnnouncementAttachment>(new List<AnnouncementAttachment>(), start, count);
            }
            return PaginatedSelect<AnnouncementAttachment>(query, "Id", start, count);
        }

        //private const string CALLER_ID = "@callerId"
        private IList<AnnouncementAttachment> GetAnnouncementAttachments(Dictionary<string, object> conds, Guid callerId, int roleId)
        {
            var query = BuildGetAttachmentQuery(conds, callerId, roleId);
            if (query == null)
            {
                return new List<AnnouncementAttachment>();
            }
            using (var reader = ExecuteReaderParametrized(query.Sql, query.Parameters as Dictionary<string,object>))
            {
                return reader.ReadList<AnnouncementAttachment>();
            }
        }

        private DbQuery BuildGetAttachmentQuery(Dictionary<string, object> conds,  Guid callerId, int roleId, bool needsAllAttachments = true)
        {

            var sql = @"select {0}
                        from AnnouncementAttachment 
                        join Announcement on Announcement.Id = AnnouncementAttachment.AnnouncementRef";
            var b = new StringBuilder();
            b.AppendFormat(sql, "AnnouncementAttachment.*");
            b = Orm.BuildSqlWhere(b, typeof (AnnouncementAttachment), conds);

            if (!needsAllAttachments)
                b.Append(" and AnnouncementAttachment.PersonRef = @callerId");

            conds.Add("@callerId", callerId);
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
                                                           where RoleRef = @roleId || PersonRef = @callerId || All = 1) 
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
