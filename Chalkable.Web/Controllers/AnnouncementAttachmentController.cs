﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementAttachmentController : AnnouncementBaseController
    {
        private const string headerFormat = "inline; filename={0}";
        private const string CONTENT_DISPOSITION = "Content-Disposition";       
        private const string HTML_CONTENT_TYPE = "text/html";

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult AddAttachment(int announcementId)
        {
            byte[] bin;
            string name;
            if (!GetFileFromRequest(out bin, out name))
            {
                return Json(new ChalkableException(ChlkResources.ERR_FILE_REQUIRED));
            }
            string uuid = null;
            if (SchoolLocator.CrocodocService.IsDocument(name))
            {
                try
                {
                    uuid = SchoolLocator.CrocodocService.UploadDocument(name, bin).uuid;
                }
                catch (ChalkableException exception)
                {
                    return Json(exception);
                }
            }
            var announcement = SchoolLocator.AnnouncementAttachmentService.AddAttachment(announcementId, bin, name, uuid);
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id);
            return Json(res, HTML_CONTENT_TYPE, 6);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult DownloadAttachment(int announcementAttachmentId, bool? needsDownload, int? width, int? height)
        {
            var attContentInfo = SchoolLocator.AnnouncementAttachmentService.GetAttachmentContent(announcementAttachmentId);
            var attName = attContentInfo.Attachment.Name;
            var content = attContentInfo.Content;
            var contentTypeName = MimeHelper.GetContentTypeByName(attName);
            if (MimeHelper.GetTypeByName(attName) == MimeHelper.AttachmenType.Picture && width.HasValue && height.HasValue)
            {
                content = ImageUtils.Scale(content, width.Value, height.Value);
            }
            if (needsDownload.HasValue && !needsDownload.Value)
            {
                Response.AddHeader(CONTENT_DISPOSITION, string.Format(headerFormat, attName));
                return File(content, contentTypeName);
            }
            return File(content, contentTypeName, attName);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult DeleteAttachment(int announcementAttachmentId)
        {
            var attachment = SchoolLocator.AnnouncementAttachmentService.GetAttachmentById(announcementAttachmentId);
            SchoolLocator.AnnouncementAttachmentService.DeleteAttachment(announcementAttachmentId);
            var res = PrepareFullAnnouncementViewData(attachment.AnnouncementRef);
            return Json(res, 6);
        }
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetAttachments(int announcementId, int? start, int? count)
        {
            var announcementAttachments = SchoolLocator.AnnouncementAttachmentService.GetAttachments(announcementId, start ?? 0, count ?? 10, false);
            var attachmentsInfo = AttachmentLogic.PrepareAttachmentsInfo(announcementAttachments);
            var res = AnnouncementAttachmentViewData.Create(attachmentsInfo, SchoolLocator.Context.UserLocalId ?? 0);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StartViewSession(int announcementAttachmentId)
        {
            var att = SchoolLocator.AnnouncementAttachmentService.GetAttachmentById(announcementAttachmentId);
            try
            {
                var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.UserLocalId ?? 0);
                bool isOwner = (person.Id == att.PersonRef);
                var canAnnotate = isOwner || person.RoleRef != CoreRoles.STUDENT_ROLE.Id;
                string name = person.FirstName;
                if (string.IsNullOrEmpty(name))
                    name = person.Email;
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
            catch (Exception ex)
            {
                return Json(new ChalkableException(ex.Message, ex));
            }
        }

    }
}