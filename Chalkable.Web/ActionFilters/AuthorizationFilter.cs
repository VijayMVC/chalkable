using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.Controllers;

namespace Chalkable.Web.ActionFilters
{
    public class AuthorizationFilter : ActionFilterAttribute
    {
        private AppPermissionType[] permissions;

        //TODO: API permissions

        public AuthorizationFilter(string roles, bool apiAccess = false, AppPermissionType[] permissions = null)
        {
            Roles = string.IsNullOrEmpty(roles) ? null : roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            this.ApiAccess = apiAccess;
            this.permissions = permissions;
        }

        public AuthorizationFilter()
        {
            Roles = null;
            ApiAccess = false;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool allow = false;
            if (!(filterContext.Controller is ChalkableController))
                throw new NotSupportedException(ChlkResources.ERR_AUTH_FILTER_UNSUPPORTED_CONTROLLER);
            var controller = (filterContext.Controller as ChalkableController);
            var context = controller.MasterLocator != null ? controller.MasterLocator.Context : null;
            if (context != null)
            {
                var hasApiAccess = ApiAccess || context.IsInternalApp;
                if (context.IsOAuthUser)
                {
                    if (!hasApiAccess)
                    {
                        filterContext.Result = controller.Json(new ChalkableSecurityException(ChlkResources.ERR_CANT_CALL_METHOD_AS_API));
                        return;    
                    }
                    if (!context.IsInternalApp)
                    {
                        if (permissions == null)
                        {
                            filterContext.Result = controller.Json(new ChalkableSecurityException(ChlkResources.ERR_REQUIRED_PERMISSIONS_NOT_SET_FOR_METHOD));
                            return;
                        }
                        if (context.AppPermissions == null)
                        {
                            filterContext.Result = controller.Json(new ChalkableSecurityException(ChlkResources.ERR_METHOD_CALLED_DOESNT_HAVE_PERMISSIONS));
                            return;
                        }
                        foreach (var appPermissionType in permissions)
                        {
                            if (context.AppPermissions.All(x => x != appPermissionType))
                            {
                                filterContext.Result = controller.Json(new ChalkableSecurityException(string.Format(ChlkResources.ERR_METHOD_CALLER_DOES_NOT_HAVE_PERMISSION, appPermissionType)));
                                return;
                            }
                        }    
                    }
                }

                if (Roles == null)
                {
                    if (filterContext.RequestContext.HttpContext.User != null)
                        allow = filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated;
                }
                else
                    if (filterContext.RequestContext.HttpContext.User != null)
                    {
                        var userName = filterContext.RequestContext.HttpContext.User.Identity.Name;
                        if (userName != null)
                        {
                            if (Roles.Any(r => context.Role.LoweredName == r.ToLower()))
                            {
                                allow = true;
                            }
                        }
                    }
            }
            if (!allow)
            {
                if (controller.Request.IsAjaxRequest())
                    throw new HttpException(401, "Unauthorized access");
                filterContext.Result = controller.Redirect<HomeController>(c => c.Index());
            }
            base.OnActionExecuting(filterContext);
        }

        public string[] Roles { get; private set; }
        public bool ApiAccess { get; private set; }
    }


    

}