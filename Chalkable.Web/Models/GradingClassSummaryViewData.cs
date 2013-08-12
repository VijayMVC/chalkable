using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class GradingClassSummaryViewData
    {
        public IList<GradingClassSummaryItemViewData> ByAnnouncementTypes { get; set; }
        public MarkingPeriodViewData MarkingPeriod { get; set; }
        public int? Avg { get; set; }

        public static GradingClassSummaryViewData Create(IList<AnnouncementComplex> announcements, IList<FinalGradeAnnouncementType> fgAnnouncementTypes,
            MarkingPeriodClassGradeAvg classGradingStats, IGradingStyleMapper mapper)
        {
            var res = new GradingClassSummaryViewData
                    {
                        MarkingPeriod = MarkingPeriodViewData.Create(classGradingStats.MarkingPeriod),
                        Avg = classGradingStats.Avg
                    };
            foreach (var fgAnnouncementType in fgAnnouncementTypes)
            {
                var annPerMp = announcements.Where(x => x.MarkingPeriodClassRef == fgAnnouncementType.FinalGradeRef 
                                                      && x.AnnouncementTypeRef == fgAnnouncementType.AnnouncementTypeRef).ToList();
                res.ByAnnouncementTypes.Add(GradingClassSummaryItemViewData.Create(annPerMp, fgAnnouncementType.AnnouncementType, fgAnnouncementType.PercentValue, mapper));
            }
            return res;
        }
    }

    public class GradingClassSummaryItemViewData
    {
        public AnnouncementTypeViewData Type { get; set; }
        public IList<AnnouncementShortGradeViewData> Announcements { get; set; }
        public int Persent { get; set; }
        public int? Avg { get; set; }

        public static GradingClassSummaryItemViewData Create(IList<AnnouncementComplex> announcements,
                       AnnouncementType announcementType, int typePersent, IGradingStyleMapper mapper)
        {
            var res = new GradingClassSummaryItemViewData()
                {
                    Persent = typePersent,
                    Type = AnnouncementTypeViewData.Create(announcementType),
                    Announcements = AnnouncementShortGradeViewData.Create(announcements, mapper)
                };
            res.Avg = (int?) res.Announcements.Average(x => x.Avg);
            return res;
        }
    }
}