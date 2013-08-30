using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Web.Controllers;

namespace Chalkable.Web.ActionFilters
{
    public class TraceControllerFilter : ActionFilterAttribute
    {
        public TraceControllerFilter()
        {
        }

        private const string paramMessageTemplate = "params type : {0} , value : {1} = {2}\n";
        private const string traceFmt = "It was call {1} -> {0} action";
        private const string actionKey = "Action";
        private const string controllerKey = "Controller";
        private const string paramsStr = "Parameters : ";
        private const string unknownType = "Unknown Type";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!(filterContext.Controller is ChalkableController))
                throw new NotSupportedException(ChlkResources.ERR_TRACE_UNSUPPORTED_CONTROLLER);
            string message = string.Format(traceFmt, filterContext.RouteData.Values[actionKey], filterContext.RouteData.Values[controllerKey]);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(message);
            builder.AppendLine(paramsStr);
            var reqParams = filterContext.HttpContext.Request.Params;
            ParameterBuilder(builder, filterContext.ActionParameters, reqParams);
           // Trace.TraceInformation(builder.ToString());
            base.OnActionExecuting(filterContext);
        }

        private void ParameterBuilder(StringBuilder builder, IDictionary<string, object> metParams, NameValueCollection reqParams)
        {
            foreach (var param in metParams)
            {
                var type = param.Value != null ? param.Value.GetType() : null;
                if (type != null && (ModelBinders.Binders.ContainsKey(type) || type.IsArray))
                {
                    builder.AppendFormat(paramMessageTemplate, type.Name, param.Key,
                                         reqParams.AllKeys.Contains(param.Key) ? reqParams[param.Key] : null);
                    continue;
                }
                builder.AppendFormat(paramMessageTemplate, type == null ? unknownType : type.Name, param.Key, param.Value);
            }
        }

    }
}