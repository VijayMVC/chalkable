using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
        
        private const string TOKEN = "token";
        private const string HTML_CONTENT_TYPE = "text/html";
        private const string UUID = "uuid";
        private const string ADMIN = "ADMIN";
        private const string USER = "USER";
        private const string EDITABLE = "EDITABLE";
        private const string SESSION_CREATE = "session/create";
        private const string DOCUMENT_UPLOAD = "document/upload";




        private static T Serializer<T>(string response)
        {
            var s = new JavaScriptSerializer();
            var jsonResponse = s.Deserialize<T>(response);
            return jsonResponse;
        }

        private class DocumentUploadResponse
        {
            public string uuid { get; set; }
        }
        private class StartSessionResponse
        {
            public string session { get; set; }
        }

        private static DocumentUploadResponse UploadFileToCrocodoc(string fileName, byte[] fileContent)
        {
            var nvc = new NameValueCollection {{TOKEN, PreferenceService.Get(Preference.CROCODOC_TOKEN).Value}};
            var storagedocUrl = PreferenceService.Get(Preference.CROCODOC_API_URL).Value;
            var fileType = MimeHelper.GetContentTypeByName(fileName);
            return ChalkableHttpFileLoader.HttpUploadFile(UrlTools.UrlCombine(storagedocUrl, DOCUMENT_UPLOAD)
                , fileName, fileContent, fileType, null, Serializer<DocumentUploadResponse>, nvc);            
        }
        
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
            if (MimeHelper.GetTypeByName(name) == MimeHelper.AttachmenType.Document)
            {
                var uploadRes = UploadFileToCrocodoc(name, bin);
                if (uploadRes == null)
                {
                    return Json(new ChalkableException(ChlkResources.ERR_PROCESSING_FILE));
                }
                uuid = uploadRes.uuid;
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
            var attinfo = AttachmentLogic.PrepareAttachmentInfo(att);
            try
            {
                var wc = new WebClient();
                var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.UserLocalId ?? 0);
                bool isOwner = (person.Id == att.PersonRef);
                var canAnnotate = isOwner || person.RoleRef != CoreRoles.STUDENT_ROLE.Id;
                var nameValue = new NameValueCollection
                    {
                        {TOKEN, attinfo.Token},
                        {UUID, att.Uuid},
                        {EDITABLE, canAnnotate.ToString().ToLower()}
                    };
                string name = person.FirstName;
                if (string.IsNullOrEmpty(name))
                    name = person.Email;
                nameValue.Add(USER, string.Format("{0},{1}", person.Id, name));
                nameValue.Add(ADMIN, isOwner.ToString().ToLower());

                var str = Encoding.ASCII.GetString(wc.UploadValues(UrlTools.UrlCombine(attinfo.StorageUrl, SESSION_CREATE), nameValue));
                var res = Serializer<StartSessionResponse>(str);
                return Json(res.session);
            }
            catch (Exception ex)
            {
                return Json(new ChalkableException(ex.Message, ex));
            }
        }

    }
}