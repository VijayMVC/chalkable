using System;
using System.Globalization;
using System.Web;

namespace Chalkable.Web.Tools
{
    public static class ApiRequestExtensions
    {
        private static bool IsBearer(string s)
        {
            return s.StartsWith("Bearer", true, CultureInfo.InvariantCulture);
        }

        public static bool IsApiRequest(this HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var authorization = request.Headers != null ? request.Headers["Authorization"] : null;
            return IsBearer(authorization ?? "");
        }

        public static bool IsApiRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var authorization = request.Headers != null ? request.Headers["Authorization"] : null;
            return IsBearer(authorization ?? "");
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest"));
        }
    }
}