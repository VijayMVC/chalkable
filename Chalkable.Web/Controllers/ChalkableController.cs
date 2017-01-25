using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.AcademicBenchmark;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Authentication;

namespace Chalkable.Web.Controllers
{
    public class ChalkableController : Controller
    {

        protected const int DEFAULT_PAGE_SIZE = 10;

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
            return Context!= null && Context.IsOAuthUser && (!Context.IsInternalApp && !Context.IsTrustedApp
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
        public IAcademicBenchmarkServiceLocator AcademicBenchmarkLocator { get; protected set; }
        protected UserContext Context => SchoolLocator?.Context ?? MasterLocator?.Context;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var chalkablePrincipal = User as ChalkablePrincipal;
            UserContext context = null;
            
            AuthorizationUserInfo authAppInfo;
            Application app;
            if (ApplicationAuthentification.AuthenticateByToken(requestContext, ServiceLocatorFactory.CreateMasterSysAdmin().ApplicationService,
                out authAppInfo, out app))
            {
                var user = ChalkableAuthentication.GetUser(authAppInfo.SessionKey);
                InitServiceLocators(user.Context);

                SchoolLocator.Context.IsOAuthUser = true;
                SchoolLocator.Context.IsInternalApp = app.IsInternal;
                SchoolLocator.Context.IsTrustedApp = app.IsTrustedApp;
                SchoolLocator.Context.OAuthApplication = app.Url;
                SchoolLocator.Context.AppPermissions = MasterLocator.ApplicationService.GetPermisions(app.Url);

                //if(SchoolLocator.Context.LoginTimeOut.HasValue)
                //    GlobalCache.UpdateExpiryUserInfo(authAppInfo.SessionKey, new TimeSpan(0, 0, SchoolLocator.Context.LoginTimeOut.Value));

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
            AcademicBenchmarkLocator = new AcademicBenchmarkServiceLocator(context);
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
                return Redirect<HomeController>(x => x.SysAdmin(false));
            if (role == CoreRoles.DISTRICT_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.DistrictAdmin());
            if (role == CoreRoles.TEACHER_ROLE)
                return Redirect<HomeController>(x => x.Teacher());
            if (role == CoreRoles.STUDENT_ROLE)
                return Redirect<HomeController>(x => x.Student());
            if (role == CoreRoles.APP_TESTER_ROLE)
                return Redirect<HomeController>(x => x.AppTester(false));
            if (role == CoreRoles.ASSESSMENT_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.AssessmentAdmin(false));
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

        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    base.OnActionExecuted(filterContext);
        //    if(!filterContext.RouteData.Values.ContainsKey("IgnoreTimeOut"))
        //        ChalkableAuthentication.UpdateLoginTimeOut(Context);
        //}
    }

    public class IgnoreTimeOut : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    filterContext.RouteData.Values.Add("IgnoreTimeOut", "1");
        //}
    }
}
