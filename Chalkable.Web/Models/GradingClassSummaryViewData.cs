using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{

    public class ClassGradingBoxesViewData
    {
        public IList<GradingPeriodViewData> GradingPeriods { get; set; }
        public GradingClassSummaryViewData CurrentGradingBox { get; set; }

        public static ClassGradingBoxesViewData Create(IList<GradingPeriodDetails> gradingPeriods,
                                                TeacherClassGrading gradingSummary)
        {
            return new ClassGradingBoxesViewData
                {
                    GradingPeriods = gradingPeriods.Select(GradingPeriodViewData.Create).ToList(),
                    CurrentGradingBox = GradingClassSummaryViewData.Create(gradingSummary)
                };
        }
    }

    public class GradingClassSummaryViewData
    {
        public IList<GradingClassSummaryItemViewData> ByAnnouncementTypes { get; set; }
        public GradingPeriodViewData GradingPeriod { get; set; }
        public decimal? Avg { get; set; }

        public static GradingClassSummaryViewData Create(IList<AnnouncementComplex> announcements, 
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

        public static GradingClassSummaryViewData Create(TeacherClassGrading gradingSummary)
        {
            return Create(gradingSummary.Announcements, gradingSummary.GradingPeriod, 
                            gradingSummary.AnnouncementTypes, gradingSummary.Avg);
        }

        public static GradingClassSummaryViewData Create(IList<AnnouncementDetails> announcements,
               GradingPeriod gradingPeriod, IList<GradedClassAnnouncementType> announcementTypes, double? avg)
        {
            var res = new GradingClassSummaryViewData
                {
                    GradingPeriod = GradingPeriodViewData.Create(gradingPeriod),
                    Avg = (decimal?)avg,
                    ByAnnouncementTypes = new List<GradingClassSummaryItemViewData>()
                };
            if (announcements != null && announcementTypes != null)
            {
                foreach (var gradedClassAnnouncementType in announcementTypes)
                {
                    var anns = announcements.Where(x => x.ClassAnnouncementTypeRef == gradedClassAnnouncementType.Id).ToList();
                    res.ByAnnouncementTypes.Add(GradingClassSummaryItemViewData.Create(anns, gradedClassAnnouncementType));
                }
            }
            return res;
        }
    }

    public class GradingClassSummaryItemViewData
    {
        public ClassAnnouncementTypeViewData Type { get; set; }
        public IList<AnnouncementShortViewData> Announcements { get; set; }
        public int Percent { get; set; }
        public decimal? Avg { get; set; }
        
        public static GradingClassSummaryItemViewData Create(IList<AnnouncementDetails> announcements,
                       GradedClassAnnouncementType announcementType)
        {
            var res = new GradingClassSummaryItemViewData
                {
                    Percent = announcementType.Percentage,
                    Type = ClassAnnouncementTypeViewData.Create(announcementType),
                    Avg = (decimal?) announcementType.Avg,
                    Announcements = announcements.Select(AnnouncementShortViewData.Create).ToList()
                };
            return res;
        }
    }
}