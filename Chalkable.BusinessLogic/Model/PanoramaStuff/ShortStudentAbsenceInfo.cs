using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model.PanoramaStuff
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
