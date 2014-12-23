using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Web.Routing;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Microsoft.IdentityModel.Claims;
using WindowsAzure.Acs.Oauth2.Protocol.Swt;
using ClaimsPrincipal = Microsoft.IdentityModel.Claims.ClaimsPrincipal;

namespace Chalkable.Web.Authentication
{

    public class OauthAuthenticate
    {
        private static OauthAuthenticate instance;
        public static OauthAuthenticate Instance
        {
            get
            {
                if (instance == null)
                    throw new ChalkableException(ChlkResources.ERR_OUATH_AUTH_NOT_INITIALIZED);
                return instance;
            }
        }

        private const string AcsUrlFormat = "https://{0}.accesscontrol.windows.net/";

        public static void InitFromConfig()
        {
            instance = new OauthAuthenticate(
                Settings.WindowsAzureOAuthRelyingPartyRealm, 
                string.Format(AcsUrlFormat, Settings.WindowsAzureOAuthServiceNamespace),
                Settings.WindowsAzureOAuthSwtSigningKey);
        }

        private string issuer, tokenSigningKey, realm;

        public OauthAuthenticate(string realm, string issuer, string tokenSigningKey)
        {
            this.issuer = issuer;
            this.realm = realm;
            this.tokenSigningKey = tokenSigningKey;
        }

        private const string authKey = "Authorization";
        private const string bearer = "Bearer"; 

        public string GetTokenFromAuthorizationHeader(HttpRequestBase request)
        {
            var authHeader = request.Headers[authKey];
            if (authHeader == null)
                return null;
            string token = null;
            if (!string.IsNullOrEmpty(authHeader)
                && String.CompareOrdinal(authHeader, 0, bearer, 0, bearer.Length) == 0)
                token = authHeader.Remove(0, bearer.Length + 1);
            return token;
        }

        protected bool TryReadAccessToken(HttpRequestBase request, out string accessToken)
        {
            accessToken = GetTokenFromAuthorizationHeader(request);
            return !string.IsNullOrEmpty(accessToken);
        }

        public bool TryAuthenticateByToken(RequestContext requestContext)
        {
            
            string accessToken = null;
            if (TryReadAccessToken(requestContext.HttpContext.Request, out accessToken))
            {
                ClaimsIdentityCollection  claimsIdentityCollection = null;
                try
                {
                    var handler = new SimpleWebTokenHandler(issuer, tokenSigningKey);
                    var token = handler.ReadToken(accessToken);
                    // validate the token
                    claimsIdentityCollection = handler.ValidateToken(token, realm);

                    // create a claims Principal from the token
                    var claimsPrincipal = ClaimsPrincipal.CreateFromIdentities(claimsIdentityCollection);
                    if (claimsPrincipal != null)
                    {
                        if (HttpContext.Current != null)
                        {
                            HttpContext.Current.User = claimsPrincipal;
                        }
                        Thread.CurrentPrincipal = claimsPrincipal;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Trace.TraceError(ex.StackTrace); 
                    return false;      
                }
            }
            return false;
        }
    }
}