using Chalkable.AcademicBenchmarkConnector.Models;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard
{
    public class AcademicBenchmarkStandard : AcademicBenchmarkShortStandard
    {
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }

        public AcademicBenchmarkStandard(Standard standard) : base(standard)
        {
            if (standard.Authority != null)
                Authority = new AcademicBenchmarkAuthority
                {
                    Id = standard.Authority.Id,
                    Code = standard.Authority.Code,
                    Description = standard.Authority.Description
                };
            if (standard.Document != null)
                Document = new AcademicBenchmarkDocument
                {
                    Id = standard.Document.Id,
                    Code = standard.Document.Code,
                    Description = standard.Document.Title,
                };
        }
        public static AcademicBenchmarkStandard Create(Standard standard)
        {
            return new AcademicBenchmarkStandard(standard);
        }
    }
    
}
