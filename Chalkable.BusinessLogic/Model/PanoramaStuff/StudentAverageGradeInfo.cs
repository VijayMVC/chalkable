using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model.PanoramaStuff
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
