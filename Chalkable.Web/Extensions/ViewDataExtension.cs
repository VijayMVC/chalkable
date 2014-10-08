using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Chalkable.Web.Extensions
{
    public static class ViewDataExtension
    {
        public static object GetJsConstantValueOrNull<TModel>(this ViewDataDictionary<TModel> viewData, string key)
        {
            return new HtmlString(viewData.ContainsKey(key) ? JsonConvert.SerializeObject(viewData[key]) : "null");
        }

        public static object GetJsJsonObjectOrNull<TModel>(this ViewDataDictionary<TModel> viewData, string key)
        {
            return new HtmlString(viewData.ContainsKey(key) ? viewData[key].ToString() : "null");
        }
    }
}