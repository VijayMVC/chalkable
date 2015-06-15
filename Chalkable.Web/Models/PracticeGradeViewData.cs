using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class PracticeGradeGridViewData
    {
        public IList<StandardViewData> Standrds { get; set; }
        public IList<PracticeGradeViewData> PracticeGrades { get; set; }
 
        public static PracticeGradeGridViewData Create(IList<PracticeGradesDetailedInfo> practiceGrades, IList<Standard> standards)
        {
            var res = new PracticeGradeGridViewData
                {
                    Standrds = StandardViewData.Create(standards),
                    PracticeGrades = practiceGrades.Select(x => PracticeGradeViewData.Create(x.PracticeGrades, x.GradingStandardInfo, x.Standard)).ToList()
                };
            return res;
        }
    }

    public class PracticeGradeViewData
    {
        public StandardViewData Standard { get; set; }
        public string PracticeScore { get; set; }
        public IList<string> PreviousScores { get; set; }
        public string GradeBookScore { get; set; }
        public string GradeBookAlphaGrade { get; set; }
        public DateTime? GradedDate { get; set; }
        public Guid? ApplicationId { get; set; }

        public static PracticeGradeViewData Create(IList<PracticeGrade> practiceGrades, GradingStandardInfo gradingStandardInfo, Standard standard)
        {
            var res = new PracticeGradeViewData {Standard = StandardViewData.Create(standard)};
            if (practiceGrades != null && practiceGrades.Count > 0)
            {
                var lastSetted = practiceGrades.First();
                res.PracticeScore = lastSetted.Score;
                res.GradedDate = lastSetted.Date;
                res.ApplicationId = lastSetted.ApplicationRef;
                res.PreviousScores = practiceGrades.Where(x => x.Id != lastSetted.Id).Select(x => x.Score).ToList();
            }
            if (gradingStandardInfo != null)
            {
                var numericGrade = gradingStandardInfo.NumericGrade;
                res.GradeBookScore = numericGrade.HasValue ? numericGrade.Value.ToString() : null;
                res.GradeBookAlphaGrade = gradingStandardInfo.AlphaGradeName;
            }
            return res;
        }

    }
}