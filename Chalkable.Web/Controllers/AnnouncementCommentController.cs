using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web.Controllers
{

    [RequireHttps, TraceControllerFilter]
    public class AnnouncementCommentController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult PostComment(int announcementId, string text, int? attachmentId)
        {
            SchoolLocator.AnnouncementCommentService.PostComment(announcementId, text, attachmentId);
            throw new NotImplementedException();
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Reply(int toCommentId, string text, int? attachmentId)
        {
            SchoolLocator.AnnouncementCommentService.Reply(toCommentId, text, attachmentId);
            throw new NotImplementedException();
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SetHidden(int commentId, bool hidden)
        {
            SchoolLocator.AnnouncementCommentService.SetHidden(commentId, hidden);
            throw new NotImplementedException();
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Delete(int commentId)
        {
            SchoolLocator.AnnouncementCommentService.Delete(commentId);
            throw new NotImplementedException();
        }
    }
}