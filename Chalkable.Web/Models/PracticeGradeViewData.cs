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
        public IList<AnnouncementStandardViewData> Standrds { get; set; }
        public IList<PracticeGradeViewData> PracticeGrades { get; set; }
 
        public static PracticeGradeGridViewData Create(IList<PracticeGrade> practiceGrades, IList<Standard> allStandards
                                                       ,IList<GradingStandardInfo> gradingStandardInfos, IList<Standard> selectedStandards)
        {
            var res = new PracticeGradeGridViewData
                {
                    Standrds = AnnouncementStandardViewData.Create(allStandards),
                    PracticeGrades = new List<PracticeGradeViewData>()
                };
            foreach (var standard in selectedStandards)
            {
                var standardScore = gradingStandardInfos.FirstOrDefault(ss => ss.Standard.Id == standard.Id);
                var pGrades = practiceGrades.Where(x => x.StandardId == standard.Id).ToList();
                if (pGrades.Count > 0)
                {
                    foreach (var practiceGrade in pGrades)
                        res.PracticeGrades.Add(PracticeGradeViewData.Create(practiceGrade, standardScore, standard));
                }
                else
                    res.PracticeGrades.Add(PracticeGradeViewData.Create(null, standardScore, standard));

            }
            return res;
        }
    }

    public class PracticeGradeViewData
    {
        public AnnouncementStandardViewData Standard { get; set; }
        public string PracticeScore { get; set; }
        public string GradeBookScore { get; set; }
        public DateTime? GradedDate { get; set; }
        public Guid? ApplicationId { get; set; }

        public static PracticeGradeViewData Create(PracticeGrade practiceGrade, GradingStandardInfo gradingStandardInfo, Standard standard)
        {
            var res = new PracticeGradeViewData
                {
                    Standard = AnnouncementStandardViewData.Create(standard)
                };
            if (practiceGrade != null)
            {
                res.PracticeScore = practiceGrade.Score;
                res.GradedDate = practiceGrade.Date;
                res.ApplicationId = practiceGrade.ApplicationRef;
            }
            if (gradingStandardInfo != null && gradingStandardInfo.NumericGrade.HasValue)
                res.GradeBookScore = gradingStandardInfo.NumericGrade.Value.ToString();
            return res;
        }

    }
}