using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Controllers;
using Chalkable.Web.Controllers.AnnouncementControllers;
using Chalkable.Web.Controllers.CalendarControllers;
using Chalkable.Web.Controllers.PersonControllers;
using Chalkable.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chalkable.Web.Logic.ApiExplorer
{

    internal enum ApiMethodCallType
    {
        Get,
        Post
    };

    public static class ChalkableApiExplorerLogic
    {

        public static Boolean IsValidApiRole(string roleName)
        {
            var loweredRoleName = roleName.ToLowerInvariant();
            return
                loweredRoleName != CoreRoles.SUPER_ADMIN_ROLE.LoweredName &&
                loweredRoleName != CoreRoles.CHECKIN_ROLE.LoweredName &&
                loweredRoleName != CoreRoles.DISTRICT_ADMIN_ROLE.LoweredName;
        }

        private static Dictionary<string, IList<ChalkableApiControllerDescription>> descriptions;
        private static IDictionary<string, ChalkableApiControllerDescription> controllerList; 
        private static Dictionary<string, Dictionary<string, string>> fakeResponses;
        private static HashSet<string> roles; 

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
        {
            //do this once
            
            RegisterApiMethodDefaults<ApplicationController>(x => x.GetAnnouncementApplication(1), ApiMethodCallType.Post);
            RegisterApiMethodDefaults<AttendanceController>(x => x.SetAttendanceForClass("A", null, DemoSchoolConstants.AlgebraClassId, DateTime.Today, true),ApiMethodCallType.Post);
            RegisterApiMethodDefaults<AttendanceController>(x => x.ClassList(null, DemoSchoolConstants.AlgebraClassId), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<AttendanceController>(x => x.AttendanceSummary(DateTime.Today), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<AttendanceController>(x => x.SeatingChart(null, DemoSchoolConstants.AlgebraClassId), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<AttendanceReasonController>(x => x.List(), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<AnnouncementCalendarController>(x => x.List(DateTime.Today, DemoSchoolConstants.AlgebraClassId, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<AnnouncementCalendarController>(x => x.Week(DateTime.Today, DemoSchoolConstants.AlgebraClassId, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<AnnouncementCalendarController>(x => x.Day(DateTime.Today, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<AnnouncementController>(x => x.Read(1, (int)AnnouncementTypeEnum.Class), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<AnnouncementTypeController>(x => x.List(DemoSchoolConstants.AlgebraClassId), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<ClassController>(x => 
                x.List(DemoSchoolConstants.CurrentSchoolYearId, DemoSchoolConstants.FirstMarkingPeriodId, DemoSchoolConstants.TeacherId, null, null), ApiMethodCallType.Post);
            RegisterApiMethodDefaults<DisciplineController>(x => x.ClassList(DateTime.Today, DemoSchoolConstants.AlgebraClassId, null, null, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<DisciplineController>(x => x.StudentDisciplineSummary(DemoSchoolConstants.Student1, DemoSchoolConstants.FirstMarkingPeriodId), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<DisciplineTypeController>(x => x.List(null, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<FeedController>(x => x.List(null, null, null, DemoSchoolConstants.AlgebraClassId, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<GradingController>(x => x.ClassSummary(DemoSchoolConstants.AlgebraClassId), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<GradingController>(x => x.ItemGradingStat(1), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<GradingController>(x => x.ClassSummaryGrids(DemoSchoolConstants.AlgebraClassId), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<GradingController>(x => x.SetAutoGrade(1, DemoSchoolConstants.Student1, "A"), ApiMethodCallType.Post);
            RegisterApiMethodDefaults<PeriodController>(x => x.List(DemoSchoolConstants.CurrentSchoolYearId), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<MarkingPeriodController>(x => x.List(DemoSchoolConstants.CurrentSchoolYearId, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<PersonController>(x => x.Me(),ApiMethodCallType.Get);
            RegisterApiMethodDefaults<StudentController>(x => x.Info(DemoSchoolConstants.Student1), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<StudentController>(x => x.GetStudents("", null, null, null, DemoSchoolConstants.AlgebraClassId, null, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<StudentController>(x => x.Summary(DemoSchoolConstants.Student1), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<TeacherController>(x => x.Summary(DemoSchoolConstants.TeacherId), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<TeacherController>(x => x.GetTeachers("", null, null, DemoSchoolConstants.AlgebraClassId, null, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<PrivateMessageController>(x => x.Send(DemoSchoolConstants.Student2, null, "test msg", "test msg body"), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<StudentController>(x => x.Schedule(DemoSchoolConstants.Student1), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<SearchController>(x => x.Search("algebra"), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<SchoolYearController>(x => x.List(null, null, null), ApiMethodCallType.Get);
            RegisterApiMethodDefaults<SchoolYearController>(x => x.CurrentSchoolYear(), ApiMethodCallType.Get);

        }

        private static void RegisterApiMethodDefaults<T>(Expression<Action<T>> expr, ApiMethodCallType callType) where T: ChalkableController
        {
            var body = expr.Body as MethodCallExpression;
            var arguments = body.Arguments;
            
            var controllerName = expr.Parameters[0].Type.ToString()
                .Split('.')
                .First(x => x.EndsWith(CONTROLLER))
                .Replace(CONTROLLER, "");

            var methodName = body.Method.Name;

            var argValues = new List<String>();
            foreach (var arg in arguments)
            {
                var expValue = "";
                var exp = arg as UnaryExpression;
                if (exp != null)
                {
                    expValue = exp.Operand.ToString();
                }
                else
                {
                    var constValue = arg as ConstantExpression;
                    if (constValue != null && constValue.Value != null)
                        expValue = constValue.Value.ToString();
                }
                if (expValue == "DateTime.Today") expValue = DateTime.Today.ToString(CultureInfo.InvariantCulture);
                argValues.Add(expValue);
            }

            var controllerDescriptionViewData = controllerList[controllerName];

            
            
            foreach (var method in controllerDescriptionViewData.Methods)
            {
                if (method.Name != methodName) continue;
                var i = 0;
                method.Method = callType.ToString();
                foreach (var param in method.Parameters)
                {
                    if (argValues.Count > 0)
                    {
                        param.Value = argValues[i];
                    }
                    ++i;
                }
            }
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
            var chalkableApiControllerDescriptions = controllers.Select(x => x.Value).ToList();

            
            BuildFakeResponses(roles);

            descriptions = new Dictionary<string, IList<ChalkableApiControllerDescription>>();

            
            foreach (var role in roles)
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

        public static IDictionary<string, ChalkableApiControllerDescription> BuildControllerList()
        {
            if (controllerList != null)
                return controllerList;

            var asm =
                BuildManager.GetReferencedAssemblies()
                    .Cast<Assembly>()
                    .Single(assembly => assembly.GetName().Name == ASSEMBLY_FILE);
            var controllers = asm.GetExportedTypes().Where(IsChalkableController);

            roles = new HashSet<string>();
            controllerList = new Dictionary<string, ChalkableApiControllerDescription>();

            foreach (var controller in controllers)
            {
                var controllerName = controller.Name.Replace(CONTROLLER, "");
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                var noApiMethods = true;

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
                            roles.Add(role);
                    }

                    var methodKey = BuildMethodDescriptionKey(controllerName, method.Name);

                    mList.Add(new ChalkableApiMethodDescription
                    {
                        Name = method.Name,
                        Description = methodKey,
                        Method = ApiMethodCallType.Get.ToString(),
                        Parameters = paramsList,
                        AvailableForRoles = avRoles
                    });
                    noApiMethods = false;
                }

                if (!noApiMethods)
                {
                    controllerList.Add(controllerName, new ChalkableApiControllerDescription
                    {
                        Name = controllerName,
                        Methods = mList
                    });    
                }
            }

            PrepareControllerMap();

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

        public static IList<string> GenerateControllerDescriptionKeys()
        {
            var controllers = BuildControllerList();
            IList<string> keys = new List<string>();
            foreach (var controller in controllers.Values)
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