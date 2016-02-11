﻿using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using WindowsAzure.Acs.Oauth2;
using WindowsAzure.Acs.Oauth2.Protocol;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IAccessControlService
    {
        string GetAccessToken(string accessTokenUrl, string redirectUrl, string clientId,
            string clientSecret, OAuthUserIdentityInfo user, string scope);

        string GetAuthorizationCode(string clientId, OAuthUserIdentityInfo user, string scope = null);
        ApplicationRegistration GetApplication(string clientId);
        bool RegisterApplication(string clientId, string appSecretKey, string appUrl, string appName);
        void RemoveApplication(string clientId);

    }

    public class AccessControlService : MasterServiceBase, IAccessControlService
    {
        private ApplicationRegistrationService regService;
        private const string GRANT_TYPE = "authorization_code";
        private const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        private const string REQUEST_METHOD = "POST";

        public AccessControlService(IServiceLocatorMaster locator)
            : base(locator)
        {
            regService = new ApplicationRegistrationService();
        }
        private AccessTokenRequestWithAuthorizationCode BuildAccessTokenRequest(Uri accessTokenUri, string clientId,
            string clientSecret, string scope, Uri redirectUri, string refreshToken)
        {
            return new AccessTokenRequestWithAuthorizationCode(accessTokenUri)
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = scope,
                GrantType = GRANT_TYPE,
                Code = refreshToken,
                RedirectUri = redirectUri
            };
        }

        private AccessTokenResponse Authorize(Uri accessTokenUri, string clientId,
            string clientSecret, string scope, Uri redirectUri, string refreshToken)
        {
            AccessTokenResponse result;
            var authorizeRequest = BuildAccessTokenRequest(accessTokenUri, clientId, clientSecret, scope, redirectUri, refreshToken);

            var serializer = new OAuthMessageSerializer();
            var encodedQueryFormat = serializer.GetFormEncodedQueryFormat(authorizeRequest);

            var httpWebRequest = WebRequest.Create(authorizeRequest.BaseUri) as HttpWebRequest;
            if (httpWebRequest != null)
            {
                httpWebRequest.Method = REQUEST_METHOD;
                httpWebRequest.ContentType = CONTENT_TYPE;
                var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                streamWriter.Write(encodedQueryFormat);
                streamWriter.Close();
            }

            try
            {
                if (httpWebRequest != null)
                {
                    var webResponse = httpWebRequest.GetResponse();
                    var response = serializer.Read(webResponse as HttpWebResponse);
                    var message = response as AccessTokenResponse;
                    if (message != null)
                    {
                        result = message;
                    }
                    else
                    {
                        var msg =
                            $"authorize message is null for [{accessTokenUri}] [{clientId}] [{clientSecret}] [{scope}] [{redirectUri}] [{refreshToken}]";
                        throw new ChalkableException(msg);
                    }
                        
                }
                else
                    throw new ChalkableException(
                        $"authorize http request is null for [{accessTokenUri}] [{clientId}] [{clientSecret}] [{scope}] [{redirectUri}] [{refreshToken}]");
            }
            catch (WebException webex)
            {
                var message = serializer.Read(webex.Response as HttpWebResponse);

                var endUserAuthorizationFailedResponse = message as EndUserAuthorizationFailedResponse;
                if (endUserAuthorizationFailedResponse != null)
                {
                    throw new AuthenticationException(endUserAuthorizationFailedResponse.ErrorDescription);
                }

                var userAuthorizationFailedResponse = message as ResourceAccessFailureResponse;
                if (userAuthorizationFailedResponse != null)
                {
                    throw new AuthenticationException(userAuthorizationFailedResponse.ErrorDescription);
                }

                throw;
            }
            return result;
        }

        public string GetAccessToken(string accessTokenUrl, string redirectUrl, string clientId,
            string clientSecret, OAuthUserIdentityInfo user, string scope)
        {
            var authorizationCode = GetAuthorizationCode(clientId, user, scope);
            var response = Authorize(new Uri(accessTokenUrl), clientId, clientSecret, scope, new Uri(redirectUrl), authorizationCode);
            if (response != null)
                return response.AccessToken;
            throw new ChalkableException(
                $"can not get authorization token for access token url {accessTokenUrl} redirect url {redirectUrl} client id {clientId} authorization code {authorizationCode}");
        }

        public string GetAuthorizationCode(string clientId, OAuthUserIdentityInfo user, string scope = null)
        {
            if (string.IsNullOrEmpty(scope))
                scope = Settings.ApiExplorerScope; //TODO: this is wrong approach

            return regService.GetAuthorizationCode(clientId,
                 new AuthorizationServerIdentity
                 {
                     IdentityProvider = "",
                     NameIdentifier = user.ToString()
                 },
                 scope);
        }


        public ApplicationRegistration GetApplication(string clientId)
        {
            return regService.GetApplication(clientId);
        }

        public bool RegisterApplication(string clientId, string appSecretKey, string appUrl, string appName)
        {
            return regService.RegisterApplication(clientId, appSecretKey, appUrl, appName);
        }

        public void RemoveApplication(string clientId)
        {
            regService.RemoveApplication(clientId);
        }
    }
}