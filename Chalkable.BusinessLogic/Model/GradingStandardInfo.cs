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
                    AlphaGradeId = standardScore.EnteredScoreAlphaGradeId ?? standardScore.ComputedScoreAlphaGradeId,
                    AlphaGradeName = standardScore.EnteredScoreAlphaGradeName ?? standardScore.ComputedScoreAlphaGradeName,
                    NumericGrade = standardScore.ComputedScore,
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
                res.Add(Create(standardScore, standard));
            }
            return res;

        }
    }

    public class AlphaGradeNameComperator : IComparer<string>
    {
        private static string[] defaultAlphaGrades = new []
            {"A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D+", "D", "D-", "F"};

        public int Compare(string x, string y)
        {
            if (!string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y))
            {
                var xIndex = Array.IndexOf(defaultAlphaGrades, x);
                var yIndex = Array.IndexOf(defaultAlphaGrades, y);

                if (xIndex  < 0 &&  yIndex < 0) 
                    return String.Compare(x, y, StringComparison.Ordinal);

                if (xIndex >= 0 && xIndex < yIndex || yIndex < 0) return 1;
                if (yIndex >= 0 && xIndex > yIndex || xIndex < 0) return -1; 
              
            }
            return String.Compare(x, y, StringComparison.Ordinal);
        }
    }
}
