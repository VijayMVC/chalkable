using System;
using Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard;

namespace Chalkable.Web.Models.ABStandardsViewData
{
    public class AcademicBenchmarkStandardViewData
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public AcademicBenchmarkAuthorityViewData Authority { get; set; }
        public AcademicBenchmarkDocumentViewData Document { get; set; }

        public static AcademicBenchmarkStandardViewData Create(AcademicBenchmarkStandard academicBenchmarkStandard)
        {
            return new AcademicBenchmarkStandardViewData
            {
                Id = academicBenchmarkStandard.Id,
                Code = academicBenchmarkStandard.Code,
                Description = academicBenchmarkStandard.Description,
                IsDeepest = academicBenchmarkStandard.IsDeepest,
                ParentId = academicBenchmarkStandard.ParentId,
                Level = academicBenchmarkStandard.Level,
                IsActive = academicBenchmarkStandard.IsActive,
                Authority = AcademicBenchmarkAuthorityViewData.Create(academicBenchmarkStandard.Authority),
                Document = AcademicBenchmarkDocumentViewData.Create(academicBenchmarkStandard.Document)
            };
        }
    }
}