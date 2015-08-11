﻿using System.Diagnostics;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
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
            raygunClient.ApplicationVersion = Settings.Domain + CompilerHelper.Version;
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

            if (filterContext.Exception == null)
            {
                // DO nothing, no exception
            }
            else if (filterContext.HttpContext.Request.IsAjaxRequest() || filterContext.HttpContext.Request.IsApiRequest())
            {
                ProcessJsonRequestError(filterContext);
            }
            else
            {
                ProcessNormalRequestError(filterContext);
            }

            base.OnException(filterContext);
        }

        private static void ProcessNormalRequestError(ExceptionContext filterContext)
        {
            if (filterContext.Exception is HttpException &&
                ((HttpException) filterContext.Exception).GetHttpCode() == 401)
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result =
                    new RedirectToRouteResult(HtmlExtensions.ParseExpression<HomeController>(c => c.Index()));

                ChalkableAuthentication.SignOut();
            }
        }

        private static void ProcessJsonRequestError(ExceptionContext filterContext)
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
            {
                filterContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.OK;
            }

            filterContext.HttpContext.Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(filterContext.HttpContext.Response.StatusCode);

            var jsonResponse = new ChalkableJsonResponce(ExceptionViewData.Create(filterContext.Exception, filterContext.Exception.InnerException))
            {
                Success = false
            };

            var jsonresult = new ChalkableJsonResult(false)
            {
                Data = jsonResponse,
                SerializationDepth = 4
            };
            filterContext.Result = jsonresult;
        }
    }
}