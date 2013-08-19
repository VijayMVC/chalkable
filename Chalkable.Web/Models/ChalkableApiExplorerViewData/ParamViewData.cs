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
        public ParamType ParamType { get; set; }

        public static ParamViewData Create(Param param)
        {
            return new ParamViewData
            {
                Name = param.Name,
                Value = param.Value,
                Description = param.Description,
                IsNullable = param.IsNullable,
                ParamType = param.ParamType
            };
        }
    }

}