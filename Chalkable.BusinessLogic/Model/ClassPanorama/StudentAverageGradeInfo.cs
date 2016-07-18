using Chalkable.StiConnector.Connectors.Model.SectionPanorama;

namespace Chalkable.BusinessLogic.Model.ClassPanorama
{
    public class StudentAverageGradeInfo
    {
        public decimal AverageGrade { get; set; }
        public int StudentId { get; set; }

        public static StudentAverageGradeInfo Create(StudentGrade model)
        {
            return new StudentAverageGradeInfo
            {
                StudentId = model.StudentId,
                AverageGrade = model.AverageGrade
            };
        }
    }
}
