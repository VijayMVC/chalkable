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
        public decimal? CalculatedAvg { get; set; }
        public decimal? EnteredAvg { get; set; }
        public int StudentId { get; set; }
        public AlphaGrade CalculatedAlphaGrade { get; set; }
        public AlphaGrade EnteredAlphaGrade { get; set; }
        public bool IsGradingPeriodAverage { get; set; }

        public static ChalkableStudentAverage Create(StudentAverage studentAverage)
        {
            var res = new ChalkableStudentAverage
                {
                    AverageId = studentAverage.AverageId,
                    AverageName = studentAverage.AverageName,
                    CalculatedAvg = studentAverage.CalculatedNumericAverage, 
                    EnteredAvg = studentAverage.EnteredNumericAverage,
                    StudentId = studentAverage.StudentId,
                    IsGradingPeriodAverage = studentAverage.IsGradingPeriodAverage
                };
            if (studentAverage.CalculatedAlphaGradeId.HasValue)
                res.CalculatedAlphaGrade = new AlphaGrade
                    {
                        Id = studentAverage.CalculatedAlphaGradeId.Value,
                        Name = studentAverage.CalculatedAlphaGradeName
                    };
            if (studentAverage.EnteredAlphaGradeId.HasValue)
                res.EnteredAlphaGrade = new AlphaGrade
                    {
                        Id = studentAverage.EnteredAlphaGradeId.Value,
                        Name = studentAverage.EnteredAlphaGradeName
                    };
            return res;
        }
    }

    public class ChalkableClassOptions
    {
        public bool DisplayAlphaGrades { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }

        public static ChalkableClassOptions Create(StiConnector.Connectors.Model.ClassroomOption classroomOption)
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
