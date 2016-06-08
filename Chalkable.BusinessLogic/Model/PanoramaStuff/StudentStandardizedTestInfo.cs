using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model.PanoramaStuff
{
    public class StudentStandardizedTestInfo
    {
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public string Score { get; set; }
        public int StandardizedTestComponentId { get; set; }
        public int StandardizedTestId { get; set; }
        public int StandardizedTestScoreTypeId { get; set; }

        public static StudentStandardizedTestInfo Create(StudentStandardizedTest model)
        {
            return new StudentStandardizedTestInfo
            {
                StudentId = model.StudentId,
                Date = model.Date,
                StandardizedTestId = model.StandardizedTestId,
                StandardizedTestScoreTypeId = model.StandardizedTestScoreTypeId,
                Score = model.Score,
                StandardizedTestComponentId = model.StandardizedTestComponentId
            };
        }
    }
}
