using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Controllers;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ChalkableApiExplorerViewData;

namespace Chalkable.Web.Logic
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


        private const string API_EXPLORER_CLIENT_ID = "api.explorer.client-id";
        private const string API_EXPLORER_SECRET = "api.explorer.secret";
        private const string API_EXPLORER_REDIRECT_URI = "api.explorer.redirecturi";
        private const string ACS_URL_FORMAT = "https://{0}.accesscontrol.windows.net/v2/OAuth2-13/";
        private const string SERVICE_NAMESPACE = "WindowsAzure.OAuth.ServiceNamespace";
        private const string API_EXPLORER_SCOPE = "api.explorer.scope";


        public static string GetAccessTokenFor(string userName, int? schoolYearId, IServiceLocatorMaster locator)
        {
            var clientId = ConfigurationManager.AppSettings[API_EXPLORER_CLIENT_ID];
            var clientSecret = ConfigurationManager.AppSettings[API_EXPLORER_SECRET];
            var redirectUri = ConfigurationManager.AppSettings[API_EXPLORER_REDIRECT_URI];
            var accessTokenUri = string.Format(ACS_URL_FORMAT,
                                      ConfigurationManager.AppSettings[SERVICE_NAMESPACE]);

            var scope = ConfigurationManager.AppSettings[API_EXPLORER_SCOPE];
            return locator.AccessControlService.GetAccessToken(accessTokenUri, redirectUri, clientId, clientSecret, userName, schoolYearId, scope);
        }

        private static bool IsNullableType(ParameterInfo param)
        {
            return param.ParameterType.IsGenericType &&
                   param.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>)
                   || param.ParameterType == typeof(string);

        }

        private const string ASSEMBLY_FILE = "/bin/Chalkable.Web.dll";
        private const string CONTROLLER = "Controller";

        private static Dictionary<string, IList<ChalkableApiControllerDescription>> BuildApiExplorerDescriptions()
        {

            descriptions = new Dictionary<string, IList<ChalkableApiControllerDescription>>();

            var controllers =
                Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + ASSEMBLY_FILE).GetExportedTypes().
                    Where(IsChalkableController);

            var roles = new List<string>();
            var callTypes = Enum.GetNames(typeof(CallType));
            var controllersList = new List<ChalkableApiControllerDescription>();

            foreach (var controller in controllers)
            {
                var controllerName = controller.Name.Replace(CONTROLLER, "");
                var methodsInfo = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                var mList = new List<ChalkableApiMethodDescription>();

                foreach (var info in methodsInfo)
                {
                    if (info.ReturnType == typeof(ActionResult))
                    {
                        var methodParams = info.GetParameters();

                        var attribute = info.GetCustomAttributes(typeof(AuthorizationFilter), true)
                            .Cast<AuthorizationFilter>()
                            .FirstOrDefault();

                        var obsoleteAttr =
                            info.GetCustomAttributes(typeof(ObsoleteAttribute), true)
                                .Cast<ObsoleteAttribute>()
                                .FirstOrDefault();


                        if (attribute != null && attribute.ApiAccess && obsoleteAttr == null)
                        {

                            var paramsList = new List<Param>();
                            foreach (var param in methodParams)
                            {
                                var description = attribute.ParamsDescriptions != null
                                    && param.Position < attribute.ParamsDescriptions.Length
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

                            string[] avRoles = attribute.Roles;

                            if (avRoles != null)
                            {
                                for (int i = 0; i < avRoles.Length; ++i)
                                {
                                    var role = avRoles[i].ToLowerInvariant();
                                    avRoles[i] = role;

                                    if (!roles.Contains(role))
                                    {
                                        roles.Add(role);
                                    }
                                }
                            }

                            mList.Add(new ChalkableApiMethodDescription
                            {
                                Name = info.Name,
                                Description = attribute.Description ?? "",
                                Method = callTypes[(int)attribute.Type],
                                Parameters = paramsList,
                                AvailableForRoles = avRoles.ToList()
                            });
                        }
                    }

                }
                controllersList.Add(new ChalkableApiControllerDescription
                {
                    Name = controllerName,
                    Methods = mList
                });

            }

            foreach (var role in roles)
            {
                foreach (var controllerDescr in controllersList)
                {
                    var availableMethodsForRole = controllerDescr.Methods.Where(x => x.AvailableForRoles.Contains(role)).ToList();
                    if (!descriptions.ContainsKey(role))
                    {
                        descriptions.Add(role, new List<ChalkableApiControllerDescription>());
                    }
                    descriptions[role].Add(new ChalkableApiControllerDescription
                    {
                        Methods = availableMethodsForRole,
                        Name = controllerDescr.Name
                    });
                }
            }
            return descriptions;
        }

        public static Dictionary<string, IList<ChalkableApiControllerDescription>> GetApi()
        {
            return descriptions ?? BuildApiExplorerDescriptions();
        }


    }
}