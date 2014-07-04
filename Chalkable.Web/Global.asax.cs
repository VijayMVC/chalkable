using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Chalkable.Common;
using Chalkable.Web.Authentication;
using Chalkable.Web.Models;
using Chalkable.Web.Models.Binders;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Chalkable.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
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


            if (ConfigurationManager.AppSettings["WindowsAzure.OAuth.RelyingPartyRealm"] != null && ConfigurationManager.AppSettings["WindowsAzure.OAuth.ServiceNamespace"] != null && ConfigurationManager.AppSettings["WindowsAzure.OAuth.SwtSigningKey"] != null)
            {
                OauthAuthenticate.InitFromConfig();
            }
            else
            {
                throw new ArgumentException("To enable OAuth2 support for your web project, configure WindowsAzure.OAuth.RelyingPartyRealm, WindowsAzure.OAuth.ServiceNamespace and WindowsAzure.OAuth.SwtSigningKey in your applications's appSettings.");
            }
            ConfigureDiagnostics();
        }


        private void ConfigureDiagnostics()
        {
            String wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));

            RoleInstanceDiagnosticManager roleInstanceDiagnosticManager =
                cloudStorageAccount.CreateRoleInstanceDiagnosticManager(RoleEnvironment.DeploymentId, RoleEnvironment.CurrentRoleInstance.Role.Name, RoleEnvironment.CurrentRoleInstance.Id);
            DiagnosticMonitorConfiguration diagnosticMonitorConfiguration = roleInstanceDiagnosticManager.GetCurrentConfiguration();
            diagnosticMonitorConfiguration.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(5d);
            diagnosticMonitorConfiguration.Logs.ScheduledTransferPeriod = TimeSpan.FromSeconds(30);
            diagnosticMonitorConfiguration.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("Application!*");
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("System!*");
            diagnosticMonitorConfiguration.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(5d);
            PerformanceCounterConfiguration performanceCounterConfiguration = new PerformanceCounterConfiguration();
            performanceCounterConfiguration.CounterSpecifier = @"\Processor(_Total)\% Processor Time";
            performanceCounterConfiguration.SampleRate = TimeSpan.FromSeconds(10d);
            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(performanceCounterConfiguration);
            diagnosticMonitorConfiguration.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1d);
            roleInstanceDiagnosticManager.SetCurrentConfiguration(diagnosticMonitorConfiguration);
        }


        protected void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            var chalkableUser = ChalkableAuthentication.GetUser();
            if (chalkableUser != null)
            {
                HttpContext.Current.User = chalkableUser;
            }
        }
    }

    
}