using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Authentication;
using Chalkable.Web.Logic.ApiExplorer;
using Chalkable.Web.Models;
using Chalkable.Web.Models.Binders;
using Chalkable.Web.Tools;
using Microsoft.Web.Mvc.Controls;


namespace Chalkable.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(StringList), new StringConstructedObjectBinder<StringList>());
            ModelBinders.Binders.Add(typeof(IntList), new StringConstructedObjectBinder<IntList>());
            ModelBinders.Binders.Add(typeof(GuidList), new StringConstructedObjectBinder<GuidList>());
            ModelBinders.Binders.Add(typeof(ListOfStringList), new StringConstructedObjectBinder<ListOfStringList>());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeBinder());


            if (Settings.WindowsAzureOAuthRelyingPartyRealm != null && Settings.WindowsAzureOAuthServiceNamespace != null && Settings.WindowsAzureOAuthSwtSigningKey != null)
            {
                OauthAuthenticate.InitFromConfig();
            }
            else
            {
                throw new ArgumentException("To enable OAuth2 support for your web project, configure WindowsAzure.OAuth.RelyingPartyRealm, WindowsAzure.OAuth.ServiceNamespace and WindowsAzure.OAuth.SwtSigningKey in your applications's appSettings.");
            }
            ConfigureDiagnostics();
            PrepareBaseServiceData();
        }


        private void PrepareBaseServiceData()
        {
            var masterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            masterLocator.CommonCoreStandardService.BuildAbToCCMapper();

            var keys = ChalkableApiExplorerLogic.GenerateControllerDescriptionKeys();
            masterLocator.PreferenceService.BuildDefaultControllerDescriptions(keys.Distinct().ToList());
        }

        private void ConfigureDiagnostics()
        {
        }


        protected void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            var chalkableUser = ChalkableAuthentication.GetUser();
            if (chalkableUser != null)
            {
                HttpContext.Current.User = chalkableUser;
            }
        }

        void Application_BeginRequest(object source, EventArgs e)
        {
            EnsureCorrectDomain(Request, Response);
        }


        private static void EnsureCorrectDomain(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            if (string.IsNullOrEmpty(httpRequest.HttpMethod) || httpRequest.HttpMethod.ToLowerInvariant() != "get")
                return;

            var ensureDomain = Settings.Domain;
            if (String.IsNullOrWhiteSpace(ensureDomain))
                return;

            var currentDomain = httpRequest.ServerVariables["SERVER_NAME"];
            if (currentDomain.Equals(ensureDomain) || httpRequest.IsApiRequest() || httpRequest.IsAjaxRequest())
                return;

            if (httpRequest.Url.PathAndQuery == "/")
            {
                var homeRedirect = Settings.HomeRedirectUrl;
                if (!string.IsNullOrEmpty(homeRedirect))
                {
                    httpResponse.Redirect(homeRedirect);
                    return;
                }
            }

            httpResponse.Redirect(
                String.Format("http{0}://{1}{2}{3}",
                              httpRequest.IsSecureConnection ? "s" : "",
                              ensureDomain,
                              "", // leave default port http or https
                              httpRequest.Url.PathAndQuery
                    ));
        }
    }

    
}