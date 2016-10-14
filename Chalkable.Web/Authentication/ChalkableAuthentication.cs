using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.WebPages;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Newtonsoft.Json;

namespace Chalkable.Web.Authentication
{
    public static class ChalkableAuthentication
    {
        public const string SESSION_KEY_COOKIE_NAME = "chlk.sid";

        private static string GenerateUID()
        {
            var ba = Guid.NewGuid().ToByteArray();
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba) hex.AppendFormat("{0:x2}", b);

            return hex.ToString();
        }

        private static string GenerateSessionKey(int? loginTimeOut)
        {
            var value = GenerateUID();
            HttpContext.Current.Response.Cookies.Set(new HttpCookie(SESSION_KEY_COOKIE_NAME)
            {
                Value = value,
                Expires = DateTime.Now.Add(loginTimeOut != null ? new TimeSpan(0, 0, loginTimeOut.Value) : FormsAuthentication.Timeout)
            });
            return value;
        }

        public static string GetSessionKey()
        {
            var httpCookie = HttpContext.Current.Request.Cookies[SESSION_KEY_COOKIE_NAME];
            if (httpCookie != null)
                return httpCookie.Value;

            httpCookie = HttpContext.Current.Response.Cookies[SESSION_KEY_COOKIE_NAME];
            return httpCookie == null ? string.Empty : httpCookie.Value;
        }

        private static void CleanSession()
        {
            var httpCookie = HttpContext.Current.Response.Cookies[SESSION_KEY_COOKIE_NAME];

            // No Cookie, no cleaning
            if (httpCookie == null) return;

            // Clean Session
            GlobalCache.CleanSession(httpCookie.Value);

            // Clean Cookie
            httpCookie.Value = null;
            httpCookie.Expires = DateTime.Now.AddYears(-1);
        }

        public static void SignIn(UserContext context, bool remember, int? loginTimeOut)
        {
            var sessionKey = GenerateSessionKey(loginTimeOut);
            
            var now = DateTime.Now;
            var ticket = new FormsAuthenticationTicket(1, context.Login, now, now.Add(loginTimeOut != null ? new TimeSpan(0, 0, loginTimeOut.Value) : FormsAuthentication.Timeout),
                remember, context.ToString(), FormsAuthentication.FormsCookiePath);
            
            var userData = new GlobalCache.UserInfo
            {
                Auth = FormsAuthentication.Encrypt(ticket),
                Claims = JsonConvert.SerializeObject(context.Claims),
                SisToken = context.SisToken
            };

            GlobalCache.SetUserInfo(sessionKey, userData, loginTimeOut != null ? new TimeSpan(0, 0, loginTimeOut.Value) : FormsAuthentication.Timeout);
        }

        public static void SignOut()
        {
            CleanSession();
        }

        public static ChalkablePrincipal GetUser()
        {
            var sessionKey = GetSessionKey();
            return GetUser(sessionKey);
        }

        public static ChalkablePrincipal GetUser(string sessionKey)
        {
            if (sessionKey == null) return null;

            var userInfo = GlobalCache.GetUserInfo(sessionKey);
            if (userInfo == null) return null;

            var ticket = FormsAuthentication.Decrypt(userInfo.Auth);
            if (ticket?.UserData == null)
                return null;

            UserContext cntx;
            if (!UserContext.TryConvertFromString(ticket.UserData, out cntx))
                return null;
            if (!string.IsNullOrEmpty(cntx.DistrictServerUrl) && !Settings.ChalkableSchoolDbServers.Contains(cntx.DistrictServerUrl))
                return null;

            cntx.SisToken = userInfo.SisToken;

            var userPermissionData = userInfo.Claims;
            if (!string.IsNullOrEmpty(userPermissionData))
            {
                cntx.Claims = JsonConvert.DeserializeObject<IList<ClaimInfo>>(userPermissionData);
            }

            return new ChalkablePrincipal(cntx);
        }

        public static void UpdateLoginTimeOut(UserContext context)
        {
            if (context?.LoginTimeOut != null)
            {
                var httpCookie = HttpContext.Current.Request.Cookies[SESSION_KEY_COOKIE_NAME];
                if (!string.IsNullOrWhiteSpace(httpCookie?.Value))
                {
                    httpCookie.Expires = DateTime.Now.AddSeconds(context.LoginTimeOut.Value);
                    HttpContext.Current.Response.Cookies.Set(httpCookie);
                    GlobalCache.UpdateExpiryUserInfo(GetSessionKey(), new TimeSpan(0, 0, context.LoginTimeOut.Value));
                }
            }
        }
    }
}