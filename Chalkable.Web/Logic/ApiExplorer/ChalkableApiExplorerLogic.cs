using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Xml;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
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
        private static Dictionary<string, Dictionary<string, string>> fakeResponses;

        private static bool IsChalkableController(Type t)
        {
            return t != null && t.IsPublic
                && t.Name.EndsWith(CONTROLLER, StringComparison.OrdinalIgnoreCase)
                && !t.IsAbstract && typeof(ChalkableController).IsAssignableFrom(t);
        }

        private static ParamType GetParameterType(Type t)
        {
            if (t == typeof(int) || t == typeof(Nullable<int>)) return ParamType.Integer;
            if (t == typeof(string)) return ParamType.String;
            if (t == typeof(bool) || t == typeof(Nullable<bool>)) return ParamType.Boolean;
            if (t == typeof(IntList)) return ParamType.IntList;
            if (t == typeof(Guid) || t == typeof(Nullable<Guid>)) return ParamType.Guid;
            if (t == typeof(GuidList)) return ParamType.GuidList;
            if (t == typeof(ListOfStringList)) return ParamType.ListOfStringList;
            if (t == typeof(DateTime) || t == typeof(Nullable<DateTime>)) return ParamType.Date;
            return ParamType.Undefined;
        }


        private const string AcsUrlFormat = "https://{0}.accesscontrol.windows.net/v2/OAuth2-13/";


        public static string GetAccessTokenFor(string userName, int? schoolYearId, IServiceLocatorMaster locator)
        {
            var clientId = Settings.ApiExplorerClientId;
            var clientSecret = Settings.ApiExplorerSecret;
            var redirectUri = Settings.ApiExplorerRedirectUri;
            var accessTokenUri = string.Format(AcsUrlFormat, Settings.WindowsAzureOAuthServiceNamespace);
            var scope = Settings.ApiExplorerScope;
            return locator.AccessControlService.GetAccessToken(accessTokenUri, redirectUri, clientId, clientSecret, userName, schoolYearId, scope);
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
            Trace.WriteLine("#123 API LIST START");
            descriptions = new Dictionary<string, IList<ChalkableApiControllerDescription>>();


            Trace.WriteLine("#123 API LIST TEST 1");
            var asm =
                BuildManager.GetReferencedAssemblies()
                    .Cast<Assembly>()
                    .Single(assembly => assembly.GetName().Name == ASSEMBLY_FILE);
            Trace.WriteLine("controller enum start {0}", asm.FullName);
            var controllers = asm.GetExportedTypes().Where(IsChalkableController);

            var roles = new HashSet<string>();
            var callTypes = Enum.GetNames(typeof(CallType));
            var controllersList = new List<ChalkableApiControllerDescription>();

            foreach (var controller in controllers)
            {

                Trace.WriteLine("enumerating {0}", controller.Name);
                var controllerName = controller.Name.Replace(CONTROLLER, "");
                var methodsInfo = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                var mList = new List<ChalkableApiMethodDescription>();

                foreach (var info in methodsInfo)
                {
                    if (info.ReturnType != typeof (ActionResult)) continue;

                    var methodParams = info.GetParameters();

                    var attribute = info.GetCustomAttributes(typeof(AuthorizationFilter), true)
                        .Cast<AuthorizationFilter>()
                        .FirstOrDefault();

                    var obsoleteAttr =
                        info.GetCustomAttributes(typeof(ObsoleteAttribute), true)
                            .Cast<ObsoleteAttribute>()
                            .FirstOrDefault();


                    if (attribute == null || !attribute.ApiAccess || obsoleteAttr != null) 
                        continue;

                    var paramsList = new List<Param>();
                    foreach (var param in methodParams)
                    {
                        var description = attribute.ParamsDescriptions != null && param.Position < attribute.ParamsDescriptions.Length
                            ? attribute.ParamsDescriptions[param.Position] : "";
                        paramsList.Add(new Param
                        {
                            Name = param.Name,
                            Value = param.DefaultValue.ToString(),
                            Description = description,
                            IsNullable = IsNullableType(param),
                            ParamType = GetParameterType(param.ParameterType)
                        });
                    }

                    var avRoles = attribute.Roles
                        .Select(x => x.ToLowerInvariant()).ToList();

                    foreach (var role in avRoles)
                    {
                       if (IsValidApiRole(role))
                            roles.Add(role);
                    }

                    mList.Add(new ChalkableApiMethodDescription
                    {
                        Name = info.Name,
                        Description = attribute.Description ?? "",
                        Method = callTypes[(int)attribute.Type],
                        Parameters = paramsList,
                        AvailableForRoles = avRoles
                    });
                }
                controllersList.Add(new ChalkableApiControllerDescription
                {
                    Name = controllerName,
                    Methods = mList
                });

            }

            if (fakeResponses == null) BuildFakeResponses(roles);

            foreach (var role in roles)
            {
                foreach (var controllerDescr in controllersList)
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

        private const string FAKE_RESPONSES_FILE = "pathfinder/{0}_fake_method_responses.json";
        private static void BuildFakeResponses(IEnumerable<string> roles)
        {
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

        private static List<ChalkableApiMethodDescription> PrepareFakeResponsesForMethods(string controller,List<ChalkableApiMethodDescription> availableMethodsForRole, String role)
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
            Trace.WriteLine("#123 GetApi() ENTERED");
            if (descriptions != null && descriptions.Count > 0)
                return descriptions;

            Trace.WriteLine("#123 BuildApiExplorerDescriptions() ENTERED");
            BuildApiExplorerDescriptions();
            Trace.WriteLine("#123 BuildApiExplorerDescriptions() COMPLETE");

            return descriptions;
        }


        public static IList<string> GenerateControllerDescriptionKeys()
        {
            var apis = GetApi();
            var controllers = new List<ChalkableApiControllerDescription>();
            IList<string> keys = new List<string>();
            foreach (var api in apis.Values)
            {
                controllers.AddRange(api.Where(x => controllers.All(y => y.Name != x.Name)));
            }
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