using System;
using System.Collections.Specialized;
using System.Web;
using Chalkable.API;

namespace Chalkable.Web.Tools
{
    public static class ApiRequestExtensions
    {
        private static bool HasAuthenticationHeader(NameValueCollection headers)
        {
            return !string.IsNullOrWhiteSpace(headers?[ChalkableAuthorization.AuthenticationHeaderName]);
        }

        public static bool IsApiRequest(this HttpRequestBase request)
        {         
            return HasAuthenticationHeader(request?.Headers);
        }

        public static bool IsApiRequest(this HttpRequest request)
        {
            return HasAuthenticationHeader(request?.Headers);
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request?["X-Requested-With"] == "XMLHttpRequest" || request?.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}