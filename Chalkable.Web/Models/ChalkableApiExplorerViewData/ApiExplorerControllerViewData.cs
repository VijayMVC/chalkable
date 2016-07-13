using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models.ChalkableApiExplorerViewData
{
    public class ApiExplorerControllerViewData
    {
        public string ControllerName { get; set; }
        public IList<ApiExplorerMethodViewData> Methods { get; set; }

        public static ApiExplorerControllerViewData Create(ChalkableApiControllerDescription descr)
        {
            return new ApiExplorerControllerViewData
            {
                ControllerName = descr.Name,
                Methods = ApiExplorerMethodViewData.Create(descr.Methods)
            };
        }
    }
}