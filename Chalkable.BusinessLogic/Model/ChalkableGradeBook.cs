using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class ChalkableGradeBook
    {
        public GradingPeriod GradingPeriod { get; set; }
        public int Avg { get; set; }
        public IList<Person> Students { get; set; }
        public IList<AnnouncementDetails> Announcements { get; set; } 
        public IList<ChalkableStudentAverage> Averages { get; set; }
        public ChalkableClassOptions Options { get; set; }
    }

    public class ChalkableStudentAverage
    {
        public int AverageId { get; set; }
        public string AverageName { get; set; }
        public decimal? AvgValue { get; set; }
        public int StudentId { get; set; }
        public AlphaGrade AlphaGrade { get; set; }
        public bool IsGradingPeriodAverage { get; set; }

        public static ChalkableStudentAverage Create(StudentAverages studentAverages)
        {
            var res = new ChalkableStudentAverage
                {
                    AverageId = studentAverages.AverageId,
                    AverageName = studentAverages.AverageName,
                    AvgValue = studentAverages.Score,
                    StudentId = studentAverages.StudentId,
                    IsGradingPeriodAverage = studentAverages.IsGradingPeriodAverage
                };
            if (studentAverages.AlphaGradeId.HasValue)
            {
                res.AlphaGrade = new AlphaGrade
                    {
                        Id = studentAverages.AlphaGradeId.Value,
                        Name = studentAverages.AlphaGradeName
                    };
            }
            return res;
        }
    }

    public class ChalkableClassOptions
    {
        public bool DisplayAlphaGrades { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }

        public static ChalkableClassOptions Create(ClassroomOption classroomOption)
        {
            return new ChalkableClassOptions
                {
                    DisplayAlphaGrades = classroomOption.DisplayAlphaGrades,
                    DisplayStudentAverage = classroomOption.DisplayStudentAverage,
                    DisplayTotalPoints = classroomOption.DisplayTotalPoints,
                    IncludeWithdrawnStudents = classroomOption.IncludeWithdrawnStudents
                };
        }
    }

    public class ClassGradingSummary
    {
        public double? Avg { get; set; }
        public IList<AnnouncementDetails> Announcements { get; set; }
        public IList<GradedClassAnnouncementType> AnnouncementTypes { get; set; }
        public GradingPeriod GradingPeriod { get; set; }
    }
}
