using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.Api.SampleApp.Common;
using Chalkable.API;
using Chalkable.API.Controllers;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

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
        }

        protected void PrepareBaseData(int? announcementApplicationId)
        {
            ViewBag.AnnouncementApplicationId = announcementApplicationId;
            ViewBag.CurrentUser = CurrentUser; 
        }
    }
}