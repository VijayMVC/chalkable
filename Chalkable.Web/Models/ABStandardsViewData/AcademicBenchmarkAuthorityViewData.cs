using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard;

namespace Chalkable.Web.Models.ABStandardsViewData
{
    public class AcademicBenchmarkAuthorityViewData
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static AcademicBenchmarkAuthorityViewData Create(AcademicBenchmarkAuthority authority)
        {
            return new AcademicBenchmarkAuthorityViewData
            {
                Id = authority.Id,
                Code = authority.Code,
                Description = authority.Description
            };
        }
    }
}