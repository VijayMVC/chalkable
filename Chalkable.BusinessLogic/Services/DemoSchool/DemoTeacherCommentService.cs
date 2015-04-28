using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoTeacherCommentService : DemoSchoolServiceBase, ITeacherCommentService
    {
        public DemoTeacherCommentService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public TeacherComment AddComment(int teacherId, string comment)
        {
           return Storage.TeacherCommentStorage.Add(new TeacherComment
                {
                    Comment = comment,
                    TeacherId = teacherId,
                });
        }

        public TeacherComment EditComment(int commentId, int teacherId, string comment)
        {
            Storage.TeacherCommentStorage.Update(new TeacherComment
            {
                CommentId = commentId,
                Comment = comment,
                TeacherId = teacherId,
            });
            return Storage.TeacherCommentStorage.GetById(commentId);
        }

        public void DeleteComment(int commentId, int teacherId)
        {
            DeleteComments(new List<int> {commentId}, teacherId);
        }

        public void DeleteComments(IList<int> commentsIds, int teacherId)
        {
            foreach (var commentId in commentsIds)
            {
                Storage.TeacherCommentStorage.Delete(commentId);
            }
        }

        public IList<TeacherComment> GetComments(int teacherId)
        {
            return Storage.TeacherCommentStorage.GetAll().Where(x => x.TeacherId == teacherId).ToList();
        }
    }
}
