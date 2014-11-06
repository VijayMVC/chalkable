using System.Diagnostics;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Authentication;
using Chalkable.Web.Controllers;
using Chalkable.Web.Tools;
using Mindscape.Raygun4Net;


namespace Chalkable.Web.ActionFilters
{
    public class AjaxHandleErrorAttribute : HandleErrorAttribute
    {
        private static RaygunClient raygunClient = new RaygunClient();
        static AjaxHandleErrorAttribute()
        {
            raygunClient.ApplicationVersion = CompilerHelper.Version;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            Trace.TraceError(ChlkResources.ERR_MESSAGE_WITH_STACKTRACE, filterContext.Exception.Message, filterContext.Exception.StackTrace);
#if !DEBUG
            if (filterContext.HttpContext != null && filterContext.HttpContext.User != null)
                raygunClient.User = filterContext.HttpContext.User.Identity.Name;
            raygunClient.SendInBackground(filterContext.Exception);
#endif
            if (filterContext.Exception.InnerException != null)
                Trace.TraceError(ChlkResources.ERR_INNER_MESSAGE_WITH_STACKTRACE, filterContext.Exception.InnerException.Message, filterContext.Exception.InnerException.StackTrace);
             
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                if (filterContext.Exception is HttpException)
                {
                    filterContext.HttpContext.Response.StatusCode =
                        (filterContext.Exception as HttpException).GetHttpCode();
                    filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                }
                else
                    filterContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                var jsonResponse = ExceptionViewData.Create(filterContext.Exception, filterContext.Exception.InnerException);
                var jsonresult = new ChalkableJsonResult(false)
                {
                    Data = jsonResponse,
                    SerializationDepth = 4
                };
                filterContext.Result = jsonresult;
            }

            if (!filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception is HttpException && ((HttpException)filterContext.Exception).GetHttpCode() == 401)
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result = new RedirectToRouteResult(HtmlExtensions.ParseExpression<HomeController>(c => c.Index() ));

                ChalkableAuthentication.SignOut();
            }

            base.OnException(filterContext);
        }
    }
}