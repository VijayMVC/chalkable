using System.Web;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AjaxHandleErrorAttribute());
        }
    }
}