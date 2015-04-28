﻿using System.Collections.Generic;
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
            var stiComment = new SectionComment {Comment = comment, TeacherId = teacherId};
            stiComment = ConnectorLocator.SectionCommentConnector.CreateComment(syId, teacherId, stiComment);
            return TeacherComment.Create(stiComment);
        }


        public TeacherComment EditComment(int commentId, int teacherId, string comment)
        {
            EnsureIsDistrictAdminOrCurrentTeacher(teacherId);
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var stiComment = new SectionComment {Comment = comment, TeacherId = teacherId, Id = commentId};
            ConnectorLocator.SectionCommentConnector.UpdateComment(syId, teacherId, stiComment);
            return TeacherComment.Create(stiComment);
        }

        public void DeleteComment(int commentId, int teacherId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            if(BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            ConnectorLocator.SectionCommentConnector.DeleteComment(syId, teacherId, commentId);
        }

        public IList<TeacherComment> GetComments(int teacherId)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var stiComments = ConnectorLocator.SectionCommentConnector.GetComments(syId, teacherId);
            return stiComments.Select(TeacherComment.Create).ToList();
        }

        private void EnsureIsDistrictAdminOrCurrentTeacher(int teacherId)
        {
            if (!(BaseSecurity.IsDistrict(Context) || (Context.Role == CoreRoles.TEACHER_ROLE && Context.PersonId == teacherId)))
                throw new ChalkableSecurityException();
        }
    }
}
