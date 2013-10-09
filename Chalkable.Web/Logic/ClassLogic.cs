using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models;

namespace Chalkable.Web.Logic
{
    public class ClassLogic
    {
        public static IList<GradingClassSummaryViewData> GetGradingSummary(IServiceLocatorSchool locator, Guid classId, Guid currentSchoolYearId,
                Guid? teacherId = null, Guid? studentId = null, bool canCreateItem = true)
        {
            var schoolYearId = currentSchoolYearId;
            var nowDate = locator.Context.NowSchoolTime.Date;
            var gradeAvgClass = locator.GradingStatisticService.GetClassGradeAvgPerMP(classId, schoolYearId, null, teacherId);
            var announcements = locator.AnnouncementService.GetAnnouncements(false, 0, int.MaxValue, classId);
            IList<GradingClassSummaryViewData> res = new List<GradingClassSummaryViewData>();
            IList<FinalGradeAnnouncementType> fgAnnTypes = null;
            var mapper = locator.GradingStyleService.GetMapper();
            foreach (var markingPeriodClassGradeAvg in gradeAvgClass)
            {
                if (markingPeriodClassGradeAvg.MarkingPeriod.StartDate <= nowDate)
                {
                    fgAnnTypes = locator.FinalGradeService.GetFinalGradeAnnouncementTypes(markingPeriodClassGradeAvg.Id);
                    foreach (var finalGradeAnnouncementType in fgAnnTypes)
                    {
                        finalGradeAnnouncementType.AnnouncementType.CanCreate = canCreateItem;
                    }
                }
                if (fgAnnTypes != null && fgAnnTypes.Count > 0)
                {
                    res.Add(GradingClassSummaryViewData.Create(announcements, fgAnnTypes, markingPeriodClassGradeAvg, mapper));
                }
            }
            return res.OrderBy(t => t.MarkingPeriod.StartDate).ToList();
        } 
    }
}