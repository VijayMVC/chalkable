using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web;

namespace Chalkable.Web
{
    public static class HtmlExtensions
    {
        
        public static RouteValueDictionary ParseExpression<T>(Expression<Action<T>> action) where T : Controller
        {
            MethodCallExpression body = action.Body as MethodCallExpression;
            if (body == null)
            {
                throw new InvalidOperationException("Expression must be a method call");
            }
            if (body.Object != action.Parameters[0])
            {
                throw new InvalidOperationException("Method call must target lambda argument");
            }
            string name = body.Method.Name;
            string str2 = typeof(T).Name;
            if (str2.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
            {
                str2 = str2.Remove(str2.Length - 10, 10);
            }
            //TODO: link builder
            //RouteValueDictionary values = LinkBuilder.BuildParameterValuesFromExpression(body) ?? new RouteValueDictionary();
            RouteValueDictionary values =  new RouteValueDictionary();
            values.Add("controller", str2);
            values.Add("action", name);
            return values;
        }
        
    }

}