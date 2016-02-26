using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.Api.SampleApp.Logic;
using Chalkable.Api.SampleApp.Models;
using Chalkable.API.Exceptions;
using Chalkable.API.Models;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class TeacherController : BaseSampleAppController
    {
        public ActionResult Index()
        {
            PrepareBaseData(null);
            return View("App");
        }

        public async Task<ActionResult> Attach(int announcementApplicationId, string contentId)
        {
            var ps = new RouteValueDictionary {{"announcementApplicationId", announcementApplicationId}};
            if (string.IsNullOrWhiteSpace(contentId))
                return RedirectToAction("SimpleAttach", ps);
            ps.Add("contentId", contentId);
            return RedirectToAction("ContentAttach", ps);
        }

        public async Task<ActionResult> SimpleAttach(int announcementApplicationId)
        {
            PrepareBaseData(announcementApplicationId);
            var annApp = await Connector.Announcement.GetAnnouncementApplicationById(announcementApplicationId);
            var ann = await Connector.Announcement.GetRead(annApp.AnnouncementId, annApp.AnnouncementType);

       //   var studentAnnouncementAppicationIds = await Connector.Announcement.StudentAnnouncementAppicationIds(179);
            var studentAnnouncementAppicationIdsForTeacher = await Connector.Announcement.StudentAnnouncementAppicationIdsForTeacher(3787, 179);
            return View("Attach", DefaultJsonViewData.Create(ann));
        }

        public async Task<ActionResult> ContentAttach(int announcementApplicationId, string contentId)
        {
            if(string.IsNullOrWhiteSpace(contentId))
                throw new ChalkableApiException("Invalid param. ContentId is missing");
            var content = ContentStorage.GetStorage().GetContentById(contentId);
            if(content == null)
                throw new ChalkableApiException("Content not found");
            PrepareBaseData(announcementApplicationId);
            var annApp = await Connector.Announcement.GetAnnouncementApplicationById(announcementApplicationId);
            var updateAnnAppMeta = await Connector.Announcement.UpdateAnnouncementApplicationMeta(announcementApplicationId, annApp.AnnouncementType, content.Text, content.ImageUrl);
            return View("Attach", DefaultJsonViewData.Create(new {content}));
        }

        private async Task<Announcement> GetAnnouncement(int announcementApplicationId)
        {
            var annApp = await Connector.Announcement.GetAnnouncementApplicationById(announcementApplicationId);
            return await Connector.Announcement.GetRead(annApp.AnnouncementId, annApp.AnnouncementType);
        }


        public ActionResult ViewMode(int announcementApplicationId)
        {
            return RedirectToAction("Attach", new RouteValueDictionary
            {
                {"announcementApplicationId", announcementApplicationId}
            });
        }

        public ActionResult GradingViewMode(int announcementApplicationId, int studentId)
        {
            throw new NotImplementedException();
        }
    }
}