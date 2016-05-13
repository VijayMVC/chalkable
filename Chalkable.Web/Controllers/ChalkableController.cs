﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Authentication;
using Microsoft.IdentityModel.Claims;

namespace Chalkable.Web.Controllers
{
    public class ChalkableController : Controller
    {

        protected const int DEFAULT_PAGE_SIZE = 10;
        private const string ACTOR_SUFFIX = "actor";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            filterContext.RequestContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            filterContext.RequestContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, X-Requested-With, Accept, Access-Control-Allow-Origin, Content-Type");

            if (filterContext.RequestContext.HttpContext.Request.HttpMethod == "OPTIONS")
            {
                filterContext.Result = new JsonResult();
            }
            else base.OnActionExecuting(filterContext);
        }

        public ActionResult Json(object data, int serializationDepth = 10)
        {
            var response = (data is IPaginatedList) 
                    ? new ChalkableJsonPaginatedResponse((IPaginatedList)data) 
                    : new ChalkableJsonResponce(data);

            if (data is ChalkableException)
            {
                Response.StatusCode = (int) HttpStatusCode.OK;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
            }

            return new ChalkableJsonResult(HideSensitiveData())
                {
                    Data = response, 
                    SerializationDepth = serializationDepth
                };
        }

        public async Task<ActionResult> Json<TResult>(Task<TResult> task, int serializationDepth = 10)
        {
            return Json(await task, serializationDepth);
        }

        private bool HideSensitiveData()
        {
            return Context!= null && Context.IsOAuthUser && (!Context.IsInternalApp 
                || (Context.OAuthApplication == Settings.ApiExplorerClientId));
        }


        public ActionResult Json(object data, string contentType, int serializationDepth = 10)
        {
            var res = Json(data, serializationDepth);
            if (res is JsonResult)
            {
                (res as JsonResult).ContentType = contentType;
            }
            return res;
        }

        protected ActionResult FakeJson(string filename)
        {
            var fullName = HttpContext.Server.MapPath(filename);
            var jsonF = System.IO.File.ReadAllText(fullName);
            return Content(jsonF, "application/json");
        }


        protected void PrepareJsonData(object data, string name, int maxDepth = 4)
        {
            var jsonResponse = new ChalkableJsonResponce(data);
            var serializer = new MagicJsonSerializer(false) { MaxDepth = maxDepth };
            var res = serializer.Serialize(jsonResponse);
            ViewData[name] = res;
        }


        protected string GetBaseAppUrl(string authority)
        {
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;
            if (!string.IsNullOrEmpty(appUrl) && appUrl != "/") appUrl += "/";
            return $"{Request.Url.Scheme}://{authority}{appUrl}";
        }
        protected string GetBaseAppUrl()
        {
            return GetBaseAppUrl(Request.Url.Authority);
        }

        protected string GetBaseAppUrlReferrer()
        {
            return GetBaseAppUrl(Request.UrlReferrer.Authority);
        }
        
        public IServiceLocatorMaster MasterLocator { get; protected set; }
        public IServiceLocatorSchool SchoolLocator { get; protected set; }
        protected UserContext Context => SchoolLocator != null ? SchoolLocator.Context : MasterLocator.Context;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var chalkablePrincipal = User as ChalkablePrincipal;
            UserContext context = null;

            bool isAuthenticatedByToken = OauthAuthenticate.Instance.TryAuthenticateByToken(requestContext);
            if (isAuthenticatedByToken)
            {
                var userData = OAuthUserIdentityInfo.CreateFromString(User.Identity.Name);
                
                var user = ChalkableAuthentication.GetUser(userData.SessionKey);
                InitServiceLocators(user.Context);

                SchoolLocator.Context.IsOAuthUser = true;
                
                var claims = (User.Identity as ClaimsIdentity).Claims;
                var actor = claims.First(x => x.ClaimType.EndsWith(ACTOR_SUFFIX)).Value.Split(',').FirstOrDefault();
                var app = MasterLocator.ApplicationService.GetApplicationByUrl(actor);
                SchoolLocator.Context.IsInternalApp = app != null && app.IsInternal;
                SchoolLocator.Context.OAuthApplication = actor;
                SchoolLocator.Context.AppPermissions = MasterLocator.ApplicationService.GetPermisions(actor);

                return;
            }

            if (chalkablePrincipal != null && chalkablePrincipal.Identity.IsAuthenticated
                && !string.IsNullOrEmpty(chalkablePrincipal.Identity.Name))
            {
                if (chalkablePrincipal.Context.SisTokenExpires.HasValue && chalkablePrincipal.Context.SisTokenExpires < DateTime.Now)
                {
                    ChalkableAuthentication.SignOut();
                    return;
                }
                context = chalkablePrincipal.Context;
            }
            InitServiceLocators(context);
        }
        
        protected void InitServiceLocators(UserContext context)
        {
            SchoolLocator = ServiceLocatorFactory.CreateSchoolLocator(context);
            MasterLocator = SchoolLocator.ServiceLocatorMaster;
        }
        
        public RedirectToRouteResult Redirect<T>(Expression<Action<T>> action) where T : Controller
        {
            var routeValues = HtmlExtensions.ParseExpression(action);
            return RedirectToRoute(routeValues);
        }


        protected bool GetFileFromRequest(out byte[] bin, out string name, bool onlyImages = false)
        {
            if (Request.Files.Count == 1 && !string.IsNullOrEmpty(Request.Files[0].FileName))
            {
                HttpPostedFileBase hpf = Request.Files[0];
                bin = new byte[hpf.InputStream.Length];
                hpf.InputStream.Read(bin, 0, (int)hpf.InputStream.Length);
                name = hpf.FileName;
                if (onlyImages)
                {
                    var validImg = ImageUtils.IsValidImage(hpf.InputStream);
                    if (!validImg)
                    {
                        bin = null;
                        name = null;
                        return false;
                    }
                }
                return true;
            }
            bin = null;
            name = null;
            return false;
        }

        protected const string HTML_CONTENT_TYPE = "text/html";
        protected ActionResult UploadPicture(IPictureService pictureService, Guid id, int? width, int? height)
        {
            byte[] bin;
            string name;
            if (!GetFileFromRequest(out bin, out name, true))
                return Json(new ChalkableException(ChlkResources.ERR_FILE_IS_REQUIRED));

            if (width.HasValue && height.HasValue)
            {
                pictureService.UploadPicture(id, bin, height, width);
            }
            else pictureService.UploadPicture(id, bin);
            
            return Json(id, HTML_CONTENT_TYPE);
        }

        protected int GetCurrentSchoolYearId()
        {
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            return Context.SchoolYearId.Value;
        }

        public string ApplicationPath
        {
            get
            {
                var res = Request.ApplicationPath;
                if (!string.IsNullOrWhiteSpace(res))
                {
                    res = res.ToLower();
                    if (res == "/") //a site
                        res = "/";
                    else if (!res.EndsWith(@"/")) //a virtual
                        res += @"/";
                }
                return res;

            }
        }

        protected int NowTimeInMinutes
        {
            get
            {
                var now = MasterLocator.Context.NowSchoolTime;
                return (int)(now - now.Date).TotalMinutes;
            }
        }


        protected ActionResult RedirectToHome(CoreRole role)
        {
            if (role == CoreRoles.SUPER_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.SysAdmin());
            if (role == CoreRoles.DISTRICT_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.DistrictAdmin());
            if (role == CoreRoles.TEACHER_ROLE)
                return Redirect<HomeController>(x => x.Teacher());
            if (role == CoreRoles.STUDENT_ROLE)
                return Redirect<HomeController>(x => x.Student());
            return Redirect<HomeController>(x => x.Index());
        }

        protected ActionResult HandleAttachmentException(Exception exception)
        {
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 500;
            Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
            return new ChalkableJsonResult(false)
            {
                Data = new ChalkableJsonResponce(ExceptionViewData.Create(exception))
                {
                    Success = false
                },
                ContentType = HTML_CONTENT_TYPE,
                SerializationDepth = 4
            };
        }
    }
}
