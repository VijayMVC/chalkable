using System;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static Document Create(AcademicBenchmarkConnector.Models.Document doc)
        {
            return new Document
            {
                Id = doc.Id,
                Code = doc.Code,
                Description = doc.Title
            };
        }
    }
}
