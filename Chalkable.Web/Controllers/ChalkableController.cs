using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Authentication;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;

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
            else
                base.OnActionExecuting(filterContext);
        }

        public new ActionResult Json(object data, int serializationDepth = 10)
        {
            var response = (data is IPaginatedList) ? new ChalkableJsonPaginatedResponse((IPaginatedList)data) 
                                                        : new ChalkableJsonResponce(data);
            return new ChalkableJsonResult(false) { Data = response, SerializationDepth = serializationDepth };
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

        public ActionResult FakeJson(string filename)
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
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, authority, appUrl);
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
        protected UserContext Context
        {
            get
            {
                if (SchoolLocator != null)
                    return SchoolLocator.Context;
                return MasterLocator.Context;
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var chalkablePrincipal = User as ChalkablePrincipal;
            UserContext context = null;

            bool isAuthenticatedByToken = OauthAuthenticate.Instance.TryAuthenticateByToken(requestContext);
            if (isAuthenticatedByToken)
            {
                var sl = User.Identity.Name.Split('\n');
                var userName = sl[0];
                int? schoolYearId = null;
                if (sl.Length > 1)
                    schoolYearId = int.Parse(sl[1]);
                var user = ServiceLocatorFactory.CreateMasterSysAdmin().UserService.GetByLogin(userName);
                InitServiceLocators(user);
                var claims = (User.Identity as ClaimsIdentity).Claims;
                var actor = claims.First(x => x.ClaimType.EndsWith(ACTOR_SUFFIX)).Value;
                SchoolLocator.Context.SchoolYearId = schoolYearId;
                SchoolLocator.Context.IsOAuthUser = true;
                SchoolLocator.Context.SisToken = user.SisToken;
                SchoolLocator.Context.SisTokenExpires = user.SisTokenExpires;
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

        private void InitServiceLocators(User user)
        {
            if (user.SchoolUsers == null || user.SchoolUsers.Count == 0)
                throw new ChalkableException(ChlkResources.ERR_USER_IS_NOT_ASSIGNED_TO_SCHOOL);
            SchoolLocator = ServiceLocatorFactory.CreateSchoolLocator(user.SchoolUsers.First());
            MasterLocator = SchoolLocator.ServiceLocatorMaster;       
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

        private const string htmlContentType = "text/html";
        protected ActionResult UploadPicture(IPictureService pictureService, Guid id, int? width, int? height)
        {
            byte[] bin;
            string name;
            if (!GetFileFromRequest(out bin, out name))
                return Json(new ChalkableException(ChlkResources.ERR_FILE_IS_REQUIRED));

            if (width.HasValue && height.HasValue)
            {
                pictureService.UploadPicture(id, bin, height, width);
            }
            else pictureService.UploadPicture(id, bin);
            
            return Json(id, htmlContentType);
        }

        protected int GetCurrentSchoolYearId()
        {
            var currentYear = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            if (currentYear != null)
                return currentYear.Id;
            throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
        }

        protected int NowTimeInMinutes
        {
            get
            {
                var now = MasterLocator.Context.NowSchoolTime;
                return (int)(now - now.Date).TotalMinutes;
            }
        }
        protected int EnsureMarkingPeriodId(int? markingPeriodId)
        {
            if (!Context.SchoolId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_NO_SCHOOL_INFO_ID);
            if (!markingPeriodId.HasValue)
            {
                var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod()
                         ?? SchoolLocator.MarkingPeriodService.GetMarkingPeriods(GetCurrentSchoolYearId()).FirstOrDefault();
                if (mp == null) throw new NoMarkingPeriodException();
                return mp.Id;
            }
            return markingPeriodId.Value;
        }


        protected ActionResult RedirectToHome(CoreRole role)
        {
            if (role == CoreRoles.SUPER_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.SysAdmin());
            if(role == CoreRoles.ADMIN_GRADE_ROLE 
                || role == CoreRoles.ADMIN_EDIT_ROLE 
                || role == CoreRoles.ADMIN_VIEW_ROLE)
                return Redirect<HomeController>(x => x.Admin(null));
            if (role == CoreRoles.TEACHER_ROLE)
                return Redirect<HomeController>(x => x.Teacher(false));
            if (role == CoreRoles.STUDENT_ROLE)
                return Redirect<HomeController>(x => x.Student(false));
            return Redirect<HomeController>(x => x.Index());
        }
    }
}
