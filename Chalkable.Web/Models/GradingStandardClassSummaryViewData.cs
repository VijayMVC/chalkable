using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class GradingStandardClassSummaryViewData
    {
        public GradingPeriodViewData GradingPeriod { get; set; }
        public decimal? Avg { get; set; }
        public IList<GradingStandardClassItemViewData> Items { get; set; }
 
        public static GradingStandardClassSummaryViewData Create(GradingPeriod gradingPeriod
            , IList<GradingStandardInfo> gradingStandards, IList<AnnouncementComplex> announcements
            , IList<AnnouncementStandard> announcementStandards)
        {
            var res = new GradingStandardClassSummaryViewData
                {
                    GradingPeriod = GradingPeriodViewData.Create(gradingPeriod),
                    Items = new List<GradingStandardClassItemViewData>()
                };
            foreach (var gradingStandard in gradingStandards)
            {
                var annIds = announcementStandards.Where(x => x.StandardRef == gradingStandard.Standard.Id).Select(x=>x.AnnouncementRef).ToList();
                var anns = announcements.Where(x => annIds.Contains(x.Id)).ToList();
                res.Items.Add(GradingStandardClassItemViewData.Create(anns, gradingStandard));
            }
            if(res.Items.Count > 0)
                res.Avg = res.Items.Average(x => x.Avg);
            return res;
        }
    }

    public class GradingStandardClassItemViewData
    {
        public AnnouncementStandardViewData ItemDescription { get; set; }
        public decimal? Avg { get; set; } 
        public IList<AnnouncementShortViewData> Announcements { get; set; }
 
        public static GradingStandardClassItemViewData Create(IList<AnnouncementComplex> announcements,
                                                              GradingStandardInfo gradingStandard)
        {
            return new GradingStandardClassItemViewData
                {
                    ItemDescription = AnnouncementStandardViewData.Create(gradingStandard.Standard),
                    Avg = gradingStandard.NumericGrade,
                    Announcements = AnnouncementShortViewData.Create(announcements)
                };
        }
    }
}