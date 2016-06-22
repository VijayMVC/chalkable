using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AttachmentController : ChalkableController
    {
        private const string CONTENT_DISPOSITION = "Content-Disposition";
        private const string HEADER_FORMAT = "inline; filename={0}";

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult AttachmentsList(string filter, int? sortType, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var attachments = SchoolLocator.AttachementService.GetAttachmentsInfo(filter, (AttachmentSortTypeEnum?) sortType, start.Value, count.Value);
            return Json(attachments.Transform(x=> AttachmentViewData.Create(x, Context.PersonId.Value)));
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult DownloadAttachment(int attachmentId, bool? needsDownload, int? width, int? height)
        {
            //TODO: security here
            var attContentInfo = SchoolLocator.AttachementService.GetAttachmentContent(attachmentId);
            if (attContentInfo == null)
            {
                Response.StatusCode = 404;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
                return null;
            }
            var attName = attContentInfo.Attachment.Name;
            var content = attContentInfo.Content;
            var contentTypeName = MimeHelper.GetContentTypeByName(attName);
            if (MimeHelper.GetTypeByName(attName) == MimeHelper.AttachmenType.Picture && width.HasValue && height.HasValue)
            {
                content = ImageUtils.Scale(content, width.Value, height.Value);
            }
            if (needsDownload.HasValue && !needsDownload.Value)
            {
                Response.AddHeader(CONTENT_DISPOSITION, string.Format(HEADER_FORMAT, attName));
                return File(content, contentTypeName);
            }
            return File(content, contentTypeName, attName);
        }

    }
}