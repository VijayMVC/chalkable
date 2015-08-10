using System;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Controllers.AnnouncementControllers;
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
        public ActionResult UploadAttachment(int announcementType, int announcementId, int assignedAttributeId)
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
                var attr = SchoolLocator.AnnouncementAssignedAttributeService.UploadAttachment((AnnouncementType)announcementType, announcementId, assignedAttributeId, bin, name);
                var attrAttachmentInfo = SchoolLocator.AttachementService.TransformToAttachmentInfo(attr.Attachment);
                var res = AnnouncementAssignedAttributeViewData.Create(attr, attrAttachmentInfo);
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

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AddAttachment(int announcementType, int announcementId, int assignedAttributeId, int attachmentId)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);            
            var attr = SchoolLocator.AnnouncementAssignedAttributeService.AddAttachment((AnnouncementType) announcementType, announcementId, assignedAttributeId, attachmentId);
            var attrAttachmentInfo = SchoolLocator.AttachementService.TransformToAttachmentInfo(attr.Attachment);
            var res = AnnouncementAssignedAttributeViewData.Create(attr, attrAttachmentInfo);
            return Json(res, HTML_CONTENT_TYPE, 6);
        }

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult RemoveAttachment(int announcementType, int announcementId, int announcementAssignedAttributeId)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);            
            var attr = SchoolLocator.AnnouncementAssignedAttributeService.RemoveAttachment(announcementAssignedAttributeId);
            var res = AnnouncementAssignedAttributeViewData.Create(attr, null);
            return Json(res, HTML_CONTENT_TYPE, 6);
        }

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AddAttribute(int announcementType, int announcementId, int attributeTypeId)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);            
            var assignedAttr = SchoolLocator.AnnouncementAssignedAttributeService.Add((AnnouncementType)announcementType, announcementId, attributeTypeId);
            var attrAttachmentInfo = assignedAttr.Attachment != null  ? SchoolLocator.AttachementService.TransformToAttachmentInfo(assignedAttr.Attachment) : null;
            var res = AnnouncementAssignedAttributeViewData.Create(assignedAttr, attrAttachmentInfo);
            return Json(res, HTML_CONTENT_TYPE, 6);
        }
        
        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult CloneAttachment(int attachmentId, int announcementId, int announcementType, int announcementAssignedAttributeId)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);
            var attribute = SchoolLocator.AnnouncementAssignedAttributeService.GetAssignedAttributeById(announcementAssignedAttributeId);
            var attContentInfo = SchoolLocator.AttachementService.GetAttachmentContent(attachmentId);
            if (attContentInfo != null && announcementId == attribute.AnnouncementRef)
            {
                byte[] bin = attContentInfo.Content;
                string name = attContentInfo.Attachment.Name;
                SchoolLocator.AnnouncementAssignedAttributeService.UploadAttachment((AnnouncementType)announcementType, announcementId, announcementAssignedAttributeId, bin, name);
            }
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcementId, (AnnouncementType)announcementType);
            return Json(res, 6);
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult DownloadAttributeAttachment(int assignedAttributeId, int announcementType, bool? needsDownload, int? width, int? height)
        {
            var attribute = SchoolLocator.AnnouncementAssignedAttributeService.GetAssignedAttributeById(assignedAttributeId);
            var attContentInfo = SchoolLocator.AttachementService.GetAttachmentContent(attribute.Attachment);
            
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
        public ActionResult StartViewSession(int assignedAttributeId)
        {
            var att = SchoolLocator.AnnouncementAssignedAttributeService.GetAssignedAttributeById(assignedAttributeId).Attachment;
            if (att == null)
            {
                Response.StatusCode = 404;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
                return null;
            }

            try
            {
                var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.PersonId ?? 0);
                var canAnnotate = person.RoleRef != CoreRoles.STUDENT_ROLE.Id;
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
                });
                return Json(res.session);
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

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult DeleteAttribute(int announcementType, int announcementId, int announcementAssignedAttributeId)
        {
            EnsureAnnouncementExsists(announcementId, announcementType);
            SchoolLocator.AnnouncementAssignedAttributeService.Delete(announcementAssignedAttributeId);
            return Json(true);
        }

    }
}