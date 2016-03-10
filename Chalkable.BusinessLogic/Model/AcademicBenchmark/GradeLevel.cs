using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class GradeLevel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Low { get; set; }
        public string Hight { get; set; }
        public static GradeLevel Create(AcademicBenchmarkConnector.Models.GradeLevel grLevel)
        {
            return new GradeLevel
            {
                Description = grLevel.Description,
                Code = grLevel.Code,
                Hight = grLevel.Hight,
                Low = grLevel.Low
            };
        }
    }
}
