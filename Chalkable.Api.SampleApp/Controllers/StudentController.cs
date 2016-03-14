using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.Api.SampleApp.Models;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class StudentController : BaseSampleAppController
    {
        public async Task<ActionResult> Index()
        {
            var announcementApplicationRecipients = await Connector.Announcement.GetAnnouncementApplicationRecipients(null);
            PrepareBaseData(null);
            return View("App");
        }

        public async Task<ActionResult> ViewMode(int announcementApplicationId)
        {
            PrepareBaseData(announcementApplicationId);

            const string defaultScore = "60";

            await  Connector.Grading.SetAutoGrade(announcementApplicationId, CurrentUser.Id, defaultScore);
            
            return View("ViewMode", DefaultJsonViewData.Create(new
            {
                AnnouncementApplicationId = announcementApplicationId,
                StudentId = CurrentUser.Id,
                Score = defaultScore
            }));
         }

        public async Task<ActionResult> Practice(string standardId, string commonCoreStandard, string standardName)
        {
            const string defaultScore = "60";

            var res = await Connector.StudyCenter.SetPracticeGrade(Guid.Parse(standardId), defaultScore);
            
            return View("ViewMode", DefaultJsonViewData.Create(new
            {
                Score = defaultScore,
                StandardId = standardId,
                CommonCoreStandard = commonCoreStandard,
                StandardName = standardName,
                Success = res
            }));
        }
    }
}