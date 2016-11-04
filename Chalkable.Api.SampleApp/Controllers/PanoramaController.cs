using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.Api.SampleApp.Models;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class PanoramaController : BaseSampleAppController
    {
        public async Task<ActionResult> ClassPanorama(int classId)
        {
            var classPanorama = await Connector.Panorama.GetClassPanorama(classId);
            PrepareBaseData(null);
            return View("App", DefaultJsonViewData.Create(classPanorama));
        }

        public ActionResult SchoolPanorama(int schoolId)
        {
            return View("NotSupported");
        }
    }
}