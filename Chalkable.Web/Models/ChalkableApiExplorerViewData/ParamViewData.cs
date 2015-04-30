using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models.ChalkableApiExplorerViewData
{
    public class ParamViewData
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsNullable { get; set; }
        public ApiMethodParamType ParamType { get; set; }

        public static ParamViewData Create(ApiMethodParam apiMethodParam)
        {
            return new ParamViewData
            {
                Name = apiMethodParam.Name,
                Value = apiMethodParam.Value,
                Description = apiMethodParam.Description,
                IsNullable = apiMethodParam.IsNullable,
                ParamType = apiMethodParam.ParamType
            };
        }
    }

}