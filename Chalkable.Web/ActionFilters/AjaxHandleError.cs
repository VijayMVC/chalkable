using System.Diagnostics;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionResults;


namespace Chalkable.Web.ActionFilters
{
    public class AjaxHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Trace.TraceError(ChlkResources.ERR_MESSAGE_WITH_STACKTRACE, filterContext.Exception.Message, filterContext.Exception.StackTrace);
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
            base.OnException(filterContext);
        }
    }
}