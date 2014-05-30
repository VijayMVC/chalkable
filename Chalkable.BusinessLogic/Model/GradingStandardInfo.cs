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

        public static GradingStandardInfo Create(StandardScore standardScore, Standard standard)
        {
            return new GradingStandardInfo
                {
                    Standard = standard,
                    StudentId = standardScore.StudentId,
                    ClassId = standardScore.SectionId,
                    AlphaGradeId = standardScore.ComputedScoreAlphaGradeId ?? standardScore.EnteredScoreAlphaGradeId,
                    AlphaGradeName = standardScore.ComputedScoreAlphaGradeName ?? standardScore.EnteredScoreAlphaGradeName,
                    NumericGrade = standardScore.ComputedScore,
                    GradingPeriodId = standardScore.GradingPeriodId,
                    Note = standardScore.Note
                };
        }
    }
}
