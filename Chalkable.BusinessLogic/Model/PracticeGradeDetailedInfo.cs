using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Logic.Comperators;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class PracticeGradesDetailedInfo : IComparable<PracticeGradesDetailedInfo>
    {
        public IList<PracticeGrade> PracticeGrades { get; set; }
        public Standard Standard { get; set; }
        public GradingStandardInfo GradingStandardInfo { get; set; }
    
        public static PracticeGradesDetailedInfo Create(IList<PracticeGrade> practiceGrades, Standard standard,
                                                       StandardScore standardScore)
        {
            var res = new PracticeGradesDetailedInfo
                {
                    PracticeGrades = practiceGrades,
                    Standard = standard,
                };
            if (standardScore != null)
                res.GradingStandardInfo = GradingStandardInfo.Create(standardScore, standard);
            return res;
        }

        public int CompareTo(PracticeGradesDetailedInfo other)
        {
            if (other == null) return 1;
            if (other == this) return 0;

            string pgScore1 = int.MaxValue.ToString(), 
                   pgScore2 = int.MaxValue.ToString();

            if (PracticeGrades != null && PracticeGrades.Count > 0) 
                pgScore1 = PracticeGrades.First().Score;
            if (other.PracticeGrades != null && other.PracticeGrades.Count > 0)
                pgScore2 = other.PracticeGrades.First().Score;

            var res = PracticeGradeScoreComperator.ComparePracticeScore(pgScore1, pgScore2);
            if (res == 0)
            {
                bool a = !GradingStandardInfo.NumericGrade.HasValue;
                bool b = !other.GradingStandardInfo.NumericGrade.HasValue;
                
                if (GradingStandardInfo == null || other.GradingStandardInfo == null)
                {
                    if (GradingStandardInfo == null && other.GradingStandardInfo == null)  return 0;
                    return GradingStandardInfo == null ? 1 : -1;
                }

                if (a && b)
                {
                    if (!GradingStandardInfo.HasAlphaGrade && !GradingStandardInfo.HasAlphaGrade) return 0;
                    if (!GradingStandardInfo.HasAlphaGrade) return 1;
                    if (!other.GradingStandardInfo.HasAlphaGrade) return -1;

                    return AlphaGradeNameComperator.CompareAlphaGrade(GradingStandardInfo.AlphaGradeName,
                                                                      other.GradingStandardInfo.AlphaGradeName);
                }
                if (a) return 1;
                if (b) return -1;

               return GradingStandardInfo.NumericGrade.Value.CompareTo(other.GradingStandardInfo.NumericGrade.Value);
            }
            return res;
        }
    }
}
