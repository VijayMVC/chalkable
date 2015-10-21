using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Web.Controllers;

namespace Chalkable.Web.ActionFilters
{
    public class TraceControllerFilter : ActionFilterAttribute
    {

        private const string PARAM_MESSAGE_TPL = "params type : {0} , value : {1} = {2}\n";
        private const string TRACE_TPL = "calling {1} -> {0} action";
        private const string ACTION_KEY = "Action";
        private const string CONTROLLER_KEY = "Controller";
        private const string PARAMS_STR = "Parameters : ";
        private const string UNKNOWN_TYPE = "Unknown Type";
        private const string EXECUTION_TIME_TPL = " {1} -> {0} action is call. time: {2} sec";
        private DateTime startTime;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            startTime = DateTime.Now;
            if (!(filterContext.Controller is ChalkableController))
                throw new NotSupportedException(ChlkResources.ERR_TRACE_UNSUPPORTED_CONTROLLER);
            string message = string.Format(TRACE_TPL, filterContext.RouteData.Values[ACTION_KEY], filterContext.RouteData.Values[CONTROLLER_KEY]);

            var builder = new StringBuilder();
            builder.AppendLine(message);
            builder.AppendLine(PARAMS_STR);
            var reqParams = filterContext.HttpContext.Request.Params;
            ParameterBuilder(builder, filterContext.ActionParameters, reqParams);
            Trace.TraceInformation(builder.ToString());
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var dt = DateTime.Now - startTime;
            string message = string.Format(EXECUTION_TIME_TPL, filterContext.RouteData.Values[ACTION_KEY]
                , filterContext.RouteData.Values[CONTROLLER_KEY], dt.TotalSeconds);
            Trace.TraceInformation(message);
        }

        private void ParameterBuilder(StringBuilder builder, IDictionary<string, object> metParams, NameValueCollection reqParams)
        {
            foreach (var param in metParams)
            {
                var type = param.Value != null ? param.Value.GetType() : null;
                if (type != null && (ModelBinders.Binders.ContainsKey(type) || type.IsArray))
                {
                    builder.AppendFormat(PARAM_MESSAGE_TPL, type.Name, param.Key,
                                         reqParams.AllKeys.Contains(param.Key) ? reqParams[param.Key] : null);
                    continue;
                }
                builder.AppendFormat(PARAM_MESSAGE_TPL, type == null ? UNKNOWN_TYPE : type.Name, param.Key, param.Value);
            }
        }

    }
}