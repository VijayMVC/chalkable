namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class Subject
    {
        public string Code { get; set; }
        public string Broad { get; set; }
        public string Description { get; set; }
        public static Subject Create(AcademicBenchmarkConnector.Models.Subject sub)
        {
            return new Subject
            {
                Broad = sub.Broad,
                Description = sub.Description,
                Code = sub.Code
            };
        }
    }
}
