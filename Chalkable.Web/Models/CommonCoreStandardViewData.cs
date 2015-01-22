using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class CommonCoreStandardViewData
    {
        public string StandardCode { get; set; }
        public string Description { get; set; }
        public Guid StandardCategoryId { get; set; }

        public static CommonCoreStandardViewData Create(CommonCoreStandard commonCoreStandard)
        {
            return new CommonCoreStandardViewData
                {
                    Description = commonCoreStandard.Description,
                    StandardCode = commonCoreStandard.Code,
                    StandardCategoryId = commonCoreStandard.StandardCategoryRef
                };
        }

        public static IList<CommonCoreStandardViewData> Create(IList<CommonCoreStandard> commonCoreStandards)
        {
            return commonCoreStandards.Select(Create).ToList();
        } 
    }
}