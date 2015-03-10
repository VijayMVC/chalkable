using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Newtonsoft.Json;

namespace Chalkable.Web.Authentication
{
    public static class ChalkableAuthentication
    {
        public const string COOKIE_NAME_1 = "Chalkable Auth";
        public const string USER_PERMISSION_COOKIE_NAME = "User Permission";
        public const string USER_PERMISSION_COOKIES_COUNT = "User Permission Cookies Count";
        public const int MAX_COOKIE_LENGTH = 4096;

        private static void SetAuthCookie(string login, DateTime now, bool remember, string data, string sisTicket, string cookieName)
        {
            var ticket = new FormsAuthenticationTicket(1, login, now, now.Add(FormsAuthentication.Timeout),
                remember, data, FormsAuthentication.FormsCookiePath);

            var encTicket = FormsAuthentication.Encrypt(ticket);
            encTicket += "#" + (sisTicket ?? "");
            SetCookie(cookieName, now, encTicket);
        }

        private static void SetCookie(string cookieName, DateTime now, string value)
        {
            HttpContext.Current.Response.Cookies.Set(new HttpCookie(cookieName)
            {
                Value = value,
                Expires = now.Add(FormsAuthentication.Timeout)
            });
        }
        private static void CleanCookie(string coockieName)
        {
            var httpCookie = HttpContext.Current.Response.Cookies[coockieName];
            if (httpCookie != null)
            {
                httpCookie.Value = string.Empty;
            }
        }

        public static void SignIn(UserContext context, bool remember)
        {
            DateTime now = DateTime.Now;
            SetAuthCookie(context.Login, now, remember, context.ToString(), context.SisToken, COOKIE_NAME_1);
            var jsonSerializer = new JavaScriptSerializer();
            int index = 1;
            //SetCookie(USER_PERMISSION_COOKIE_NAME, now, jsonSerializer.Serialize(context.Claims), ref index);
            SetClaimsToCookie(USER_PERMISSION_COOKIE_NAME, now, jsonSerializer.Serialize(context.Claims), ref index);
            SetCookie(USER_PERMISSION_COOKIES_COUNT, now, index.ToString());
        }

        private static void SetClaimsToCookie(string key, DateTime now, string data, ref int index)
        {
            var d = data;
            if (d.Length > MAX_COOKIE_LENGTH)
            {
                int newLength = d.Length/2;
                var s1 = d.Substring(0, newLength);
                SetClaimsToCookie(key, now, s1, ref index);
                index++;
                var s2 = d.Substring(newLength, d.Length - newLength);
                SetClaimsToCookie(key, now, s2, ref index);
            }
            else SetCookie(string.Format("{0} {1}", key, index), now, d);
        }

        private static string GetClaimsDataFromCookie(string key)
        {
            var cookiesCountCookie = HttpContext.Current.Request.Cookies.Get(USER_PERMISSION_COOKIES_COUNT);
            if (cookiesCountCookie != null && !string.IsNullOrEmpty(cookiesCountCookie.Value))
            {
                var count = int.Parse(cookiesCountCookie.Value);
                var res = new StringBuilder();
                for (int i = 1; i <= count; i++)
                {
                    var userClaimsCookie = HttpContext.Current.Request.Cookies.Get(string.Format("{0} {1}", key, i));
                    if (userClaimsCookie != null && !string.IsNullOrEmpty(userClaimsCookie.Value))
                        res.Append(userClaimsCookie.Value);
                }
                return res.ToString();
            }
            return null;
        }
        private static void CleanClaimsCookie(string key)
        {
            var cookiesCountCookie = HttpContext.Current.Request.Cookies.Get(USER_PERMISSION_COOKIES_COUNT);
            if (cookiesCountCookie != null && !string.IsNullOrEmpty(cookiesCountCookie.Value))
            {
                var count = int.Parse(cookiesCountCookie.Value);
                for (int i = 1; i <= count; i++)
                {
                    CleanCookie(string.Format("{0} {1}", key, i));
                }
            }
            CleanCookie(USER_PERMISSION_COOKIES_COUNT);
        }

        public static void SignOut()
        {
            CleanCookie(COOKIE_NAME_1);
            CleanClaimsCookie(USER_PERMISSION_COOKIE_NAME);
        }

        public static ChalkablePrincipal GetUser()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies.Get(COOKIE_NAME_1);
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                var sl = authCookie.Value.Split('#');
                var ticket = FormsAuthentication.Decrypt(sl[0]);
                if (ticket == null || ticket.UserData == null)
                    return null;
                var cntx = UserContext.FromString(ticket.UserData);
                if (sl.Length > 1)
                    cntx.SisToken = sl[1];

                if (!string.IsNullOrEmpty(cntx.DistrictServerUrl) && !Settings.ChalkableSchoolDbServers.Contains(cntx.DistrictServerUrl))
                    return null;

                var userPermissionData = GetClaimsDataFromCookie(USER_PERMISSION_COOKIE_NAME);
                if (!string.IsNullOrEmpty(userPermissionData))
                {
                    var serializer = new JavaScriptSerializer();
                    cntx.Claims = serializer.Deserialize<IList<ClaimInfo>>(userPermissionData);
                }
                return new ChalkablePrincipal(cntx);
            }
            return null;
        }
    }
}