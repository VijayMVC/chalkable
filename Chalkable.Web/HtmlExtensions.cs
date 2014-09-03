using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Mvc;

namespace Chalkable.Web
{
    public static class HtmlExtensions
    {
        
        public static RouteValueDictionary ParseExpression<T>(Expression<Action<T>> action) where T : Controller
        {
            var body = action.Body as MethodCallExpression;
            if (body == null)
            {
                throw new InvalidOperationException("Expression must be a method call");
            }
            if (body.Object != action.Parameters[0])
            {
                throw new InvalidOperationException("Method call must target lambda argument");
            }
            var name = body.Method.Name;
            var str2 = typeof(T).Name;
            if (str2.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
            {
                str2 = str2.Remove(str2.Length - 10, 10);
            }

            var values = new RouteValueDictionary
            {
                {"controller", str2}, 
                {"action", name}
            };

            if (body.Method.GetParameters().Length > 0)
            {
                var methodParams = LinkBuilder.BuildParameterValuesFromExpression(body) ?? new RouteValueDictionary();
                foreach (var methodParam in methodParams)
                {
                    values.Add(methodParam.Key, methodParam.Value);
                }
            }
            return values;
        }
    }

}