﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{

    public class StandardGradingGridsViewData
    {
        public StandardGradingGridViewData CurrentStandardGradingGrid { get; set; }
        public IList<GradingPeriodViewData> GradingPeriods { get; set; }

        public static StandardGradingGridsViewData Create(IList<GradingPeriodDetails> gradingPeriods, StandardGradingGridViewData standardGradingGrid)
        {
            return new StandardGradingGridsViewData
                {
                    GradingPeriods = gradingPeriods.Select(GradingPeriodViewData.Create).ToList(),
                    CurrentStandardGradingGrid = standardGradingGrid
                };
        }
    }

    public class StandardGradingGridViewData
    {
        public int? Avg { get; set; }
        public IList<GradeStudentViewData> Students { get; set; }
        public GradingPeriodViewData GradingPeriod { get; set; }
        public IList<StandardGradingViewData> GradingItems { get; set; } 

        public static StandardGradingGridViewData Create(GradingPeriod gradingPeriod, IList<GradingStandardInfo> gradingStandardInfos, IList<StudentDetails> students)
        {
            var res = new StandardGradingGridViewData
                {
                    GradingPeriod = GradingPeriodViewData.Create(gradingPeriod),
                    Students = students.Select(x=>GradeStudentViewData.Create(x, x.IsWithdrawn)).ToList()
                };
            gradingStandardInfos = gradingStandardInfos.Where(x => students.Any(y => y.Id == x.StudentId)
                && gradingPeriod.Id == x.GradingPeriodId).ToList();
            res.GradingItems = StandardGradingViewData.Create(gradingStandardInfos, res.Students.Select(x=>x.StudentInfo.Id).ToList());
            if (res.GradingItems.Count > 0)
                res.Avg = (int?) res.GradingItems.Average(x => x.NumericAvg);
            return res;
        }
    }

    public class StandardGradingViewData
    {
        public IList<StandardGradingItemViewData> Items { get; set; }
        public int? NumericAvg { get; set; }
        public string AlphaGradeNameAvg { get; set; }
        public StandardViewData Standard { get; set; }

        public static IList<StandardGradingViewData> Create(IList<GradingStandardInfo> gradingStandards, IList<int> studentIds)
        {
            var gradigDic = gradingStandards.GroupBy(x => x.Standard.Id).ToDictionary(x => x.Key, x => x.ToList());
            var res = new List<StandardGradingViewData>();
            foreach (var kv in gradigDic)
            {
                var gradingSt = kv.Value.First();
                var standardGrading = new StandardGradingViewData
                    {
                        Standard = StandardViewData.Create(gradingSt.Standard),
                        NumericAvg = (int?) kv.Value.Average(x => x.NumericGrade),
                        Items = new List<StandardGradingItemViewData>()
                    };
                res.Add(standardGrading);
                foreach (var studentId in studentIds)
                {
                    var gradingInfo = kv.Value.FirstOrDefault(x=>x.StudentId == studentId);
                    if(gradingInfo != null)
                        standardGrading.Items.Add(StandardGradingItemViewData.Create(gradingInfo));
                }
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
        public decimal? NumericGrade { get; set; }
        public int GradingPeriodId { get; set; }
        public int ClassId { get; set; }
        public string Comment { get; set; }

        public static StandardGradingItemViewData Create(GradingStandardInfo gradingStandard)
        {
            var res = new StandardGradingItemViewData
                {
                    StudentId = gradingStandard.StudentId,
                    GradeId = gradingStandard.AlphaGradeId,
                    GradeValue = gradingStandard.AlphaGradeName,
                    StandardId = gradingStandard.Standard.Id,
                    GradingPeriodId = gradingStandard.GradingPeriodId,
                    ClassId = gradingStandard.ClassId,
                    Comment = gradingStandard.Note
                };
            if (string.IsNullOrEmpty(res.GradeValue) && gradingStandard.NumericGrade.HasValue)
                res.GradeValue = gradingStandard.NumericGrade.Value.ToString();
            res.NumericGrade = gradingStandard.NumericGrade;
            return res;
        }
    }
}
