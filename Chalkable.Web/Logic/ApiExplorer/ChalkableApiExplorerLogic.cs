using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public class ApiPathFinderData
    {
        public Dictionary<string, IList<string>> ParamsForMethods { get; set; }
        public Dictionary<string, string> RequiredMethodsForParams { get; set; }
        public Dictionary<string, IList<string>> RequiredParams { get; set; }
        public Dictionary<string, IList<KeyValuePair<string, string>>> CallGraph { get; set; }

        public static ApiPathFinderData Create(Dictionary<string, IList<string>> paramsForMethods,
                                               Dictionary<string, string> requiredMethodsForParams,
                                               Dictionary<string, IList<string>> requiredParams,
            Dictionary<string, IList<KeyValuePair<string, string>>> callGraph)
        {
            return new ApiPathFinderData
            {
                CallGraph = callGraph,
                ParamsForMethods = paramsForMethods,
                RequiredMethodsForParams = requiredMethodsForParams,
                RequiredParams = requiredParams
            };
        }

        public bool NeedsBuild()
        {
            return ParamsForMethods == null && RequiredMethodsForParams == null && CallGraph == null;
        }
    }

    public static class ApiPathfinder
    {
        private static Dictionary<string, ApiPathFinderData> finders = new Dictionary<string, ApiPathFinderData>();

        private const string PATHFINDER_FILE_FORMAT = "pathfinder/{0}_methods.xml";
        
        private const string PARAM_NAME = "name";
        private const string PARAM_NEED_TO_CALL = "needToCall";

        private static ApiPathFinderData BuildFor(string role)
        {
            var required = new Dictionary<string, IList<string>>();
            var returns = new Dictionary<string, IList<string>>();

            var callGraph = new Dictionary<string, IList<KeyValuePair<string, string>>>();
            var paramsFoMethods = new Dictionary<string, IList<string>>();
            var requiredMethodsForParams = new Dictionary<string, string>();

            var xml = new XmlDocument();


            var fname = AppDomain.CurrentDomain.BaseDirectory + string.Format(PATHFINDER_FILE_FORMAT, role);
            if (File.Exists(fname))
            {
                xml.Load(fname);

                var controllers = xml.DocumentElement.ChildNodes;
                foreach (XmlNode controller in controllers)
                {
                    var methods = controller.ChildNodes;
                    if (methods.Count > 0)
                    {
                        foreach (XmlNode method in methods)
                        {
                            var methodName = controller.Name + "/" + method.Name;
                            callGraph[methodName] = new List<KeyValuePair<string, string>>();

                            var requiredParams = method.FirstChild;
                            if (!required.ContainsKey(methodName)) required[methodName] = new List<string>();

                            if (requiredParams.ChildNodes.Count > 0)
                            {
                                foreach (XmlNode reqParam in requiredParams)
                                {
                                    var paramName = reqParam.Attributes[PARAM_NAME].InnerText.ToLower();
                                    required[methodName].Add(paramName);

                                    if (!paramsFoMethods.ContainsKey(paramName))
                                        paramsFoMethods[paramName] = new List<string>();
                                    paramsFoMethods[paramName].Add(methodName);

                                    if (reqParam.Attributes.Count > 1)
                                    {
                                        var methodCallName = reqParam.Attributes[PARAM_NEED_TO_CALL].InnerText;
                                        requiredMethodsForParams[paramName] = methodCallName;
                                        if (!returns.ContainsKey(methodCallName))
                                            returns[methodCallName] = new List<string>();
                                        returns[methodCallName].Add(paramName);
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var reqMethod in required)
                {
                    foreach (var retMethod in returns)
                    {
                        if (reqMethod.Key != retMethod.Key)
                        {
                            foreach (var param in retMethod.Value)
                            {
                                if (reqMethod.Value.Contains(param))
                                {
                                    var val = new KeyValuePair<string, string>(retMethod.Key, param);
                                    if (!callGraph[reqMethod.Key].Contains(val))
                                        callGraph[reqMethod.Key].Add(val);
                                }
                            }

                        }
                    }
                }
            }
            return ApiPathFinderData.Create(paramsFoMethods, requiredMethodsForParams, required, callGraph);

        }

        private static ApiPathFinderData GetApiPathfinderFor(string role)
        {
            if (!finders.ContainsKey(role))
                finders[role] = BuildFor(role);
            return finders[role];
        }

        private static IList<ApiExplorerDropdownItemViewData> GetRequiredMethodCallsForMethod(string method, string role)
        {
            var result = new Dictionary<string, IList<string>>();
            var visited = new List<string>();

            var pathfinder = GetApiPathfinderFor(role);

            var graph = pathfinder.CallGraph;
            var requiredParams = pathfinder.RequiredParams;

            if (graph.ContainsKey(method))
            {
                var st = new Stack<string>();
                st.Push(method);

                while (st.Count > 0)
                {
                    var vertex = st.Pop();
                    visited.Add(vertex);

                    if (graph.ContainsKey(vertex))
                    {
                        var adj = graph[vertex];

                        foreach (var v in adj)
                        {
                            if (!visited.Contains(v.Key))
                            {
                                st.Push(v.Key);
                            }
                        }
                    }


                    if (!result.ContainsKey(vertex))
                    {

                        if (requiredParams.ContainsKey(vertex))
                            result[vertex] = requiredParams[vertex];
                        else
                            result[vertex] = new List<string>();

                    }

                }

            }

            return result.Select(item => ApiExplorerDropdownItemViewData.Create(item.Key, true, item.Value)).Reverse().ToList();
        }

        private static IList<ApiExplorerDropdownItemViewData> GetRequiredMethodCallForParam(string param, string role)
        {
            var res = new List<ApiExplorerDropdownItemViewData>();

            var pathfinder = GetApiPathfinderFor(role);
            var requiredMethodsForParams = pathfinder.RequiredMethodsForParams;
            var requiredParams = pathfinder.RequiredParams;



            if (requiredMethodsForParams.ContainsKey(param))
            {
                var name = requiredMethodsForParams[param];
                if (requiredParams.ContainsKey(name))
                {
                    var reqParams = requiredParams[name];

                    res.Add(ApiExplorerDropdownItemViewData.Create(name, true, reqParams));

                    foreach (var requiredParam in reqParams)
                    {
                        if (requiredMethodsForParams.ContainsKey(requiredParam))
                        {
                            var methodName = requiredMethodsForParams[requiredParam];
                            if (requiredParams.ContainsKey(methodName))
                            {
                                var rParams = requiredParams[methodName];
                                res.Add(ApiExplorerDropdownItemViewData.Create(methodName, true, rParams));
                            }
                        }
                    }
                }
            }
            res.Reverse();
            return res;
        }

        public static IList<ApiExplorerDropdownItemViewData> GetParamsListByQuery(string query, string role)
        {

            var result = new List<ApiExplorerDropdownItemViewData>();
            var methods = new List<string>();
            var paramsFoMethods = GetApiPathfinderFor(role).ParamsForMethods;

            foreach (var key in paramsFoMethods)
            {
                if (key.Key.Contains(query))
                {
                    result.Add(ApiExplorerDropdownItemViewData.Create(key.Key, false));
                    methods.AddRange(key.Value);
                }
            }

            methods = methods.Distinct().ToList();
            result.AddRange(methods.Select(method => ApiExplorerDropdownItemViewData.Create(method, true)));
            return result;
        }

        public static IList<ApiExplorerDropdownItemViewData> GetRequiredMethodCallsFor(string param, bool isMethod, string role)
        {
            return isMethod ? GetRequiredMethodCallsForMethod(param, role) : GetRequiredMethodCallForParam(param, role);
        }
    }

    public static class ChalkableApiExplorerLogic
    {
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

        private static Dictionary<string, IList<ChalkableApiControllerDescription>> BuildApiExplorerDescriptions()
        {
            descriptions = new Dictionary<string, IList<ChalkableApiControllerDescription>>();
            var asm = AppDomain.CurrentDomain.GetAssemblies().
                Single(assembly => assembly.GetName().Name == ASSEMBLY_FILE);

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

                    var avRoles = attribute.Roles.Select(x => x.ToLowerInvariant()).ToList();
                    foreach (var role in avRoles)
                    {
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
            return descriptions;
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
                    using (var fs = new FileStream(fname, FileMode.Open))
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
            return descriptions ?? BuildApiExplorerDescriptions();
        }


    }
}