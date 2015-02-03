using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class PracticeGradeDetailedInfo
    {
        public PracticeGrade PracticeGrade { get; set; }
        public StandardDetailsInfo Standard { get; set; }
        public GradingStandardInfo GradingStandardInfo { get; set; }
    
        public static PracticeGradeDetailedInfo Create(PracticeGrade practiceGrade, StandardDetailsInfo standard,
                                                       StandardScore standardScore)
        {
            var res = new PracticeGradeDetailedInfo
                {
                    PracticeGrade = practiceGrade,
                    Standard = standard,
                };
            if (standardScore != null)
                res.GradingStandardInfo = GradingStandardInfo.Create(standardScore, standard.Standard);
            return res;
        }
    }
}
