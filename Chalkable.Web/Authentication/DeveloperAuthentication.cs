using System;
using System.Web;
using System.Web.Security;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.Web.Authentication
{
    public static class DeveloperAuthentication
    {
        public const string COOKIE_NAME = "Chalkable Auth Dev";

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
            SetAuthCookie(context.Login, DateTime.Now, remember, context.ToString(), context.SisToken, COOKIE_NAME);
        }

        
        public static void SignOut()
        {
            CleanCookie(COOKIE_NAME);
        }

        public static UserContext GetUser()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies.Get(COOKIE_NAME);
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                var sl = authCookie.Value.Split('#');
                var ticket = FormsAuthentication.Decrypt(sl[0]);
                if (ticket == null || ticket.UserData == null)
                    return null;
                var cntx = UserContext.FromString(ticket.UserData);
                if (sl.Length > 1)
                    cntx.SisToken = sl[1];
                return cntx;
            }
            return null;
        }
    }
}