using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model.Announcements.Sis;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Controllers.AnnouncementControllers;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementAttachmentController : AnnouncementBaseController
    {
        private const string HTML_CONTENT_TYPE = "text/html";

        private const string HEADER_FORMAT = "inline; filename={0}";
        private const string CONTENT_DISPOSITION = "Content-Disposition";
        
        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult UploadAnnouncementAttachment(int announcementId, int announcementType)
        {
            try
            {
                EnsureAnnouncementExsists(announcementId, announcementType);            

                byte[] bin;
                string name;
                if (!GetFileFromRequest(out bin, out name))
                {
                    return Json(new ChalkableException(ChlkResources.ERR_FILE_REQUIRED));
                }
                var announcement = SchoolLocator.AnnouncementAttachmentService.UploadAttachment(announcementId, (AnnouncementType)announcementType, bin, name);
                AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id, (AnnouncementType)announcementType);
                return Json(res, HTML_CONTENT_TYPE, 6);
            }
            catch (Exception exception)
            {
                return HandleAttachmentException(exception);
            }
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult Add(int announcementId, int announcementType, int attachmentId)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);
            SchoolLocator.AnnouncementAttachmentService.Add(announcementId, (AnnouncementType)announcementType, attachmentId);
            var res = PrepareFullAnnouncementViewData(announcementId, (AnnouncementType) announcementType);
            return Json(res, 6);
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult DownloadAttachment(int attachmentId, bool? needsDownload, int? width, int? height)
        {
            var att = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachmentById(attachmentId).Attachment;
            var attContentInfo = SchoolLocator.AttachementService.GetAttachmentContent(att);
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

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult StartViewSession(int announcementAttachmentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var att = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachmentById(announcementAttachmentId).Attachment;
            if (att == null)
            {
                Response.StatusCode = 404;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
                return null;
            }
            try
            {
                bool isOwner = (Context.PersonId == att.PersonRef);
                var canAnnotate = isOwner || Context.Role != CoreRoles.STUDENT_ROLE;
                var person = SchoolLocator.PersonService.GetPerson(Context.PersonId.Value);
                string name = person.FirstName;
                if (string.IsNullOrEmpty(name))
                {
                    name = Context.Login;
                }
                var res = SchoolLocator.CrocodocService.StartViewSession(new StartViewSessionRequestModel
                {
                    Uuid = att.Uuid,
                    CanAnnotate = canAnnotate,
                    PersonId = Context.PersonId.Value,
                    PersonName = name,
                    IsOwner = isOwner
                });
                return Json(res.session);
            }
            catch (Exception exception)
            {
                return HandleAttachmentException(exception);
            }
        }
        private ActionResult HandleAttachmentException(Exception exception)
        {
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 500;
            Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
            return new ChalkableJsonResult(false)
            {
                Data = ExceptionViewData.Create(exception, exception.InnerException),
                ContentType = HTML_CONTENT_TYPE,
                SerializationDepth = 4
            };
        }


        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult CloneAttachment(int originalAttachmentId, int announcementId, int announcementType)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);

            //var annAtt = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachmentById(originalAttachmentId);
            var attContentInfo = SchoolLocator.AttachementService.GetAttachmentContent(originalAttachmentId); //SchoolLocator.AttachementService.GetAttachmentContent(annAtt.AttachmentRef);
            if (attContentInfo != null) //&& announcementId == annAtt.AnnouncementRef)
            {
                byte[] bin = attContentInfo.Content;
                string name = attContentInfo.Attachment.Name;
                SchoolLocator.AnnouncementAttachmentService.UploadAttachment(announcementId, (AnnouncementType)announcementType, bin, name);
            }
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcementId, (AnnouncementType)announcementType);
            return Json(res, 6);
        }

        
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult DeleteAttachment(int announcementAttachmentId, int announcementId, int announcementType)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);

            var attachment = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachmentById(announcementAttachmentId);
            if (attachment != null && attachment.AnnouncementRef == announcementId)
                SchoolLocator.AnnouncementAttachmentService.Delete(announcementAttachmentId);

            var res = PrepareFullAnnouncementViewData(announcementId, (AnnouncementType)announcementType);
            return Json(res, 6);
        }
        
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult GetAttachments(int announcementId, int? announcementType, int? start, int? count)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);

            var announcementAttachments = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachments(announcementId, start ?? 0, count ?? 10, false);
            var attachmentsInfo = SchoolLocator.AnnouncementAttachmentService.TransformToAttachmentsInfo(announcementAttachments, null);
            var res = AnnouncementAttachmentViewData.Create(attachmentsInfo, SchoolLocator.Context.PersonId ?? 0);
            return Json(res);
        }

        
    }
}