﻿using System;
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
        public ActionResult AddAttributeAttachment(int announcementType, int announcementId, int assignedAttributeId)
        {
            try
            {
                EnsureAnnouncementExists(announcementType, announcementId);            

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
                var announcement = SchoolLocator.AnnouncementAssignedAttributeService.AddAttributeAttachment((AnnouncementType)announcementType, announcementId, assignedAttributeId, bin, name, uuid);
                AnnouncementViewData res = PrepareAnnouncmentViewDataForEdit(announcement);
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
        public ActionResult RemoveAttributeAttachment(int announcementType, int announcementId, int assignedAttributeId)
        {
            EnsureAnnouncementExists(announcementType, announcementId);
            var announcement = SchoolLocator.AnnouncementAssignedAttributeService.RemoveAttributeAttachment((AnnouncementType)announcementType, announcementId, assignedAttributeId);
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id, (AnnouncementType)announcementType);
            return Json(res, HTML_CONTENT_TYPE, 6);
        }

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AddAttribute(int announcementType, int announcementId, int attributeTypeId)
        {
            EnsureAnnouncementExists(announcementType, announcementId);
            var announcement = SchoolLocator.AnnouncementAssignedAttributeService.Add((AnnouncementType)announcementType, announcementId, attributeTypeId);
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id, (AnnouncementType)announcementType);
            return Json(res, HTML_CONTENT_TYPE, 6);
        }

        private void EnsureAnnouncementExists(int announcementType, int announcementId)
        {
            // get announcement to ensure it exists
            SchoolLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementById(announcementId);
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult DownloadAttributeAttachment(int assignedAttributeId, int announcementType, bool? needsDownload, int? width, int? height)
        {
            var attContentInfo = SchoolLocator.AnnouncementAssignedAttributeService.GetAttributeAttachmentContent(assignedAttributeId, (AnnouncementType)announcementType);
            if (attContentInfo == null)
            {
                Response.StatusCode = 404;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
                return null;
            }

            var attName = attContentInfo.AttachmentName;
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
            var att = SchoolLocator.AnnouncementAssignedAttributeService.GetAssignedAttribyteById(assignedAttributeId);
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
        public ActionResult DeleteAttribute(int announcementType, int announcementId, int assignedAttributeId)
        {
            EnsureAnnouncementExists(announcementType, announcementId);
            var announcement = SchoolLocator.AnnouncementAssignedAttributeService.Delete((AnnouncementType)announcementType, announcementId, assignedAttributeId);
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id, (AnnouncementType)announcementType);
            return Json(res, 6);
        }

    }
}