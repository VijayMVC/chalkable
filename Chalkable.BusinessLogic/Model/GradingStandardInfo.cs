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
        public bool HasAlphaGrade => !string.IsNullOrEmpty(AlphaGradeName);
        public bool HasScore => HasAlphaGrade || NumericGrade.HasValue;

        public static GradingStandardInfo Create(StandardScore standardScore, Standard standard)
        {
            return new GradingStandardInfo
                {
                    Standard = standard,
                    StudentId = standardScore.StudentId,
                    ClassId = standardScore.SectionId,
                    AlphaGradeId = standardScore.EnteredScoreAlphaGradeId ?? standardScore.ComputedScoreAlphaGradeId,
                    AlphaGradeName = standardScore.EnteredScoreAlphaGradeName ?? standardScore.ComputedScoreAlphaGradeName,
                    NumericGrade = standardScore.EnteredScoreAveragingEquivalent ?? standardScore.ComputedScore,
                    GradingPeriodId = standardScore.GradingPeriodId,
                    Note = standardScore.Note
                };
        }
        

        public static IList<GradingStandardInfo> Create(IList<StandardScore> standardScores, IList<Standard> standards)
        {
            var res = new List<GradingStandardInfo>();
            foreach (var standardScore in standardScores)
            {
                var standard = standards.FirstOrDefault(st => st.Id == standardScore.StandardId);
                if(standard!=null)
                    res.Add(Create(standardScore, standard));
            }
            return res;

        }
    }
}
