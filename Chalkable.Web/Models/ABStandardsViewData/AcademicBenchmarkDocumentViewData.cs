using System;
using Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard;

namespace Chalkable.Web.Models.ABStandardsViewData
{
    public class AcademicBenchmarkDocumentViewData
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static AcademicBenchmarkDocumentViewData Create(AcademicBenchmarkDocument document)
        {
            return new AcademicBenchmarkDocumentViewData
            {
                Id = document.Id,
                Code = document.Code,
                Description = document.Description
            };
        }
    }
}