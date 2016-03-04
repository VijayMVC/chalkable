using Chalkable.AcademicBenchmarkConnector.Models;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard
{
    public class AcademicBenchmarkStandard : AcademicBenchmarkShortStandard
    {
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }

        protected AcademicBenchmarkStandard(Standard standard) : base(standard)
        {
            if (standard.Authority != null)
                Authority = new AcademicBenchmarkAuthority
                {
                    Id = standard.Authority.Id,
                    Description = standard.Authority.Description
                };
            if (standard.Document != null)
                Document = new AcademicBenchmarkDocument
                {
                    Id = standard.Document.Id,
                    Description = standard.Document.Title,
                };
        }
        public static AcademicBenchmarkStandard Create(Standard standard)
        {
            return new AcademicBenchmarkStandard(standard);
        }
    }
    
}
