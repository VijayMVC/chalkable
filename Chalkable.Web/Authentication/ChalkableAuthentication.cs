using System;
using System.Web;
using System.Web.Security;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.Web.Authentication
{
    public static class ChalkableAuthentication
    {
        public const string COOKIE_NAME_1 = "Chalkable Auth";

        private static void SetAuthCoockie(string login, DateTime now, bool remember, string data, string sisTicket, string coockieName)
        {
            var ticket = new FormsAuthenticationTicket(1, login, now, now.Add(FormsAuthentication.Timeout),
                remember, data, FormsAuthentication.FormsCookiePath);

            var encTicket = FormsAuthentication.Encrypt(ticket);
            encTicket += "#" + (sisTicket ?? "");
            var authCookie = new HttpCookie(coockieName)
            {
                Value = encTicket,
                Expires = now.Add(FormsAuthentication.Timeout)
            };
            HttpContext.Current.Response.Cookies.Set(authCookie);
        }

        public static void SignIn(UserContext context, bool remember)
        {
            DateTime now = DateTime.Now;
            SetAuthCoockie(context.Login, now, remember, context.ToString(), context.SisToken, COOKIE_NAME_1);
        }

        public static void SignOut()
        {
            var httpCookie = HttpContext.Current.Response.Cookies[COOKIE_NAME_1];
            if (httpCookie != null)
            {
                httpCookie.Value = string.Empty;
            }
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
                return new ChalkablePrincipal(cntx);
            }
            return null;
        }
    }
}