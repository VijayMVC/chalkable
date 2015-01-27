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
 
        public static PracticeGradeGridViewData Create(IList<PracticeGrade> practiceGrades, IList<Standard> standards
                                                       , IList<GradingStandardInfo> gradingStandardInfos)
        {
            var res = new PracticeGradeGridViewData
                {
                    Standrds = AnnouncementStandardViewData.Create(standards),
                    PracticeGrades = new List<PracticeGradeViewData>()
                };
            foreach (var practiceGrade in practiceGrades)
            {
                var standard = standards.FirstOrDefault(s => s.Id == practiceGrade.StandardId);
                if(standard == null) continue;
                var standardScore = gradingStandardInfos.FirstOrDefault(ss => ss.Standard.Id == practiceGrade.StandardId);
                res.PracticeGrades.Add(PracticeGradeViewData.Create(practiceGrade, standardScore, standard));
            }
            return res;
        }
    }

    public class PracticeGradeViewData
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public AnnouncementStandardViewData Standard { get; set; }
        public string PracticeScore { get; set; }
        public string GradeBookScore { get; set; }
        public DateTime GradedDate { get; set; }
        public Guid ApplicationId { get; set; }

        public static PracticeGradeViewData Create(PracticeGrade practiceGrade, GradingStandardInfo gradingStandardInfo, Standard standard)
        {
            var res = new PracticeGradeViewData
                {
                    Id = practiceGrade.Id,
                    StudentId = practiceGrade.StudentId,
                    PracticeScore = practiceGrade.Score,
                    GradedDate = practiceGrade.Date,
                    ApplicationId = practiceGrade.ApplicationRef,
                    Standard = AnnouncementStandardViewData.Create(standard)
                };
            if (gradingStandardInfo != null && gradingStandardInfo.NumericGrade.HasValue)
                res.GradeBookScore = gradingStandardInfo.NumericGrade.Value.ToString();
            return res;
        }

    }
}