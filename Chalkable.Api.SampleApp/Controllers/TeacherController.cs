using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.Api.SampleApp.Logic;
using Chalkable.Api.SampleApp.Models;
using Chalkable.API.Exceptions;

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

        private static IList<string> defaultAbIds = new List<string>
        {
            "7FCA0002-7440-11DF-93FA-01FD9CFF4B22",
            "DB06A266-DE81-11E0-831A-85489DFF4B22",
        };

        public async Task<ActionResult> SimpleAttach(int announcementApplicationId)
        {
            PrepareBaseData(announcementApplicationId);
            var annApp = await Connector.Announcement.GetAnnouncementApplicationById(announcementApplicationId);
            var annTask = Connector.Announcement.GetRead(annApp.AnnouncementId, annApp.AnnouncementType);
            // var announcementAppIdsTask = Connector.Announcement.StudentAnnouncementAppicationIds(3787, 179);
            var announcementApplicationRecipients = Connector.Announcement.GetAnnouncementApplicationRecipients(null);

            var ids = defaultAbIds.Select(Guid.Parse).ToList();
            var standardsTask = Connector.Standards.GetStandardsByIds(ids);
            var relationsTask = Connector.Standards.GetListOfStandardRelations(ids);

            return View("Attach", DefaultJsonViewData.Create(new
            {
                // AnnouncementApplicationIds = await announcementAppIdsTask,
                AnnouncementApplicationRecipients = await announcementApplicationRecipients,
                Announcement = await annTask,
                Standards = await standardsTask,
                Relations = await relationsTask,
            }));
        }

        public async Task<ActionResult> ContentAttach(int announcementApplicationId, string contentId)
        {
            if(string.IsNullOrWhiteSpace(contentId))
                throw new ChalkableApiException("Invalid param. ContentId is missing");
            var content = ContentStorage.GetStorage().GetContentById(contentId);
            if(content == null)
                throw new ChalkableApiException("Content not found");
            PrepareBaseData(announcementApplicationId);
            var updateAnnAppMeta = Connector.Announcement.UpdateAnnouncementApplicationMeta(announcementApplicationId, content.Text, content.ImageUrl);

            var ids = defaultAbIds.Select(Guid.Parse).ToList();
            var standardsTask = Connector.Standards.GetStandardsByIds(ids);
            var relationsTask = Connector.Standards.GetListOfStandardRelations(ids);

           


            return View("Attach", DefaultJsonViewData.Create(new
            {
                Content = content,
                Standards = await standardsTask,
                Relations = await relationsTask,
                UpdatedAnnAppMeate = await updateAnnAppMeta,
            }));
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