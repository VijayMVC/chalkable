using System;
using System.Collections.Generic;
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