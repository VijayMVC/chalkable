using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class TeacherCommentStorage : BaseDemoIntStorage<TeacherComment>
    {
        public TeacherCommentStorage() : base(x=> x.CommentId.HasValue ? x.CommentId.Value : 0, true)
        {
        }
    }

    public class DemoTeacherCommentService : DemoSchoolServiceBase, ITeacherCommentService
    {
        private TeacherCommentStorage TeacherCommentStorage { get; set; }

        public DemoTeacherCommentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            TeacherCommentStorage = new TeacherCommentStorage();
        }

        public TeacherComment AddComment(int teacherId, string comment)
        {
           return TeacherCommentStorage.Add(new TeacherComment
                {
                    Comment = comment,
                    TeacherId = teacherId,
                });
        }

        public TeacherComment EditComment(int commentId, int teacherId, string comment)
        {
            TeacherCommentStorage.Update(new TeacherComment
            {
                CommentId = commentId,
                Comment = comment,
                TeacherId = teacherId,
            });
            return TeacherCommentStorage.GetById(commentId);
        }

        public void DeleteComment(int commentId, int teacherId)
        {
            DeleteComments(new List<int> {commentId}, teacherId);
        }

        public void DeleteComments(IList<int> commentsIds, int teacherId)
        {
            foreach (var commentId in commentsIds)
            {
                TeacherCommentStorage.Delete(commentId);
            }
        }

        public IList<TeacherComment> GetComments(int teacherId)
        {
            return TeacherCommentStorage.GetAll().Where(x => x.TeacherId == teacherId).ToList();
        }
    }
}
