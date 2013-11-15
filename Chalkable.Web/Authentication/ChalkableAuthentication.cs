using System;
using System.Web;
using System.Web.Security;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.Web.Authentication
{
    public static class ChalkableAuthentication
    {
        public const string COOKIE_NAME_1 = "Chalkable Auth";
        public const string COOKIE_NAME_2 = "Chalkable Auth 2";

        private static void SetAuthCoockie(string login, DateTime now, bool remember, string data, string coockieName)
        {
            var ticket = new FormsAuthenticationTicket(1, login, now, now.Add(FormsAuthentication.Timeout),
                remember, data, FormsAuthentication.FormsCookiePath);

            var encTicket = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(coockieName)
            {
                Value = encTicket,
                Expires = now.Add(FormsAuthentication.Timeout)
            };
            HttpContext.Current.Response.Cookies.Set(authCookie);
        }

        public static void SignIn(UserContext context, bool remember)
        {
            var s = context.ToString();
            var mid = s.Length / 2;
            var s1 = s.Substring(0, mid);
            var s2 = s.Substring(mid);
            DateTime now = DateTime.Now;
            SetAuthCoockie(context.Login, now, remember, s1, COOKIE_NAME_1);
            SetAuthCoockie(context.Login, now, remember, s2, COOKIE_NAME_2);
        }

        public static void SignOut()
        {
            var httpCookie = HttpContext.Current.Response.Cookies[COOKIE_NAME_1];
            if (httpCookie != null)
            {
                httpCookie.Value = string.Empty;
            }

            httpCookie = HttpContext.Current.Response.Cookies[COOKIE_NAME_2];
            if (httpCookie != null)
            {
                httpCookie.Value = string.Empty;
            }
        }

        private static string ReadAuthCoockie(string name)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies.Get(name);
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null)
                    return null;
                return ticket.UserData;
            }
            return null;
        }

        public static ChalkablePrincipal GetUser()
        {
            var s1 = ReadAuthCoockie(COOKIE_NAME_1);
            var s2 = ReadAuthCoockie(COOKIE_NAME_2);
            if (!string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2))
            {
                var cntx = UserContext.FromString(s1 + s2);
                return new ChalkablePrincipal(cntx);
            }
            return null;
        }
    }
}