using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.Api.SampleApp.Models;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class StudentController : BaseSampleAppController
    {
        public ActionResult Index()
        {
            PrepareBaseData(null);
            return View("App");
        }

        public async Task<ActionResult> ViewMode(int announcementApplicationId)
        {
            PrepareBaseData(announcementApplicationId);

            const string defaultScore = "60";

            await  Connector.GradingEndpoint.SetAutoGrade(announcementApplicationId, CurrentUser.Id, defaultScore);
            
            return View("ViewMode", DefaultJsonViewData.Create(new
            {
                AnnouncementApplicationId = announcementApplicationId,
                StudentId = CurrentUser.Id,
                Score = defaultScore
            }));

        }

        public ActionResult Practice(string standardId, string commonCoreStandard, string standardName)
        {
            throw new NotImplementedException();
        }
    }
}