using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.ClassPanorama;
using SisStudentPanorama = Chalkable.StiConnector.Connectors.Model.StudentPanorama.StudentPanorama;

namespace Chalkable.BusinessLogic.Model.StudentPanorama
{
    public class StudentPanoramaInfo
    {
        public IList<StudentAbsenceInfo> DailyAbsences { get; set; }
        public IList<StudentInfractionInfo> Infractions { get; set; }
        public IList<StudentStandardizedTestInfo> StandardizedTests { get; set; }

        public static StudentPanoramaInfo Create(SisStudentPanorama model)
        {
            var res = new StudentPanoramaInfo();

            if (model.DailyAbsences != null)
                res.DailyAbsences = StudentAbsenceInfo.Create(model.DailyAbsences.ToList(), model.PeriodAbsences?.ToList());

            if (model.Infractions != null)
                res.Infractions = model.Infractions.Select(StudentInfractionInfo.Create).ToList();

            if (model.StandardizedTests != null)
                res.StandardizedTests = model.StandardizedTests.Select(StudentStandardizedTestInfo.Create).ToList();

            return res;
        }
    }
}
