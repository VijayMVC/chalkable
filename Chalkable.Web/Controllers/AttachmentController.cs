using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Common.Enums;
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
        
        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult Upload()
        {
            try
            {
                byte[] bin;
                string name;
                if (!GetFileFromRequest(out bin, out name))
                    return Json(new ChalkableException(ChlkResources.ERR_FILE_REQUIRED));
                
                return Json(UploadAttachment(name, bin), HTML_CONTENT_TYPE, 6);
            }
            catch (Exception exception)
            {
                return HandleAttachmentException(exception);
            }
        }

        [AcceptVerbs(HttpVerbs.Put), AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult Upload(string fileName)
        {
            var length = Request.InputStream.Length;
            if (length == 0)
                return Json(new ChalkableException(ChlkResources.ERR_FILE_REQUIRED));

            var bin = new byte[length];
            Request.InputStream.Read(bin, 0, (int)length);
            return Json(UploadAttachment(fileName, bin));
        }

        private AttachmentViewData UploadAttachment(string fileName, byte[] bin)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var attachment = SchoolLocator.AttachementService.Upload(fileName, bin);
            var res = SchoolLocator.AttachementService.TransformToAttachmentInfo(attachment, new List<int> { Context.PersonId.Value });
            return AttachmentViewData.Create(res, true);
        }
    }
}