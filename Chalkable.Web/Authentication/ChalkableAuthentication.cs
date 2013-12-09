using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Chalkable.BusinessLogic.Services;
using Newtonsoft.Json;

namespace Chalkable.Web.Authentication
{
    public static class ChalkableAuthentication
    {
        public const string COOKIE_NAME_1 = "Chalkable Auth";
        public const string USER_PERMISSION_COOKIE_NAME = "User Permission";

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
            SetCookie(USER_PERMISSION_COOKIE_NAME, now, jsonSerializer.Serialize(context.Claims));
        }

        public static void SignOut()
        {
            CleanCookie(COOKIE_NAME_1);
            CleanCookie(USER_PERMISSION_COOKIE_NAME);
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
                var userPermissionCoockie = HttpContext.Current.Request.Cookies.Get(USER_PERMISSION_COOKIE_NAME);
                if (userPermissionCoockie != null && !string.IsNullOrEmpty(userPermissionCoockie.Value))
                {
                    var serializer = new JavaScriptSerializer();
                    cntx.Claims = serializer.Deserialize<IList<StiConnector.Connectors.Model.Claim>>(userPermissionCoockie.Value);
                }
                return new ChalkablePrincipal(cntx);
            }
            return null;
        }
    }
}