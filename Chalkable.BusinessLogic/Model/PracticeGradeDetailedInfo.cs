using System;
using Chalkable.BusinessLogic.Logic.Comperators;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class PracticeGradeDetailedInfo : IComparable<PracticeGradeDetailedInfo>
    {
        public PracticeGrade PracticeGrade { get; set; }
        public Standard Standard { get; set; }
        public GradingStandardInfo GradingStandardInfo { get; set; }
    
        public static PracticeGradeDetailedInfo Create(PracticeGrade practiceGrade, Standard standard,
                                                       StandardScore standardScore)
        {
            var res = new PracticeGradeDetailedInfo
                {
                    PracticeGrade = practiceGrade,
                    Standard = standard,
                };
            if (standardScore != null)
                res.GradingStandardInfo = GradingStandardInfo.Create(standardScore, standard);
            return res;
        }

        public int CompareTo(PracticeGradeDetailedInfo other)
        {
            if (other == null) return 1;
            if (other == this) return 0;

            string pgScore1 = int.MaxValue.ToString(), 
                   pgScore2 = int.MaxValue.ToString();

            if (PracticeGrade != null) pgScore1 = PracticeGrade.Score;
            if (other.PracticeGrade != null) pgScore2 = other.PracticeGrade.Score;

            var res = PracticeGradeScoreComperator.ComparePracticeScore(pgScore1, pgScore2);
            if (res == 0)
            {
                bool a = GradingStandardInfo == null || string.IsNullOrEmpty(GradingStandardInfo.AlphaGradeName);
                bool b = other.GradingStandardInfo == null || string.IsNullOrEmpty(other.GradingStandardInfo.AlphaGradeName);
                
                if (a && b) return 0;
                if (a) return 1;
                if (b) return -1;

                return AlphaGradeNameComperator.CompareAlphaGrade(GradingStandardInfo.AlphaGradeName,
                                                                  other.GradingStandardInfo.AlphaGradeName);
            }
            return res;
        }
    }
}
