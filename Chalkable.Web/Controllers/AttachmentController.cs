using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
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
            return Json(attachments
                .Transform(x => AttachmentViewData.Create(x, Context)));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new [] { AppPermissionType.Announcement })]
        public ActionResult List(IntList ids)
        {
            var attachments = new PaginatedList<AttachmentInfo>(ids
                .Select(x => SchoolLocator.AttachementService.GetAttachmentById(x))
                .Select(x => SchoolLocator.AttachementService.TransformToAttachmentInfo(x))
                , 0, ids.Count);

            return Json(attachments.Transform(x => AttachmentViewData.Create(x, Context)));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
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

        public ActionResult PublicAttachment(string r, bool? needsDownload, int? width, int? height)
        {
            if (string.IsNullOrWhiteSpace(r))
            {
                Response.StatusCode = 400;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
                return null;
            }

            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();

            Guid districtId;
            int schoolId;
            Guid userId;
            int attachmentId;
            if (!AttachmentSecurityTools.TryParseAndVerifyRequestStr(r, out districtId, out schoolId, out userId, out attachmentId))
            {
                Response.StatusCode = 400;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
                return null;
            }

            var user = MasterLocator.UserService.GetById(userId);
            SchoolLocator = ServiceLocatorFactory.CreateSchoolLocator(new SchoolUser
            {
                DistrictRef = districtId,
                SchoolRef = schoolId,
                UserRef = user.SchoolUsers.First(x => x.SchoolRef == schoolId).UserRef,
                School = MasterLocator.SchoolService.GetById(districtId, schoolId),
                User = new User
                {
                    Login = string.Empty,
                    LoginInfo = new UserLoginInfo
                    {
                        SisToken = string.Empty,
                        SisTokenExpires = null
                    },
                    DistrictRef = districtId,
                    District = MasterLocator.DistrictService.GetByIdOrNull(districtId)
                }
            });

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

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("SysAdmin, AppTester, AssessmentAdmin, DistrictAdmin, Teacher, Student")]
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

        [AcceptVerbs(HttpVerbs.Put), AuthorizationFilter("SysAdmin, AppTester, AssessmentAdmin, DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult Upload(string fileName)
        {
            var length = Request.InputStream.Length;
            if (length == 0)
                return Json(new ChalkableException(ChlkResources.ERR_FILE_REQUIRED));

            var bin = new byte[length];
            Request.InputStream.Read(bin, 0, (int)length);
            return Json(UploadAttachment(fileName, bin));
        }

        private const string PICTURE_CONTAINER_ADDRESS = "pictureconteiner";
        private AttachmentViewData UploadForSysAdmin(string filename, byte[] bin)
        {
            var key = $"{Context.Role.LoweredName}_{DateTime.UtcNow.ToString("yyyyMMdd_hhmmss")}_{Guid.NewGuid().ToString("N")}";
            SchoolLocator.StorageBlobService.AddBlob(PICTURE_CONTAINER_ADDRESS, key, bin);
            
            return AttachmentViewData.CreateForSysAdmin(filename, key);
        }

        private AttachmentViewData UploadAttachment(string fileName, byte[] bin)
        {
            if (!Context.SchoolId.HasValue || !Context.DistrictId.HasValue)
                return UploadForSysAdmin(fileName, bin);

            Trace.Assert(Context.PersonId.HasValue);

            var attachment = SchoolLocator.AttachementService.Upload(fileName, bin);
            var res = SchoolLocator.AttachementService.TransformToAttachmentInfo(attachment, new List<int> { Context.PersonId.Value });
            return AttachmentViewData.Create(res, Context, true);
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult StartViewSession(int attachmentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var att = SchoolLocator.AttachementService.GetAttachmentById(attachmentId);
            if (att == null)
            {
                Response.StatusCode = 404;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
                return null;
            }
            try
            {
                bool isOwner = (Context.PersonId == att.PersonRef);
                var canAnnotate = isOwner;
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
    }
}