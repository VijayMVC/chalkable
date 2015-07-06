using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Controllers.AnnouncementControllers;
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
        public ActionResult AddAttributeAttachment(int announcementType, int announcementId)
        {
            /*try
            {
                EnsureAnnouncementExists(announcementType, announcementId);            

                byte[] bin;
                string name;
                if (!GetFileFromRequest(out bin, out name))
                {
                    return Json(new ChalkableException(ChlkResources.ERR_FILE_REQUIRED));
                }
                string uuid = nul;Al
                if (SchoolLocator.CrocodocService.IsDocument(name))
                {
                    uuid = SchoolLocator.CrocodocService.UploadDocument(name, bin).uuid;
                }
                var announcement = SchoolLocator.AnnouncementAttachmentService.AddAttachment(announcementType, announcementId, bin, name, uuid);
                AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id, (AnnouncementType)announcementType);
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
            }*/
            throw new NotImplementedException();
        }

        [AcceptVerbs(HttpVerbs.Post), AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AddAttribute(int announcementType, int announcementId, int attributeTypeId)
        {
            EnsureAnnouncementExists(announcementType, announcementId);
            var announcement = SchoolLocator.AnnouncementAssignedAttributeService.Add(announcementType, announcementId, attributeTypeId);
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id, (AnnouncementType)announcementType);
            return Json(res, HTML_CONTENT_TYPE, 6);
        }

        private void EnsureAnnouncementExists(int announcementType, int announcementId)
        {
            // get announcement to ensure it exists
            SchoolLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementById(announcementId);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult CloneAttribute(int originalAttachmentId, int announcementId)
        {
            throw new NotImplementedException();
            /*EnsureAnnouncementExists(announcementId);

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
            return Json(res, 6);*/
        }




       
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult DeleteAttribute(int announcementType, int announcementId, int assignedAttributeId)
        {

            EnsureAnnouncementExists(announcementType, announcementId);
            var announcement = SchoolLocator.AnnouncementAssignedAttributeService.Delete(announcementType, announcementId, assignedAttributeId);
            AnnouncementViewData res = PrepareFullAnnouncementViewData(announcement.Id, (AnnouncementType)announcementType);
            return Json(res, 6);

            //check if there is attachment remove it and remove it from inow

            /*

            var attachment = SchoolLocator.AnnouncementAttachmentService.GetAttachmentById(assignedAttributeId);
            if (attachment != null && attachment.AnnouncementRef == announcementId)
                //SchoolLocator.AnnouncementAttachmentService.DeleteAttachment(announcementAttachmentId);

            var res = PrepareFullAnnouncementViewData(announcementId);*/
            //return Json(res, 6);
        }

    }
}