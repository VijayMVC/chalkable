using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Controllers;


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
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var jsonResponse = new { Data = filterContext.Exception, Success = false };
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