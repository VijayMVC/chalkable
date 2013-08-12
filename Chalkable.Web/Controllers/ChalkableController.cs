﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Authentication;

namespace Chalkable.Web.Controllers
{
    public class ChalkableController : Controller
    {

        protected const int DEFAULT_PAGE_SIZE = 10;

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


        protected void PrepareJsonData(object data, string name, int maxDepth = 4)
        {
            var jsonResponse = new ChalkableJsonResponce(data);
            var serializer = new MagicJsonSerializer(false) { MaxDepth = maxDepth };
            var res = serializer.Serialize(jsonResponse);
            ViewData[name] = res;
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
            InitServiceLocators(context);
        }

        private void InitServiceLocators(UserContext context)
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
            
            return Json(true, htmlContentType);
        }

        protected Guid GetCurrentSchoolYearId()
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
        protected Guid EnsureMarkingPeriodId(Guid? markingPeriodId)
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


        protected ActionResult Confirm(string key, Func<UserContext, ActionResult> redirectAction)
        {
            var context = LogOn(false, us => us.Login(key));
            if (context != null)
            {
                InitServiceLocators(context);
                return redirectAction(context);
            }
            return Redirect<HomeController>(c => c.Index());
        }

        protected UserContext LogOn(bool remember, Func<IUserService, UserContext> logOnAction)
        {
            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var context = logOnAction(serviceLocator.UserService);
            if (context != null)
                ChalkableAuthentication.SignIn(context, remember);
            return context;
        }
    }
}
