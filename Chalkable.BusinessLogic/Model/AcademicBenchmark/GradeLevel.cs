namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class GradeLevel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Low { get; set; }
        public string Height { get; set; }
        public static GradeLevel Create(AcademicBenchmarkConnector.Models.GradeLevel grLevel)
        {
            return new GradeLevel
            {
                Description = grLevel.Description,
                Code = grLevel.Code,
                Height = grLevel.Height,
                Low = grLevel.Low
            };
        }
    }
}
