using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class StandardGradingGridViewData
    {
        public int? Avg { get; set; }
        public IList<GradeStudentViewData> Students { get; set; }
        public MarkingPeriodViewData MarkingPeriod { get; set; }
        public IList<StandardGradingViewData> GradingItems { get; set; } 

        public static StandardGradingGridViewData Create(MarkingPeriod markingPeriod, IList<GradingStandardInfo> gradingStandardInfos)
        {
            var res = new StandardGradingGridViewData {MarkingPeriod = MarkingPeriodViewData.Create(markingPeriod)};
            var students = gradingStandardInfos.GroupBy(x => x.Student).Select(x => x.Key).ToList();
            res.Students = students.Select(GradeStudentViewData.Create).ToList();
            res.GradingItems = StandardGradingViewData.Create(gradingStandardInfos);
            if (res.GradingItems.Count > 0)
                res.Avg = (int?)res.GradingItems.Average(x => x.NumericAvg);
            return res;
        }
    }

    public class StandardGradingViewData
    {
        public IList<StandardGradingItemViewData> Items { get; set; }
        public int? NumericAvg { get; set; }
        public string AlphaGradeNameAvg { get; set; }
        public AnnouncementStandardViewData Standard { get; set; }

        public static IList<StandardGradingViewData> Create(IList<GradingStandardInfo> gradingStandards)
        {
            var gradigDic = gradingStandards.GroupBy(x => x.Standard.Id).ToDictionary(x => x.Key, x => x.ToList());
            var res = new List<StandardGradingViewData>();
            foreach (var kv in gradigDic)
            {
                var gradingSt = kv.Value.First();
                res.Add(new StandardGradingViewData
                    {
                        Standard = AnnouncementStandardViewData.Create(gradingSt.Standard),
                        NumericAvg = (int?)kv.Value.Average(x => x.NumericGrade),
                        Items = kv.Value.Select(StandardGradingItemViewData.Create).ToList()
                    });
            }
            return res;
        }
    }

    public class StandardGradingItemViewData
    {
        public int StudentId { get; set; }
        public int StandardId { get; set; }
        public int? GradeId { get; set; }
        public string GradeValue { get; set; }
        public int GradingPeriodId { get; set; }

        public static StandardGradingItemViewData Create(GradingStandardInfo gradingStandard)
        {
            return new StandardGradingItemViewData
                {
                    StudentId = gradingStandard.StudentId,
                    GradeId = gradingStandard.AlphaGradeId,
                    GradeValue = gradingStandard.AlphaGradeName,
                    StandardId = gradingStandard.Standard.Id,
                    GradingPeriodId = gradingStandard.GradingPeriodId
                };
        }
    }
}
