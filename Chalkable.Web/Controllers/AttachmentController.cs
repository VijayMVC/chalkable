using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AttachmentController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult AttachmentsList(string filter, int? sortType, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var attachments = SchoolLocator.AttachementService.GetAttachmentsInfo(filter, (AttachmentSortTypeEnum?) sortType, start.Value, count.Value);
            return Json(attachments.Transform(x=> AttachmentViewData.Create(x, Context.PersonId.Value)));
        }
 
    }
}