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

        private static string idsstr = "0aa2d9cc-c604-11e5-84c5-0a6a392f403d,a0cb6492-c608-11e5-84c5-0a6a392f403d,73ff688a-c524-11e5-84c5-0a6a392f403d,38a31ee2-c544-11e5-84c5-0a6a392f403d,d4684734-c54f-11e5-84c5-0a6a392f403d,5d781008-c682-11e5-84c5-0a6a392f403d,653e946a-c682-11e5-84c5-0a6a392f403d,6398d76a-c682-11e5-84c5-0a6a392f403d,deaff7ba-c5c1-11e5-84c5-0a6a392f403d,1c93f1da-c5c7-11e5-84c5-0a6a392f403d,a572d6d8-c5d1-11e5-84c5-0a6a392f403d,adb4bd16-c5d6-11e5-84c5-0a6a392f403d,b1f95aac-c5e6-11e5-84c5-0a6a392f403d,e6c0bc87-9b4b-42a8-a74d-21e3bad0018f,924c9668-acd6-4df2-8a72-4d0192202423,27ec3f00-74f7-11df-80dd-6b359dff4b22,2808500a-74f7-11df-80dd-6b359dff4b22,27fb400e-74f7-11df-80dd-6b359dff4b22,27dca04a-74f7-11df-80dd-6b359dff4b22,2c9d2460-74f7-11df-80dd-6b359dff4b22,2cd77e62-74f7-11df-80dd-6b359dff4b22,2c8d6372-74f7-11df-80dd-6b359dff4b22,2cedfcaa-74f7-11df-80dd-6b359dff4b22,27d3f0bc-74f7-11df-80dd-6b359dff4b22,281222c4-74f7-11df-80dd-6b359dff4b22,281dabf8-74f7-11df-80dd-6b359dff4b22,db213a0e-de81-11e0-831a-85489dff4b22,da285916-de81-11e0-831a-85489dff4b22,da54433c-de81-11e0-831a-85489dff4b22,dc3d2042-de81-11e0-831a-85489dff4b22,db505942-de81-11e0-831a-85489dff4b22,dc0eed58-de81-11e0-831a-85489dff4b22,dc6c8062-de81-11e0-831a-85489dff4b22,da82db66-de81-11e0-831a-85489dff4b22,dbdda072-de81-11e0-831a-85489dff4b22,daeb63ac-de81-11e0-831a-85489dff4b22,dab7d2bc-de81-11e0-831a-85489dff4b22,dbace4be-de81-11e0-831a-85489dff4b22,db7cafc4-de81-11e0-831a-85489dff4b22,d9fab2e0-de81-11e0-831a-85489dff4b22,00417c08-d9e0-11e2-b767-92bed51f4efc,5c38f626-d9e0-11e2-b767-92bed51f4efc,3e3cfd52-d9e0-11e2-b767-92bed51f4efc,76db4056-d9e0-11e2-b767-92bed51f4efc,2d7fb25c-d9e0-11e2-b767-92bed51f4efc,1e0e206a-d9e0-11e2-b767-92bed51f4efc,0dc7f37a-d9e0-11e2-b767-92bed51f4efc,4de5257c-d9e0-11e2-b767-92bed51f4efc,8a40c288-d9e0-11e2-b767-92bed51f4efc,6a2652d8-d9e0-11e2-b767-92bed51f4efc,9230d2da-d9e0-11e2-b767-92bed51f4efc,816724e0-d9e0-11e2-b767-92bed51f4efc,ced36fde-67ad-11df-ab5f-995d9dff4b22,3d31e0dd-b11e-4ecf-b72b-9f4cbcd492c1,a2f247b7-be0e-42de-9132-b1cd055a9637,062bcca1-20b8-451b-943d-bd0133f25727,088b96a5-cc57-47f3-bce0-e68a2fb2f8b2,c0e71e0c-d9ec-11e2-a4cc-ef239dff4b22,de069120-d9ec-11e2-a4cc-ef239dff4b22,a3e2f146-d9ec-11e2-a4cc-ef239dff4b22,d04d2562-d9ec-11e2-a4cc-ef239dff4b22,aa7c1d7a-d9ec-11e2-a4cc-ef239dff4b22,b1ca647e-d9ec-11e2-a4cc-ef239dff4b22,ee587e80-d9ec-11e2-a4cc-ef239dff4b22,d73a82c0-d9ec-11e2-a4cc-ef239dff4b22,b9662dc6-d9ec-11e2-a4cc-ef239dff4b22,e4c472d4-d9ec-11e2-a4cc-ef239dff4b22,c8cb8bda-d9ec-11e2-a4cc-ef239dff4b22,fb396dee-d9ec-11e2-a4cc-ef239dff4b22,43b5057e-d249-4c0e-b9ed-f519d18e8f47,e0daa00c-d9bb-11e2-bacf-f789d51f4efc,d9a5633a-d9bb-11e2-bacf-f789d51f4efc,cbb52544-d9bb-11e2-bacf-f789d51f4efc,ef8a4c7e-d9bb-11e2-bacf-f789d51f4efc,f69d2b8a-d9bb-11e2-bacf-f789d51f4efc,fdc96590-d9bb-11e2-bacf-f789d51f4efc,d2bf639a-d9bb-11e2-bacf-f789d51f4efc,c4ac31a2-d9bb-11e2-bacf-f789d51f4efc,e86c40aa-d9bb-11e2-bacf-f789d51f4efc,be1b26b8-d9bb-11e2-bacf-f789d51f4efc,0d014ab4-d9bc-11e2-bacf-f789d51f4efc,05b18bca-d9bc-11e2-bacf-f789d51f4efc,fc560846-d9b9-11e2-9295-fe079dff4b22,ecc65246-d9b9-11e2-9295-fe079dff4b22,f4343e9e-d9b9-11e2-9295-fe079dff4b22,e4fa7bd2-d9b9-11e2-9295-fe079dff4b22,d72caaf2-d9b9-11e2-9295-fe079dff4b22,dd9f89fe-d9b9-11e2-9295-fe079dff4b22,3975313e-d9ba-11e2-9295-fe079dff4b22,19fb3948-d9ba-11e2-9295-fe079dff4b22,0b531d98-d9ba-11e2-9295-fe079dff4b22,122db2ae-d9ba-11e2-9295-fe079dff4b22,2a13ffd6-d9ba-11e2-9295-fe079dff4b22,03c24bee-d9ba-11e2-9295-fe079dff4b22,10badac8-b508-11e1-86c6-01289dff4b22,ad738b1a-d9bc-11e2-abff-3e079dff4b22,ad811366-d9bc-11e2-abff-3e079dff4b22,ada7b7be-d9bc-11e2-abff-3e079dff4b22,18ffb32a-9d49-11e0-8793-50509dff4b22,18fc6f62-9d49-11e0-8793-50509dff4b22,18fcec8a-9d49-11e0-8793-50509dff4b22,d13f091e-a544-4afa-9776-7d9b08a026f0,48b7097a-d9e1-11e2-850a-9cbed51f4efc,48a3cdc4-d9e1-11e2-850a-9cbed51f4efc,48f366ea-d9e1-11e2-850a-9cbed51f4efc,19efd767-6c32-419d-98c9-a9083b2c3775,fc0ef5e0-7446-4a38-a73e-b16dd09d83a6,1f2e953c-7053-11df-8ebf-be719dff4b22,1f23c68e-7053-11df-8ebf-be719dff4b22,1f2530c8-7053-11df-8ebf-be719dff4b22,ac601a86-04eb-4708-974f-c3076a094000,a7d83616-d9ed-11e2-b074-e9239dff4b22,a7ff8950-d9ed-11e2-b074-e9239dff4b22,a7cb0fea-d9ed-11e2-b074-e9239dff4b22,e5d3de1c-d9ba-11e2-aea7-ef109dff4b22,e5c2197a-d9ba-11e2-aea7-ef109dff4b22,e6073dfc-d9ba-11e2-aea7-ef109dff4b22,b8638691-d384-4d27-baf3-febee2173770";
        
        private static IList<string> defaultAbIds = idsstr.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);

        public async Task<ActionResult> SimpleAttach(int announcementApplicationId)
        {
            PrepareBaseData(announcementApplicationId);
            var annApp = await Connector.Announcement.GetAnnouncementApplicationById(announcementApplicationId);
            var annTask = Connector.Announcement.GetRead(annApp.AnnouncementId, annApp.AnnouncementType);
            // var announcementAppIdsTask = Connector.Announcement.StudentAnnouncementAppicationIds(3787, 179);
            var announcementApplicationRecipients = Connector.Announcement.GetAnnouncementApplicationRecipients(null);
            var topicId = Guid.Parse("585FCDB8-B949-11E0-9185-BB84D8FDF33C");
            var topicsTask = Connector.Standards.GetTopicsByIds(new List<Guid> { topicId });

            var ids = defaultAbIds.Select(Guid.Parse).ToList();
            var standardsTask = Connector.Standards.GetStandardsByIds(ids);
            var relationsTask = Connector.Standards.GetListOfStandardRelations(ids);

            var isSchoolDay = await Connector.Calendar.IsSchoolDay(null);

            var studentAttendance = await Connector.Attendance.GetStudentAttendance(3688, new DateTime(2015,12,22,12,0,0));
            studentAttendance = await Connector.Attendance.GetStudentAttendance(3688, new DateTime(2015, 12, 23, 12, 0, 0));
            studentAttendance = await Connector.Attendance.GetStudentAttendance(3688, new DateTime(2015, 12, 24, 12, 0, 0));
            studentAttendance = await Connector.Attendance.GetStudentAttendance(3688, null);

            return View("Attach", DefaultJsonViewData.Create(new
            {
                // AnnouncementApplicationIds = await announcementAppIdsTask,
                Topics = await topicsTask,
                AnnouncementApplicationRecipients = await announcementApplicationRecipients,
                Announcement = await annTask,
                Standards = await standardsTask,
                Relations = await relationsTask,
                IsSchoolDay = isSchoolDay,
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
            var updateAnnAppMeta = Connector.Announcement.UpdateAnnouncementApplicationMeta(announcementApplicationId, content.Text, content.ImageUrl, content.Description);

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