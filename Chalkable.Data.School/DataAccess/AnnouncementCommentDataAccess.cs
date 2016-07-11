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

        //TODO make sp for this method
        public AnnouncementComment GetDetailsById(int announcementCommentId, int callerId, int roleId, bool withSubComments = true)
        {
            var idColumn = $"{nameof(AnnouncementComment)}_{nameof(AnnouncementComment.Id)}";
            var conds = new AndQueryCondition {{idColumn, announcementCommentId}};
            var q = Orm.SimpleSelect(AnnouncementComment.VW_ANNOUNCEMENT_COMMENT, conds);
            var res = Read(q, r => 
            {
                r.Read();
                return ReadComment(r);
            });

            if (withSubComments)
            {
                var all = GetList(res.AnnouncementRef, callerId, roleId);
                return BuildSubComments(new List<AnnouncementComment> { res }, all).First();
            }
            return res;
        }

        private IList<AnnouncementComment> GetList(int announcementId, int callerId, int roleId)
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

        public IList<AnnouncementComment> GetCommentsTree(int announcementId, int callerId, int roleId)
        {
            var res = GetList(announcementId, callerId, roleId);
            return BuildCommentsTree(res);
        }
        
        private static IList<AnnouncementComment> ReadCommentsResult(DbDataReader reader)
        {
            var comments = new List<AnnouncementComment>();
            while (reader.Read())
                comments.Add(ReadComment(reader));
            return comments;
        }

        public static IList<AnnouncementComment> BuildCommentsTree(IList<AnnouncementComment> comments)
        {
            var rootComments = comments.Where(x => !x.ParentCommentRef.HasValue).ToList();
            return BuildSubComments(rootComments, comments);
        } 
        private static IList<AnnouncementComment> BuildSubComments(IList<AnnouncementComment> parents, IList<AnnouncementComment> all)
        {
            foreach (var parent in parents)
            {
                var subComments = all.Where(x => x.ParentCommentRef == parent.Id).ToList();
                parent.SubComments = subComments.Count > 0 ? BuildSubComments(subComments, all) : new List<AnnouncementComment>();
            }
            return parents;
        }

        private static AnnouncementComment ReadComment(DbDataReader reader)
        {
            var res = reader.Read<AnnouncementComment>(true);
            if (res.AttachmentRef.HasValue)
                res.Attachment = reader.Read<Attachment>(true);
            return res;
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
