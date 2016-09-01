namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class Standard : ShortStandard
    {
        public Authority Authority { get; set; }
        public Document Document { get; set; }

        protected Standard(AcademicBenchmarkConnector.Models.Standard standard) : base(standard)
        {
            if (standard.Authority != null)
                Authority = new Authority
                {
                    Id = standard.Authority.Id,
                    Code = standard.Authority.Code,
                    Description = standard.Authority.Description
                };
            if (standard.Document != null)
                Document = new Document
                {
                    Id = standard.Document.Id,
                    Code = standard.Document.Code,
                    Description = standard.Document.Title,
                };
        }
        public static Standard Create(AcademicBenchmarkConnector.Models.Standard standard)
        {
            return new Standard(standard);
        }
    }
    
}
