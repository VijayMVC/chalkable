using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Web;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Authentication;

namespace Chalkable.Web.Controllers
{
    public class ChalkableController : Controller
    {
        public new ActionResult Json(object data, int serializationDepth = 10)
        {
            var response = (data is IPaginatedList) ? new ChalkableJsonPaginatedResponse((IPaginatedList)data) 
                                                        : new ChalkableJsonResponce(data);
            return new ChalkableJsonResult(false) { Data = response, SerializationDepth = serializationDepth };
        }

        
        
        public IServiceLocatorMaster MasterLocator { get; protected set; }
        public IServiceLocatorSchool SchoolLocator { get; protected set; }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var chalkablePrincipal = User as ChalkablePrincipal;
            UserContext context = null;
            
            //TODO: authenticateByToken
            //bool isAuthenticatedByToken = OauthAuthenticate.Instance.TryAuthenticateByToken(requestContext);
            //if (isAuthenticatedByToken)
            //{
            //    if ()
            //    {
            //        SchoolLocator = LocatorFactory.Create(User.Identity.Name);
            //        var claims = (User.Identity as ClaimsIdentity).Claims;
            //        var actor = claims.First(x => x.ClaimType.EndsWith(ACTOR_SUFFIX)).Value;

            //        SchoolLocator.Context.IsOAuthUser = true;
            //        var app = ServiceLocator.ApplicationService.GetApplicationByUrl(actor);
            //        SchoolLocator.Context.IsInternalApp = app != null && app.IsInternal;
            //        SchoolLocator.Context.OAuthApplication = actor;
            //        SchoolLocator.Context.AppPermissions = ServiceLocator.ApplicationService.GetPermisions(actor);
            //        return;
            //    }
            //}

            if (chalkablePrincipal != null && chalkablePrincipal.Identity.IsAuthenticated
                && !string.IsNullOrEmpty(chalkablePrincipal.Identity.Name))
            {
                context = chalkablePrincipal.Context;
            }
            SchoolLocator = ServiceLocatorFactory.CreateSchoolLocator(context);
            MasterLocator = SchoolLocator.ServiceLocatorMaster;
        }

        
        
        public RedirectToRouteResult Redirect<T>(Expression<Action<T>> action) where T : Controller
        {
            var routeValues = HtmlExtensions.ParseExpression(action);
            return RedirectToRoute(routeValues);
        }
        protected bool GetFileFromRequest(out byte[] bin, out string name)
        {
            if (Request.Files.Count == 1 && !string.IsNullOrEmpty(Request.Files[0].FileName))
            {
                HttpPostedFileBase hpf = Request.Files[0];
                bin = new byte[hpf.InputStream.Length];
                hpf.InputStream.Read(bin, 0, (int)hpf.InputStream.Length);
                name = Request.Files[0].FileName;
                return true;
            }
            bin = null;
            name = null;
            return false;
        }
    }
}
