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
            var gsDic = gradingStandards.GroupBy(x => x.Standard.Id).ToDictionary(x=>x.Key, x=>x.ToList());
            foreach (var kv in gsDic)
            {
                var annIds = announcementStandards.Where(x => x.StandardRef == kv.Key).Select(x => x.AnnouncementRef).ToList();
                var anns = announcements.Where(x => annIds.Contains(x.Id)).ToList();
                res.Items.Add(GradingStandardClassItemViewData.Create(anns, kv.Value.Average(x => x.NumericGrade)
                    , kv.Value.First().Standard));
            }
            if(res.Items.Count > 0)
                res.Avg = res.Items.Average(x => x.StandardAvg);
            return res;
        }
    }

    public class GradingStandardClassItemViewData
    {
        public AnnouncementStandardViewData Standard { get; set; }
        public decimal? StandardAvg { get; set; } 
        public IList<AnnouncementShortViewData> Announcements { get; set; }
 
        public static GradingStandardClassItemViewData Create(IList<AnnouncementComplex> announcements,
                                                              decimal? avg, Standard standard)
        {
            return new GradingStandardClassItemViewData
                {
                    Standard = AnnouncementStandardViewData.Create(standard),
                    StandardAvg = avg,
                    Announcements = AnnouncementShortViewData.Create(announcements)
                };
        }
    }
}