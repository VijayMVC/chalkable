using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StorageMonitorController : ChalkableController
    {
        [AuthorizationFilter("System Admin")]
        public ActionResult ListBlobContainers(int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var res = MasterLocator.StorageBlobService.GetBlobContainers(start.Value, count.Value);
            return Json(res.Transform(ContainerViewData.Create));
        }
        [AuthorizationFilter("System Admin")]
        public ActionResult ListBlobs(string containeraddress, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var res = MasterLocator.StorageBlobService.GetBlobs(containeraddress, null, start.Value, count.Value);
            return Json(res.Transform(BlobViewData.Create));
        }

        [AuthorizationFilter("System Admin")]
        public ActionResult Delete(string blobAddress)
        {
            MasterLocator.StorageBlobService.DeleteBlob(blobAddress);
            return Json(true);
        }
    }
}
