using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models.ChalkableApiExplorerViewData
{
    public class ApiExplorerMethodViewData
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public IList<ParamViewData> Params { get; set; }

        public static ApiExplorerMethodViewData Create(ChalkableApiMethodDescription method)
        {
            return new ApiExplorerMethodViewData
            {
                Name = method.Name,
                Params = method.Parameters.Select(ParamViewData.Create).ToList(),
                Method = method.Method,
                Description = method.Description
            };
        }
        public static IList<ApiExplorerMethodViewData> Create(IList<ChalkableApiMethodDescription> methods)
        {
            return methods.Select(Create).ToList();
        }
    }
}