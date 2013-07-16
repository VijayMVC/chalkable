using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StorageMonitorController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult ListBlobContainers(int? start, int? count)
        {
            var res = MasterLocator.StorageBlobService.GetBlobContainers(start ?? 0, count ?? 10);
            return Json(res.Transform(ContainerViewData.Create));
        }
        [AuthorizationFilter("SysAdmin")]
        public ActionResult ListBlobs(string containeraddress, int? start, int? count)
        {
            var res = MasterLocator.StorageBlobService.GetBlobs(containeraddress, null, start ?? 0, count ?? 10);
            return Json(res.Transform(BlobViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Delete(string blobAddress)
        {
            MasterLocator.StorageBlobService.DeleteBlob(blobAddress);
            return Json(true);
        }
    }
}
