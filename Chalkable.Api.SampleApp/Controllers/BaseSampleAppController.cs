using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.Api.SampleApp.Models;
using Chalkable.API;
using Chalkable.API.Controllers;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class BaseSampleAppController : BaseController
    {
        protected ChalkableConnector Connector { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            base.Initialize(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var chalkableAuthorization = ChalkableAuthorization;
            if (chalkableAuthorization != null)
            {
                Connector = new ChalkableConnector(chalkableAuthorization);
                ViewBag.ApiRoot = chalkableAuthorization.ApiRoot;
            }
            base.OnActionExecuting(filterContext);
        }

        protected void PrepareBaseData(int? announcementApplicationId)
        {
            ViewBag.AnnouncementApplicationId = announcementApplicationId;
            ViewBag.CurrentUser = DefaultJsonViewData.Create(CurrentUser); 
        }

        protected ActionResult ChlkJson(object data)
        {
            var res = new JsonResult
            {
                Data = new
                {
                    Success = true,
                    Data = data
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return res;
        }
    }
}