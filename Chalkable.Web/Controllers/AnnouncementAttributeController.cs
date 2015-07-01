using System;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementAttributeController : AnnouncementBaseController
    {
        private const string HEADER_FORMAT = "inline; filename={0}";
        private const string CONTENT_DISPOSITION = "Content-Disposition";       
        private const string HTML_CONTENT_TYPE = "text/html";

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AddAttribute(int announcementId)
        {
            try
            {
                EnsureAnnouncementExists(announcementId);            

                byte[] bin;
                string name;
                if (!GetFileFromRequest(out bin, out name))
                {
                    return Json(new ChalkableException(ChlkResources.ERR_FILE_REQUIRED));
                }
                string uuid = null;
                if (SchoolLocator.CrocodocService.IsDocument(name))
                {
                    uuid = SchoolLocator.CrocodocService.UploadDocument(name, bin).uuid;
                }
                var announcement = SchoolLocator.AnnouncementAttachmentService.AddAttachment(announcementId, bin, name, uuid);
                AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id);
                return Json(res, HTML_CONTENT_TYPE, 6);
            }
            catch (Exception exception)
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
        }

        private void EnsureAnnouncementExists(int announcementId)
        {
            // get announcement to ensure it exists
            SchoolLocator.AnnouncementService.GetAnnouncementById(announcementId);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult CloneAttribute(int originalAttachmentId, int announcementId)
        {
            EnsureAnnouncementExists(announcementId);

            var attContentInfo = SchoolLocator.AnnouncementAttachmentService.GetAttachmentContent(originalAttachmentId);
            if (attContentInfo != null && announcementId == attContentInfo.Attachment.AnnouncementRef)
            {
                byte[] bin = attContentInfo.Content;
                string name = attContentInfo.Attachment.Name;
                string uuid = null;
                if (SchoolLocator.CrocodocService.IsDocument(name))
                {
                    uuid = SchoolLocator.CrocodocService.UploadDocument(name, bin).uuid;
                }
                
                SchoolLocator.AnnouncementAttachmentService.AddAttachment(attContentInfo.Attachment.AnnouncementRef, bin, name, uuid);
            }
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcementId);
            return Json(res, 6);
        }

       
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult DeleteAttribute(int announcementAttachmentId, int announcementId)
        {
            EnsureAnnouncementExists(announcementId);

            var attachment = SchoolLocator.AnnouncementAttachmentService.GetAttachmentById(announcementAttachmentId);
            if (attachment != null && attachment.AnnouncementRef == announcementId)
                SchoolLocator.AnnouncementAttachmentService.DeleteAttachment(announcementAttachmentId);

            var res = PrepareFullAnnouncementViewData(announcementId);
            return Json(res, 6);
        }
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult GetAttributes(int announcementId, int? start, int? count)
        {
            EnsureAnnouncementExists(announcementId);

            var announcementAttachments = SchoolLocator.AnnouncementAttachmentService.GetAttachments(announcementId, start ?? 0, count ?? 10, false);
            var attachmentsInfo = AttachmentLogic.PrepareAttachmentsInfo(announcementAttachments, MasterLocator.CrocodocService);
            var res = AnnouncementAttachmentViewData.Create(attachmentsInfo, SchoolLocator.Context.PersonId ?? 0);
            return Json(res);
        }

    }
}