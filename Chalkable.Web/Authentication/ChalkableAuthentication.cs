using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;

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

        private static string GenerateSessionKey()
        {
            var value = GenerateUID();
            HttpContext.Current.Response.Cookies.Set(new HttpCookie(SESSION_KEY_COOKIE_NAME)
            {
                Value = value,
                Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
            });
            return value;
        }

        private static string GetSessionKey()
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

        public static void SignIn(UserContext context, bool remember)
        {
            var sessionKey = GenerateSessionKey();
            
            var now = DateTime.Now;
            var ticket = new FormsAuthenticationTicket(1, context.Login, now, now.Add(FormsAuthentication.Timeout),
                remember, context.ToString(), FormsAuthentication.FormsCookiePath);

            GlobalCache.SetAuth(sessionKey, FormsAuthentication.Encrypt(ticket), FormsAuthentication.Timeout);            
            GlobalCache.SetClaims(sessionKey, new JavaScriptSerializer().Serialize(context.Claims), FormsAuthentication.Timeout);

            if (!string.IsNullOrWhiteSpace(context.SisToken))
                GlobalCache.SetSisToken(sessionKey, context.SisToken, FormsAuthentication.Timeout);
        }

        public static void SignOut()
        {
            CleanSession();
        }

        public static bool IsPersistentAuthentication()
        {
            var sessionKey = GetSessionKey();
            if (sessionKey == null) return false;

            var sl = GlobalCache.GetAuth(sessionKey);
            if (sl == null) return false;

            var ticket = FormsAuthentication.Decrypt(sl);
            return ticket != null && ticket.IsPersistent;
        }

        public static ChalkablePrincipal GetUser()
        {
            var sessionKey = GetSessionKey();
            if (sessionKey == null) return null;

            var sl = GlobalCache.GetAuth(sessionKey);
            if (sl == null) return null;

            var ticket = FormsAuthentication.Decrypt(sl);
            if (ticket == null || ticket.UserData == null)
                return null;

            var cntx = UserContext.FromString(ticket.UserData);
            cntx.SisToken = GlobalCache.GetSisToken(sessionKey);

            if (!string.IsNullOrEmpty(cntx.DistrictServerUrl) && !Settings.ChalkableSchoolDbServers.Contains(cntx.DistrictServerUrl))
                return null;

            var userPermissionData = GlobalCache.GetClaims(sessionKey);
            if (!string.IsNullOrEmpty(userPermissionData))
            {
                var serializer = new JavaScriptSerializer();
                cntx.Claims = serializer.Deserialize<IList<ClaimInfo>>(userPermissionData);
            }

            return new ChalkablePrincipal(cntx);
        }
    }
}