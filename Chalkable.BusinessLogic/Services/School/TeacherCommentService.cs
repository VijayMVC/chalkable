using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ITeacherCommentService
    {
        TeacherComment AddComment(int teacherId, string comment);
        TeacherComment EditComment(int commentId, int teacherId, string comment);
        void DeleteComment(int commentId, int teacherId);
        void DeleteComments(IList<int> commentsIds, int teacherId);
        IList<TeacherComment> GetComments(int teacherId);
        
    }

    public class TeacherCommentService : SisConnectedService, ITeacherCommentService
    {
        public TeacherCommentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public TeacherComment AddComment(int teacherId, string comment)
        {
            EnsureIsDistrictAdminOrCurrentTeacher(teacherId);
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var stiComment = new GradebookCommect {Comment = comment, TeacherId = teacherId};
            stiComment = ConnectorLocator.SectionCommentConnector.CreateComment(syId, teacherId, stiComment);
            return stiComment != null ? TeacherComment.Create(stiComment) : null;
        }


        public TeacherComment EditComment(int commentId, int teacherId, string comment)
        {
            EnsureIsDistrictAdminOrCurrentTeacher(teacherId);
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var stiComment = new GradebookCommect {Comment = comment, TeacherId = teacherId, Id = commentId};
            ConnectorLocator.SectionCommentConnector.UpdateComment(syId, teacherId, stiComment);
            return TeacherComment.Create(stiComment);
        }

        public void DeleteComment(int commentId, int teacherId)
        {
            DeleteComments(new List<int>{commentId}, teacherId);
        }

        public void DeleteComments(IList<int> commentsIds, int teacherId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            if (!BaseSecurity.IsDistrictOrTeacher(Context))
                throw new ChalkableSecurityException();
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            foreach (var commentId in commentsIds)
            {
                ConnectorLocator.SectionCommentConnector.DeleteComment(syId, teacherId, commentId);
            }
        }

        public IList<TeacherComment> GetComments(int teacherId)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var stiComments = ConnectorLocator.SectionCommentConnector.GetComments(syId, teacherId) ?? new List<GradebookCommect>();
            return stiComments.Select(TeacherComment.Create).ToList();
        }

        private void EnsureIsDistrictAdminOrCurrentTeacher(int teacherId)
        {
            if (!(BaseSecurity.IsDistrictAdmin(Context) || (Context.Role == CoreRoles.TEACHER_ROLE && Context.PersonId == teacherId)))
                throw new ChalkableSecurityException();
        }
    }
}
