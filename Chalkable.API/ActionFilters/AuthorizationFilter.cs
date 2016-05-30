using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using Chalkable.API.Controllers;

namespace Chalkable.API.ActionFilters
{
    public class AuthorizationFilter: ActionFilterAttribute
    {
        private IEnumerable<string> Roles { get; } 

        public AuthorizationFilter(string roles = null)
        {
            Roles = (roles ?? string.Empty).ToLowerInvariant().Split(',').Select(x => x.Trim());
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!(filterContext.Controller is IBaseController))
                throw new NotSupportedException("AuthorizationFilter works only on IBaseController descedents");

            var controller = (IBaseController) filterContext.Controller;

            if (controller.ChalkableAuthorization == null || controller.CurrentUser == null)
                throw new SecurityException("Not authorized");

            if (Roles != null && Roles.Any() && !Roles.Contains(controller.CurrentUser.Role.LoweredName))
                throw new SecurityException("Forbidden");

            base.OnActionExecuting(filterContext);
        }
    }
}