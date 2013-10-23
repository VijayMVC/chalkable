using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Authentication
{
    public static class ChalkableAuthentication
    {
        public const string COOKIE_NAME = "Chalkable Auth";
        public static void SignIn(UserContext context, bool remember)
        {
            var ticket = new FormsAuthenticationTicket(1,context.Login,DateTime.Now,DateTime.Now.Add(FormsAuthentication.Timeout),
                remember,context.ToString(),FormsAuthentication.FormsCookiePath);
            var encTicket = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(COOKIE_NAME)
            {
                Value = encTicket,
                Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
            };
            HttpContext.Current.Response.Cookies.Set(authCookie);
        }

        public static void SignOut()
        {
            var httpCookie = HttpContext.Current.Response.Cookies[COOKIE_NAME];
            if (httpCookie != null)
            {
                httpCookie.Value = string.Empty;
            }
        }

        public static ChalkablePrincipal GetUser()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies.Get(COOKIE_NAME);
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null)
                    return null;
                var cntx = UserContext.FromString(ticket.UserData);
                return new ChalkablePrincipal(cntx);
            }
            return null;
        }
    }
}