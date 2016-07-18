using System;
using Chalkable.StiConnector.Connectors.Model.SectionPanorama;

namespace Chalkable.BusinessLogic.Model.ClassPanorama
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
