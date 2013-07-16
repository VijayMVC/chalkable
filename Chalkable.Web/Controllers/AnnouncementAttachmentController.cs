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

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementAttachmentController : AnnouncementBaseController
    {
        private const string headerFormat = "inline; filename={0}";
        private const string CONTENT_DISPOSITION = "Content-Disposition";
        
        private const string TOKEN = "token";
        private const string FILE_CONTENT_TYPE = "file";
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

        private const string BOUNDARY_CONTENT_TYPE_FMT = "multipart/form-data; boundary={0}";
        private const string FORM_ITEM_CONTENT_DISPOSITION = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        private const string HEADER_CONTENT_DISPOSITION =
            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
        
        private static DocumentUploadResponse HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc, byte[] bytes)
        {
            string boundary = "----" + DateTime.UtcNow.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = string.Format(BOUNDARY_CONTENT_TYPE_FMT, boundary);
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;
            Stream rs = wr.GetRequestStream();
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(FORM_ITEM_CONTENT_DISPOSITION, key, nvc[key]);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }

            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string header = string.Format(HEADER_CONTENT_DISPOSITION, paramName, file, contentType);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            Stream fileStream = new MemoryStream(bytes);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();
            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                var reader2 = new StreamReader(stream2);
                string resp = reader2.ReadToEnd();
                var res = Serializer<DocumentUploadResponse>(resp);
                return res;
            }
            catch (Exception)
            {
                if (wresp != null)
                    wresp.Close();
            }
            return null;
        }
        
        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult AddAttachment(Guid announcementId)
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
                var nvc = new NameValueCollection {{TOKEN, PreferenceService.Get(Preference.CROCODOC_TOKEN).Value}};
                var storagedocUrl = PreferenceService.Get(Preference.CROCODOC_API_URL).Value;
                var uploadRes = HttpUploadFile(UrlTools.UrlCombine(storagedocUrl, DOCUMENT_UPLOAD), name, FILE_CONTENT_TYPE,
                                               MimeHelper.GetContentTypeByName(name), nvc, bin);
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
        public ActionResult DownloadAttachment(Guid id, bool? needsDownload, int? width, int? height)
        {
            var attContentInfo = SchoolLocator.AnnouncementAttachmentService.GetAttachmentContent(id);
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
        public ActionResult DeleteAttachment(Guid announcementAttachmentId)
        {
            var attachment = SchoolLocator.AnnouncementAttachmentService.GetAttachmentById(announcementAttachmentId);
            SchoolLocator.AnnouncementAttachmentService.DeleteAttachment(announcementAttachmentId);
            var res = PrepareFullAnnouncementViewData(attachment.AnnouncementRef);
            return Json(res, 6);
        }
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetAttachments(Guid announcementId, int? start, int? count)
        {
            var announcementAttachments = SchoolLocator.AnnouncementAttachmentService.GetAttachments(announcementId, start ?? 0, count ?? 10, false);
            var attachmentsInfo = AttachmentLogic.PrepareAttachmentsInfo(announcementAttachments);
            var res = AnnouncementAttachmentViewData.Create(attachmentsInfo, SchoolLocator.Context.UserId);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StartViewSession(Guid announcementAttachmentId)
        {
            var att = SchoolLocator.AnnouncementAttachmentService.GetAttachmentById(announcementAttachmentId);
            var attinfo = AttachmentLogic.PrepareAttachmentInfo(att);
            try
            {
                var wc = new WebClient();
                var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.UserId);
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