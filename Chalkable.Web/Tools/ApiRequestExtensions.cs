using System;
using System.Web;

namespace Chalkable.Web.Tools
{
    public static class ApiRequestExtensions
    {
        public static bool IsApiRequest(this HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var authorization = request.Headers != null ? request.Headers["Authorization"] : "";
            return authorization.StartsWith("Bearer:");
        }
    }
}