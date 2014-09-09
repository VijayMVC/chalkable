using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chalkable.Web.Extensions
{
    public static class ViewDataExtension
    {
        public static object GetValueOrNull<TModel>(this ViewDataDictionary<TModel> viewData, string key)
        {
            return viewData.ContainsKey(key) ? viewData[key] : null;
        }
    }
}