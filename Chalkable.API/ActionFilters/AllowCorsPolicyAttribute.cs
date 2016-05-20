using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;

namespace Chalkable.API.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllowCorsPolicyAttribute : ActionFilterAttribute
    {
        private const string DEFAULT_METHODS = "GET, POST, OPTIONS";
        private const string DEFAULT_HEADERS = "authorization, x-requested-with";
        private string _methods;
        private string _headers;
        private IList<Uri> _supportedUris;
        public AllowCorsPolicyAttribute(string headers = DEFAULT_HEADERS, string methods = DEFAULT_METHODS)
        {
            _headers = headers;
            _methods = methods;
            _supportedUris = GetSupportedUris();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var requestMethod = filterContext.HttpContext.Request.HttpMethod;
            var urlReferrer = filterContext.HttpContext.Request.UrlReferrer;
            var origin = filterContext.HttpContext.Request.Headers.Get("Origin");
            var urlOrigin = string.IsNullOrWhiteSpace(origin) ? null : new Uri(origin);

            if (requestMethod == HttpMethod.Options.Method)
            {
                filterContext.Result = new EmptyResult();
            }

            if (IsSupportCurrentUri(urlReferrer) || IsSupportCurrentUri(urlOrigin))
            {
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Methods", _methods);
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", _headers);
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Max-Age", "604800");
            }

            base.OnActionExecuting(filterContext);
        }

        private bool IsSupportCurrentUri(Uri uri)
        {
            return uri != null && _supportedUris.Any(x => x.Scheme == uri.Scheme && x.Host == uri.Host);
        }

        private static IList<Uri> GetSupportedUris()
        {
            var res = new List<Uri>();
            var appEnvs = Settings.GetApplicationEnvironments();
            for (var i = 0; i < appEnvs?.Count; i++)
            {
                var strUri = appEnvs[i].Environment;
                Uri uri;
                if (!string.IsNullOrWhiteSpace(strUri) && Uri.TryCreate(strUri, UriKind.RelativeOrAbsolute, out uri))
                {
                    res.Add(uri);
                }
            }
            return res;
        } 
    }
}
