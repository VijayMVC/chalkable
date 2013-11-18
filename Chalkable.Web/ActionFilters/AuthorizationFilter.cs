using System;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.Controllers;

namespace Chalkable.Web.ActionFilters
{

    public enum CallType
    {
        Get = 0,
        Post = 1
    };
    public class AuthorizationFilter : ActionFilterAttribute
    {
        private string[] role;
        private bool apiAccess;
        private string description;
        private CallType callType;
        private AppPermissionType[] permissions;

        //TODO: API permissions

        public AuthorizationFilter(string role, string descriptionKey = "", bool apiAccess = false, CallType methodCallType = CallType.Post, AppPermissionType[] permissions = null)
        {
            this.callType = methodCallType;

            if (!string.IsNullOrEmpty(descriptionKey))
            {
                var info = PreferenceService.Get(descriptionKey);
                if (info != null) this.description = info.Value;
            }
            if (string.IsNullOrEmpty(role))
                this.role = null;
            else
                this.role = role.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            this.apiAccess = apiAccess;
            this.permissions = permissions;
        }

        public AuthorizationFilter()
        {
            role = null;
            apiAccess = false;
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
                if (context.IsOAuthUser)
                {
                    if (!apiAccess)
                    {
                        filterContext.Result = controller.Json(new ChalkableSecurityException(ChlkResources.ERR_CANT_CALL_METHOD_AS_API));
                        return;    
                    }
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

                if (role == null)
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
                            if (role.Any(r => context.Role.LoweredName == r.ToLower()))
                            {
                                allow = true;
                            }
                        }
                    }
            }
            if (!allow)
            {
                
                if (controller.RouteData.Values.ContainsKey(format) &&
                    controller.RouteData.Values[format].ToString().ToLower() != htmlFormat
                    && controller.RouteData.Values[format].ToString().ToLower() != aspxFormat)
                {
                    filterContext.RequestContext.HttpContext.Response.AddHeader(requiresAuth, "1");
                }                   
                else
                    filterContext.Result = controller.Redirect<HomeController>(c => c.Index());
            }
            base.OnActionExecuting(filterContext);
        }
        private const string format = "format";
        private const string htmlFormat = "html";
        private const string aspxFormat = "aspx";
        private const string requiresAuth = "REQUIRES_AUTH";

        public string[] Roles { get { return role; } }
        public string[] ParamsDescriptions { get { return null; } }
        public string Description { get { return description; } }
        public CallType Type { get { return callType; } }
        public bool ApiAccess { get { return apiAccess; } }
    }


    

}