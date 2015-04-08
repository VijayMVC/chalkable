using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Xml;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Controllers;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ChalkableApiExplorerViewData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chalkable.Web.Logic.ApiExplorer
{

    public static class ChalkableApiExplorerLogic
    {

        public static Boolean IsValidApiRole(string roleName)
        {
            var loweredRoleName = roleName.ToLowerInvariant();
            return
                loweredRoleName != CoreRoles.SUPER_ADMIN_ROLE.LoweredName &&
                loweredRoleName != CoreRoles.CHECKIN_ROLE.LoweredName &&
                loweredRoleName != CoreRoles.ADMIN_GRADE_ROLE.LoweredName &&
                loweredRoleName != CoreRoles.ADMIN_VIEW_ROLE.LoweredName &&
                loweredRoleName != CoreRoles.ADMIN_EDIT_ROLE.LoweredName;
        }

        private static Dictionary<string, IList<ChalkableApiControllerDescription>> descriptions;
        private static IList<ChalkableApiControllerDescription> controllerList; 
        private static Dictionary<string, Dictionary<string, string>> fakeResponses;
        private static HashSet<string> rolesList; 

        private static bool IsChalkableController(Type t)
        {
            return t != null && t.IsPublic
                && t.Name.EndsWith(CONTROLLER, StringComparison.OrdinalIgnoreCase)
                && !t.IsAbstract && typeof(ChalkableController).IsAssignableFrom(t);
        }

        private static ApiMethodParamType GetParameterType(Type t)
        {
            if (t == typeof(int) || t == typeof(int?)) return ApiMethodParamType.Integer;
            if (t == typeof(string)) return ApiMethodParamType.String;
            if (t == typeof(bool) || t == typeof(bool?)) return ApiMethodParamType.Boolean;
            if (t == typeof(IntList)) return ApiMethodParamType.IntList;
            if (t == typeof(Guid) || t == typeof(Guid?)) return ApiMethodParamType.Guid;
            if (t == typeof(GuidList)) return ApiMethodParamType.GuidList;
            if (t == typeof(ListOfStringList)) return ApiMethodParamType.ListOfStringList;
            if (t == typeof(DateTime) || t == typeof(DateTime?)) return ApiMethodParamType.Date;
            return ApiMethodParamType.Undefined;
        }

        private static void PrepareControllerMap()
        {//write method for paramatrized type that builds info about controller action, description, default params
            RegisterApiMethodDefaults<ClassController>(x => x.List(1, 1, 1, null, null));
            RegisterApiMethodDefaults<ClassController>(x => x.List(1, 1, 1, null, null));
        }

        private static void RegisterApiMethodDefaults<T>(Action<T> func) where T: ChalkableController
        {
            
        }

        private static bool IsNullableType(ParameterInfo param)
        {
            return param.ParameterType.IsGenericType &&
                   param.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>)
                   || param.ParameterType == typeof(string);

        }

        private const string ASSEMBLY_FILE = "Chalkable.Web";
        private const string CONTROLLER = "Controller";

        private static void BuildApiExplorerDescriptions()
        {
            var controllers = BuildControllerList();
            var chalkableApiControllerDescriptions = controllers as IList<ChalkableApiControllerDescription> ?? controllers.ToList();

            foreach (var controller in chalkableApiControllerDescriptions)
            {
                foreach (var method in controller.Methods)
                {
                    method.Description = PreferenceService.Get(method.Description).Value;
                    foreach (var param in method.Parameters)
                    {
                        param.Description = PreferenceService.Get(param.Description).Value;
                    }
                }
            }
            BuildFakeResponses(rolesList);

            descriptions = new Dictionary<string, IList<ChalkableApiControllerDescription>>();

            
            foreach (var role in rolesList)
            {
                foreach (var controllerDescr in chalkableApiControllerDescriptions)
                {
                    var availableMethodsForRole = controllerDescr.Methods.Where(x => x.AvailableForRoles.Contains(role)).ToList();
                    if (!descriptions.ContainsKey(role))
                    {
                        descriptions.Add(role, new List<ChalkableApiControllerDescription>());
                    }

                    availableMethodsForRole = PrepareFakeResponsesForMethods(controllerDescr.Name, availableMethodsForRole, role);

                    descriptions[role].Add(new ChalkableApiControllerDescription
                    {
                        Methods = availableMethodsForRole,
                        Name = controllerDescr.Name
                    });
                }
            }            
        }

        public static IEnumerable<ChalkableApiControllerDescription> BuildControllerList()
        {
            if (controllerList != null)
                return controllerList;

            var asm =
                BuildManager.GetReferencedAssemblies()
                    .Cast<Assembly>()
                    .Single(assembly => assembly.GetName().Name == ASSEMBLY_FILE);
            var controllers = asm.GetExportedTypes().Where(IsChalkableController);

            rolesList = new HashSet<string>();
            controllerList = new List<ChalkableApiControllerDescription>();

            foreach (var controller in controllers)
            {
                var controllerName = controller.Name.Replace(CONTROLLER, "");
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                var mList = new List<ChalkableApiMethodDescription>();

                foreach (var method in methods)
                {
                    if (method.ReturnType != typeof (ActionResult)) continue;

                    var methodParams = method.GetParameters();

                    var attribute = method.GetCustomAttributes(typeof (AuthorizationFilter), true)
                        .Cast<AuthorizationFilter>()
                        .FirstOrDefault();

                    var obsoleteAttr =
                        method.GetCustomAttributes(typeof (ObsoleteAttribute), true)
                            .Cast<ObsoleteAttribute>()
                            .FirstOrDefault();


                    if (attribute == null || !attribute.ApiAccess || obsoleteAttr != null)
                        continue;

                    var paramsList = new List<ApiMethodParam>();
                    foreach (var param in methodParams)
                    {
                        var paramKey = BuildParametherDescriptionKey(controllerName, method.Name, param.Name);
                        paramsList.Add(new ApiMethodParam
                        {
                            Name = param.Name,
                            Value = param.DefaultValue.ToString(),
                            Description = paramKey,
                            IsNullable = IsNullableType(param),
                            ParamType = GetParameterType(param.ParameterType)
                        });
                    }

                    var avRoles = attribute.Roles
                        .Select(x => x.ToLowerInvariant()).ToList();

                    foreach (var role in avRoles)
                    {
                        if (IsValidApiRole(role))
                            rolesList.Add(role);
                    }

                    var methodKey = BuildMethodDescriptionKey(controllerName, method.Name);

                    mList.Add(new ChalkableApiMethodDescription
                    {
                        Name = method.Name,
                        Description = methodKey,
                        Method = CallType.Get.ToString(),
                        Parameters = paramsList,
                        AvailableForRoles = avRoles
                    });
                }
                controllerList.Add(new ChalkableApiControllerDescription
                {
                    Name = controllerName,
                    Methods = mList
                });
            }
            return controllerList;
        }

        private const string FAKE_RESPONSES_FILE = "pathfinder/{0}_fake_method_responses.json";

        private static void BuildFakeResponses(IEnumerable<string> roles)
        {
            if (fakeResponses != null) return;

            fakeResponses = new Dictionary<string, Dictionary<string, string>>();
            foreach (var role in roles)
            {
                var roleFakeResponses = new Dictionary<string, string>();
                var fname = AppDomain.CurrentDomain.BaseDirectory + string.Format(FAKE_RESPONSES_FILE, role);

                if (File.Exists(fname))
                {
                    var json = "";
                    using (var fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            json = sr.ReadToEnd();
                        }
                    }

                    var res = JsonConvert.DeserializeObject<JObject>(json);

                    var controllers = res.Value<JObject>("controllers");
                    foreach (var controller in controllers.Properties())
                    {
                        var controllerName = controller.Name;
                        var methodsInfo = controllers.Value<JObject>(controllerName);
                        foreach (var method in methodsInfo.Properties())
                        {
                            var methodName = method.Name;
                            var response = methodsInfo.Value<JObject>(methodName).ToString();
                            roleFakeResponses[controller.Name + "/" + method.Name] = response;
                        }                        
                    }
                }
                fakeResponses[role] = roleFakeResponses;
            }
        }

        private static List<ChalkableApiMethodDescription> PrepareFakeResponsesForMethods(string controller,
            List<ChalkableApiMethodDescription> availableMethodsForRole, String role)
        {
            var responses = fakeResponses[role];

            foreach (var chalkableApiMethodDescription in availableMethodsForRole)
            {
                var key = controller + "/" + chalkableApiMethodDescription.Name;

                if (responses.ContainsKey(key))
                    chalkableApiMethodDescription.Response = responses[key];
            }
            return availableMethodsForRole;
        }

        public static Dictionary<string, IList<ChalkableApiControllerDescription>> GetApi()
        {
            if (descriptions != null && descriptions.Count > 0)
                return descriptions;
            BuildApiExplorerDescriptions();
            return descriptions;
        }


        public static IList<string> GenerateControllerDescriptionsKeys()
        {
            var controllers = BuildControllerList();
            IList<string> keys = new List<string>();
            foreach (var controller in controllers)
            {
                foreach (var method in controller.Methods)
                {
                    keys.Add(BuildMethodDescriptionKey(controller.Name, method.Name));
                    foreach (var parameter in method.Parameters)
                    {
                        keys.Add(BuildParametherDescriptionKey(controller.Name, method.Name, parameter.Name));
                    }
                }
            }
            return keys;
        } 


        private static string BuildMethodDescriptionKey(string controllerName, string methodName)
        {
            return string.Format("{0}_{1}", controllerName, methodName);
        }

        private static string BuildParametherDescriptionKey(string controller, string method, string parameter)
        {
            return string.Format("{0}_{1}_{2}", controller, method, parameter);
        }

    }
}