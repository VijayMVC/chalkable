using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementCommentDataAccess : DataAccessBase<AnnouncementComment, int>
    {
        public AnnouncementCommentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AnnouncementComment> GetList(int announcementId, int callerId, int roleId)
        {
            var ps = new Dictionary<string, object>
            {
                ["announcementId"] = announcementId,
                ["callerId"] = callerId,
                ["roleId"] = roleId
            };
            using (var reader = ExecuteStoredProcedureReader("spGetAnnouncementComments", ps))
            {
                return ReadCommentsResult(reader);
            }
        }

        public static IList<AnnouncementComment> ReadCommentsResult(DbDataReader reader)
        {
            var comments = reader.ReadList<AnnouncementComment>(true);
            var rootComments = comments.Where(x => !x.ParentCommentRef.HasValue).ToList();
            return BuildSubComments(rootComments, comments);
        } 
        private static IList<AnnouncementComment> BuildSubComments(IList<AnnouncementComment> parents, IList<AnnouncementComment> all)
        {
            foreach (var parent in parents)
            {
                var subComments = all.Where(x => x.ParentCommentRef == parent.Id).ToList();
                parent.SubComments = BuildSubComments(subComments, all);
            }
            return parents;
        } 
        
        public void HideAll(int announcementId)
        {
            //TODO: hide only students comments
            var conds = new AndQueryCondition {{nameof(AnnouncementComment.AnnouncementRef), announcementId}};
            var ps = new Dictionary<string, object> {{nameof(AnnouncementComment.Hidden), true}};
            var q = Orm.SimpleUpdate<AnnouncementComment>(ps, conds);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }
    }
}
