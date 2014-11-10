using System;
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
    public class AnnouncementAttachmentController : AnnouncementBaseController
    {
        private const string HEADER_FORMAT = "inline; filename={0}";
        private const string CONTENT_DISPOSITION = "Content-Disposition";       
        private const string HTML_CONTENT_TYPE = "text/html";

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult AddAttachment(int announcementId)
        {
            try
            {
                EnsureAnnouncementExsists(announcementId);            

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
                return new ChalkableJsonResult(false)
                {
                    Data = ExceptionViewData.Create(exception, exception.InnerException),
                    ContentType = HTML_CONTENT_TYPE,
                    SerializationDepth = 4
                };
            }
        }

        private void EnsureAnnouncementExsists(int announcementId)
        {
            // get announcement to ensure it exists
            SchoolLocator.AnnouncementService.GetAnnouncementById(announcementId);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult CloneAttachment(int originalAttachmentId, int announcementId)
        {
            EnsureAnnouncementExsists(announcementId);

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

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult DownloadAttachment(int announcementAttachmentId, bool? needsDownload, int? width, int? height)
        {
            var attContentInfo = SchoolLocator.AnnouncementAttachmentService.GetAttachmentContent(announcementAttachmentId);
            if (attContentInfo == null)
            {
                Response.StatusCode = 404;
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

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult DeleteAttachment(int announcementAttachmentId, int announcementId)
        {
            EnsureAnnouncementExsists(announcementId);

            var attachment = SchoolLocator.AnnouncementAttachmentService.GetAttachmentById(announcementAttachmentId);
            if (attachment != null && attachment.AnnouncementRef == announcementId)
                SchoolLocator.AnnouncementAttachmentService.DeleteAttachment(announcementAttachmentId);

            var res = PrepareFullAnnouncementViewData(announcementId);
            return Json(res, 6);
        }
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetAttachments(int announcementId, int? start, int? count)
        {
            EnsureAnnouncementExsists(announcementId);

            var announcementAttachments = SchoolLocator.AnnouncementAttachmentService.GetAttachments(announcementId, start ?? 0, count ?? 10, false);
            var attachmentsInfo = AttachmentLogic.PrepareAttachmentsInfo(announcementAttachments);
            var res = AnnouncementAttachmentViewData.Create(attachmentsInfo, SchoolLocator.Context.PersonId ?? 0);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StartViewSession(int announcementAttachmentId)
        {
            var att = SchoolLocator.AnnouncementAttachmentService.GetAttachmentById(announcementAttachmentId);
            if (att == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            try
            {
                var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.PersonId ?? 0);
                bool isOwner = (person.Id == att.PersonRef);
                var canAnnotate = isOwner || person.RoleRef != CoreRoles.STUDENT_ROLE.Id;
                string name = person.FirstName;
                if (string.IsNullOrEmpty(name))
                {
                    var user = MasterLocator.UserService.GetById(SchoolLocator.Context.UserId);
                    name = user.Login;
                }
                var res = SchoolLocator.CrocodocService.StartViewSession(new StartViewSessionRequestModel
                    {
                        Uuid = att.Uuid,
                        CanAnnotate = canAnnotate,
                        PersonId = person.Id,
                        PersonName = name,
                        IsOwner = isOwner
                    });
                return Json(res.session);
            }
            catch (Exception exception)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                return new ChalkableJsonResult(false)
                {
                    Data = ExceptionViewData.Create(exception, exception.InnerException),
                    ContentType = HTML_CONTENT_TYPE,
                    SerializationDepth = 4
                };
            }
        }

    }
}