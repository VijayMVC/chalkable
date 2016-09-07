using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Common;

namespace Chalkable.Web.Controllers
{

    [RequireHttps, TraceControllerFilter]
    public class AnnouncementCommentController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult PostComment(int announcementId, string text, IntList attachmentIds)
        {
            var res = SchoolLocator.AnnouncementCommentService.PostComment(announcementId, text, attachmentIds);
            return Json(GetList(res.AnnouncementRef));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Reply(int toCommentId, string text, IntList attachmentIds)
        {
            var res = SchoolLocator.AnnouncementCommentService.Reply(toCommentId, text, attachmentIds);
            return Json(GetList(res.AnnouncementRef));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Edit(int announcementCommentId, string text, IntList attachmentIds)
        {
            var res = SchoolLocator.AnnouncementCommentService.Edit(announcementCommentId, text, attachmentIds);
            return Json(GetList(res.AnnouncementRef));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SetHidden(int announcementCommentId, bool hidden)
        {
            var comment = SchoolLocator.AnnouncementCommentService.GetById(announcementCommentId);
            SchoolLocator.AnnouncementCommentService.SetHidden(announcementCommentId, hidden);
            return Json(GetList(comment.AnnouncementRef), 20);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Delete(int announcementCommentId)
        {
            var comment = SchoolLocator.AnnouncementCommentService.GetById(announcementCommentId);
            SchoolLocator.AnnouncementCommentService.Delete(announcementCommentId);
            return Json(GetList(comment.AnnouncementRef));
        }

        private IList<AnnouncementCommentViewData> GetList(int announcementId)
        {
            return PrepareListOfCommentViewData(SchoolLocator.AnnouncementCommentService.GetList(announcementId), SchoolLocator);
        } 

        public static IList<AnnouncementCommentViewData> PrepareListOfCommentViewData(IEnumerable<AnnouncementComment> comments, IServiceLocatorSchool serviceLocator)
        {
            return comments.Select(x=>PrepareCommentViewData(x, serviceLocator)).ToList();
        }
        public static AnnouncementCommentViewData PrepareCommentViewData(AnnouncementComment comment, IServiceLocatorSchool serviceLocator)
        {
            Trace.Assert(serviceLocator.Context.PersonId.HasValue);

            var attachmentInfo = comment.Attachments?.Select(x=>serviceLocator.AttachementService.TransformToAttachmentInfo(x)).ToList() ?? new List<AttachmentInfo>();
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