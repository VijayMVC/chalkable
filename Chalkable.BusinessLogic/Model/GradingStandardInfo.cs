using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class GradingStandardInfo
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int GradingPeriodId { get; set; }
        public int? AlphaGradeId { get; set; }
        public string AlphaGradeName { get; set; }
        public decimal? NumericGrade { get; set; }
        public string Note { get; set; }
        public Standard Standard { get; set; }
        public Person Student { get; set; }
        
        public static GradingStandardInfo Create(StandardScore standardScore, Standard standard, Person student)
        {
            return new GradingStandardInfo
                {
                    Student = student,
                    Standard = standard,
                    StudentId = standardScore.StudentId,
                    ClassId = standardScore.SectionId,
                    AlphaGradeId = standardScore.ComputedScoreAlphaGradeId,
                    AlphaGradeName = standardScore.ComputedScoreAlphaGradeName,
                    NumericGrade = standardScore.ComputedScore,
                    GradingPeriodId = standardScore.GradingPeriodId,
                    Note = standardScore.Note
                };
        }
    }
}
