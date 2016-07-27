using System;
using System.Text;
using System.Web;
using System.Web.Routing;
using Chalkable.API;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Newtonsoft.Json;

namespace Chalkable.Web.Authentication
{
    public class ApplicationAuthentification
    {
        private const string Bearer = "Bearer";

        private static string GetTokenFromAuthorizationHeader(HttpRequestBase request )
        {
            var authHeader = request.Headers[ChalkableAuthorization.AuthenticationHeaderName];
            if (string.IsNullOrEmpty(authHeader))
                return null;

            if (string.CompareOrdinal(authHeader, 0, Bearer, 0, Bearer.Length) == 0)
                throw new ChalkableSecurityException("ACS authentication is discontinued");

            const string signature = ChalkableAuthorization.AuthenticationSignature;
            if (string.CompareOrdinal(authHeader, 0, signature, 0, signature.Length) == 0)
                return authHeader.Remove(0, signature.Length + 1).Trim();

            throw new ChalkableSecurityException("Not supported authentication method");
        }

        public static bool AuthenticateByToken(RequestContext requestContext, IApplicationService appService, out AuthorizationUserInfo authAppInfo, out Application app)
        {
            authAppInfo = null;
            app = null;

            var authToken = GetTokenFromAuthorizationHeader(requestContext.HttpContext.Request);
            if (string.IsNullOrWhiteSpace(authToken))
                return false;

            TokenAuthenticationInfo info = null;
            try
            {
                var infoJson = Encoding.UTF8.GetString(Convert.FromBase64String(authToken.Trim()));
                info = JsonConvert.DeserializeObject<TokenAuthenticationInfo>(infoJson);
            }
            catch
            {
                // ignore
            }

            if (string.IsNullOrWhiteSpace(info?.AppToken))
                throw new ChalkableSecurityException("Invalid auth token");

            var ts = ChalkableAuthorization.DateTimeToUnixTimestamp(DateTime.UtcNow.AddMinutes(-5));
            if (ts > info.Timestamp)
                throw new ChalkableSecurityException("Auth token has expired");

            try
            {
                var authInfo = EncryptionTools.AesDecrypt(info.AppToken, Chalkable.Common.Settings.AesSecretKey);
                authAppInfo = AuthorizationUserInfo.FromString(authInfo);
            }
            catch (Exception)
            {
                throw new ChalkableSecurityException("Invalid auth app token");
            }

            app = appService.GetApplicationById(authAppInfo.ApplicationId);

            if (app == null)
                throw new ChalkableSecurityException("Invalid auth app token");

            var hash = ChalkableAuthorization.ComputeSignature(requestContext.HttpContext.Request.HttpMethod, requestContext.HttpContext.Request.Url
                , requestContext.HttpContext.Request.ContentLength, info.Timestamp, info.AppToken, app.SecretKey);

            if (string.CompareOrdinal(hash, info.Signature) != 0)
                throw new ChalkableSecurityException("Auth token has invalid signature");

            return true;
        }
    }
}