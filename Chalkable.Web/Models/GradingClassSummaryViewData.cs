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
            MarkingPeriodClassGradeAvg classGradingStats, IGradingStyleMapper mapper, IList<StudentAnnouncementGrade> stAnnGrades = null)
        {
            throw new NotImplementedException();
            //var res = new GradingClassSummaryViewData
            //        {
            //            MarkingPeriod = MarkingPeriodViewData.Create(classGradingStats.MarkingPeriod),
            //            Avg = classGradingStats.Avg,
            //            ByAnnouncementTypes = new List<GradingClassSummaryItemViewData>()
            //        };
            //announcements = announcements.Where(x => x.MarkingPeriodClassRef == classGradingStats.Id).ToList();
            //foreach (var fgAnnouncementType in fgAnnouncementTypes)
            //{
            //    var annPerMp = announcements.Where(x => x.AnnouncementTypeRef == fgAnnouncementType.AnnouncementTypeRef).ToList();
            //    res.ByAnnouncementTypes.Add(GradingClassSummaryItemViewData.Create(annPerMp, fgAnnouncementType.AnnouncementType, fgAnnouncementType.PercentValue, mapper, stAnnGrades));
            //}
            //return res;
        }
    }

    public class GradingClassSummaryItemViewData
    {
        public AnnouncementTypeViewData Type { get; set; }
        public IList<AnnouncementShortGradeViewData> Announcements { get; set; }
        public int Percent { get; set; }
        public int? Avg { get; set; }
        public int? StudentAvg { get; set; }

        public static GradingClassSummaryItemViewData Create(IList<AnnouncementComplex> announcements,
                       AnnouncementType announcementType, int typePercent, IGradingStyleMapper mapper
            , IList<StudentAnnouncementGrade> stAnnGrades = null)
        {
            var res = new GradingClassSummaryItemViewData
                {
                    Percent = typePercent,
                    Type = AnnouncementTypeViewData.Create(announcementType),
                };
            if (stAnnGrades == null)
                res.Announcements = AnnouncementShortGradeViewData.Create(announcements, mapper);
            else
            {
                res.Announcements = new List<AnnouncementShortGradeViewData>();
                foreach (var ann in announcements)
                {
                    var stAnn = stAnnGrades.FirstOrDefault(x=>x.AnnouncementRef == ann.Id);
                    res.Announcements.Add(AnnouncementShortGradeViewData.Create(ann, mapper, stAnn != null ? stAnn.GradeValue : null));
                }
                res.StudentAvg = (int?) res.Announcements.Average(x => x.Grade);
            }
            res.Avg = (int?) res.Announcements.Average(x => x.Avg);
            return res;
        }
    }
}