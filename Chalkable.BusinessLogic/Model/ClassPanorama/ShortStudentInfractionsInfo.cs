using Chalkable.StiConnector.Connectors.Model.SectionPanorama;

namespace Chalkable.BusinessLogic.Model.ClassPanorama
{
    public class ShortStudentInfractionsInfo
    {
        public int StudentId { get; set; }
        public int NumberOfInfractions { get; set; }

        public static ShortStudentInfractionsInfo Create(StudentInfraction model)
        {
            return new ShortStudentInfractionsInfo
            {
                StudentId = model.StudentId,
                NumberOfInfractions = model.NumberOfInfractions
            };
        }      
    }
}
