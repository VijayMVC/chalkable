using Subject = Chalkable.BusinessLogic.Model.AcademicBenchmark.Subject;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class SubjectViewData
    {
        public string Code { get; set; }
        public string Broad { get; set; }
        public string Description { get; set; }
        public static SubjectViewData Create(Subject sub)
        {
            return new SubjectViewData
            {
                Broad = sub.Broad,
                Description = sub.Description,
                Code = sub.Code
            };
        }
    }
}