using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Web.Models
{
    public class CommonCoreStandardViewData
    {
        public Guid Id { get; set; }
        public Guid? ParentStandardId { get; set; }
        public string StandardCode { get; set; }
        public string Description { get; set; }
        public Guid StandardCategoryId { get; set; }

        public static CommonCoreStandardViewData Create(CommonCoreStandard commonCoreStandard)
        {
            return new CommonCoreStandardViewData
                {
                    Id = commonCoreStandard.Id,
                    ParentStandardId = commonCoreStandard.ParentStandardRef,
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