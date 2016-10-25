using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model.SectionPanorama;

namespace Chalkable.BusinessLogic.Model.ClassPanorama
{
    public class ClassPanorama
    {
        public IList<ShortStudentAbsenceInfo> Absences { get; set; }
        public IList<StudentAverageGradeInfo> Grades { get; set; }
        public IList<ShortStudentInfractionsInfo> Infractions { get; set; }
        public IList<StudentStandardizedTestInfo> StandardizedTests { get; set; }

        public static ClassPanorama Create(SectionPanorama model)
        {
            var classPanorama = new ClassPanorama();
            if (model.Absences != null)
                classPanorama.Absences = model.Absences.Select(ShortStudentAbsenceInfo.Create).ToList();

            if (model.Grades != null)
                classPanorama.Grades = model.Grades.Select(StudentAverageGradeInfo.Create).ToList();

            if (model.Infractions != null)
                classPanorama.Infractions = model.Infractions.Select(ShortStudentInfractionsInfo.Create).ToList();

            if (model.StandardizedTests != null)
                classPanorama.StandardizedTests = model.StandardizedTests.Select(StudentStandardizedTestInfo.Create).ToList();

            return classPanorama;
        }
    }
}
