using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class PracticeGradeDetailedInfo
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
    }
}
