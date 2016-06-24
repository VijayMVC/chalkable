using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class TeacherCommentController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult CommentsList(int teacherId)
        {
            var res = SchoolLocator.TeacherCommentService.GetComments(teacherId);
            return Json(TeacherCommentViewData.Create(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Create(string comment)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var res = SchoolLocator.TeacherCommentService.AddComment(Context.PersonId.Value, comment);
            return Json(TeacherCommentViewData.Create(res));
        }
        
        [AuthorizationFilter("Teacher")]
        public ActionResult Update(int commentId, string comment)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var res = SchoolLocator.TeacherCommentService.EditComment(commentId, Context.PersonId.Value, comment);
            return Json(TeacherCommentViewData.Create(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Delete(int commentId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            SchoolLocator.TeacherCommentService.DeleteComment(commentId, Context.PersonId.Value);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult DeleteComments(IntList commentIds)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            SchoolLocator.TeacherCommentService.DeleteComments(commentIds, Context.PersonId.Value);
            return Json(true);
        }

    }
}