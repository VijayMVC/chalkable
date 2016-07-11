using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{

    [RequireHttps, TraceControllerFilter]
    public class AnnouncementCommentController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult PostComment(int announcementId, string text, int? attachmentId)
        {
            var res = SchoolLocator.AnnouncementCommentService.PostComment(announcementId, text, attachmentId);
            return Json(PrepareCommentViewData(res, SchoolLocator));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Reply(int toCommentId, string text, int? attachmentId)
        {
            var res = SchoolLocator.AnnouncementCommentService.Reply(toCommentId, text, attachmentId);
            return Json(PrepareCommentViewData(res, SchoolLocator));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Edit(int announcementCommentId, string text, int? attachmentId)
        {
            var res = SchoolLocator.AnnouncementCommentService.Edit(announcementCommentId, text, attachmentId);
            return Json(PrepareCommentViewData(res, SchoolLocator));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SetHidden(int announcementCommentId, bool hidden)
        {
            var comment = SchoolLocator.AnnouncementCommentService.GetById(announcementCommentId);
            SchoolLocator.AnnouncementCommentService.SetHidden(announcementCommentId, hidden);
            var res = PrepareListOfCommentViewData(SchoolLocator.AnnouncementCommentService.GetList(comment.AnnouncementRef), SchoolLocator);
            return Json(res, 20);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Delete(int announcementCommentId)
        {
            var comment = SchoolLocator.AnnouncementCommentService.GetById(announcementCommentId);
            SchoolLocator.AnnouncementCommentService.Delete(announcementCommentId);
            var res = PrepareListOfCommentViewData(SchoolLocator.AnnouncementCommentService.GetList(comment.AnnouncementRef), SchoolLocator);
            return Json(res);
        }

        public static IList<AnnouncementCommentViewData> PrepareListOfCommentViewData(IEnumerable<AnnouncementComment> comments, IServiceLocatorSchool serviceLocator)
        {
            return comments.Select(x=>PrepareCommentViewData(x, serviceLocator)).ToList();
        }

        public static AnnouncementCommentViewData PrepareCommentViewData(AnnouncementComment comment, IServiceLocatorSchool serviceLocator)
        {
            AttachmentInfo attachmentInfo = null;
            if (comment.Attachment != null)
                attachmentInfo = serviceLocator.AttachementService.TransformToAttachmentInfo(comment.Attachment);
            var res = AnnouncementCommentViewData.Create(comment, attachmentInfo, serviceLocator.Context.PersonId.Value);
            if (comment.SubComments != null && comment.SubComments.Count > 0)
            {
                res.SubComments = new List<AnnouncementCommentViewData>();
                foreach (var subComment in comment.SubComments)
                {
                    res.SubComments.Add(PrepareCommentViewData(subComment, serviceLocator));
                }
            }
            return res;
        }
    }
}