using Chalkable.StiConnector.Connectors.Model.SectionPanorama;

namespace Chalkable.BusinessLogic.Model.ClassPanorama
{
    public class ShortStudentAbsenceInfo
    {
        public decimal NumberOfDaysEnrolled { get; set; }
        public decimal NumberOfAbsences { get; set; }
        public int StudentId { get; set; }

        public static ShortStudentAbsenceInfo Create(StudentAbsence model)
        {
            return new ShortStudentAbsenceInfo
            {
                StudentId = model.StudentId,
                NumberOfAbsences = model.NumberOfAbsences,
                NumberOfDaysEnrolled = model.NumberOfDaysEnrolled
            };
        }
    }
}
