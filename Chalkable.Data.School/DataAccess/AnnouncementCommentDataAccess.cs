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
            var queries = new List<DbQuery> {Orm.SimpleSelect(AnnouncementComment.VW_ANNOUNCEMENT_COMMENT, conds)};

            var attsQuery = new DbQuery();
            attsQuery.Sql.Append($"Select * From {nameof(AnnouncementCommentAttachment)} ")
                .Append($"Join {nameof(Attachment)} ")
                .Append($" On {nameof(Attachment)}.{nameof(Attachment.Id)} = ")
                .Append($" {nameof(AnnouncementCommentAttachment)}.{nameof(AnnouncementCommentAttachment.AttachmentRef)}");

            var attsConds = new AndQueryCondition {{nameof(AnnouncementCommentAttachment.AnnouncementCommentRef), announcementCommentId}};
            attsConds.BuildSqlWhere(attsQuery, nameof(AnnouncementCommentAttachment));

            queries.Add(attsQuery);
            var res = Read(new DbQuery(queries), ReadCommentsResult).FirstOrDefault();

            if (withSubComments && res != null)
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

            reader.NextResult();
            var annCommentattachments = reader.ReadList<AnnouncementCommentAttachment>();
            foreach (var comment in comments)
            {
                comment.Attachments = annCommentattachments
                    .Where(x=>x.AnnouncementCommentRef == comment.Id)
                    .Select(x=>x.Attachment).ToList();
            }
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



    public class AnnouncementCommentAttachmentDataAccess : DataAccessBase<AnnouncementCommentAttachment>
    {
        public AnnouncementCommentAttachmentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void PostAttachements(int announcementCommentId, IList<int> attachmentIds)
        {
            IList<DbQuery> queries = new List<DbQuery>();
            var deleteConds = new AndQueryCondition
            {
                {nameof(AnnouncementCommentAttachment.AnnouncementCommentRef), announcementCommentId}
            };
            queries.Add(Orm.SimpleDelete<AnnouncementCommentAttachment>(deleteConds));

            if (attachmentIds != null)
            {
                var annCommentAtts = attachmentIds.Select(x => new AnnouncementCommentAttachment
                {
                    AnnouncementCommentRef = announcementCommentId,
                    AttachmentRef = x
                }).ToList();
                queries.Add(Orm.SimpleListInsert(annCommentAtts, false));
            }

            var q = new DbQuery(queries);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }
    }
}
