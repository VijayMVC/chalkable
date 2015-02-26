﻿using System;
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
        public static IList<GradingClassSummaryViewData> GetGradingSummary(IServiceLocatorSchool locator, int classId, int currentSchoolYearId,
                int? teacherId = null, int? studentId = null, bool canCreateItem = true)
        {
            throw new NotImplementedException();
            //var schoolYearId = currentSchoolYearId;
            //var nowDate = locator.Context.NowSchoolTime.Date;
            //var gradeAvgClass = locator.GradingStatisticService.GetClassGradeAvgPerMP(classId, schoolYearId, null, teacherId);
            //var announcements = locator.AnnouncementService.GetAnnouncements(false, 0, int.MaxValue, classId);
            //IList<StudentAnnouncementGrade> stAnns = null;
            //if (studentId.HasValue)
            //{
            //    stAnns = locator.StudentAnnouncementService.GetLastGrades(studentId.Value, classId);
            //}
            //IList<GradingClassSummaryViewData> res = new List<GradingClassSummaryViewData>();
            //IList<FinalGradeAnnouncementType> fgAnnTypes = null;
            //var mapper = locator.GradingStyleService.GetMapper();
            //foreach (var markingPeriodClassGradeAvg in gradeAvgClass)
            //{
            //    if (markingPeriodClassGradeAvg.MarkingPeriod.StartDate <= nowDate)
            //    {
            //        fgAnnTypes = locator.FinalGradeService.GetFinalGradeAnnouncementTypes(markingPeriodClassGradeAvg.Id);
            //        foreach (var finalGradeAnnouncementType in fgAnnTypes)
            //        {
            //            finalGradeAnnouncementType.AnnouncementType.CanCreate = canCreateItem;
            //        }
            //    }
            //    if (fgAnnTypes != null && fgAnnTypes.Count > 0)
            //    {
            //        res.Add(GradingClassSummaryViewData.Create(announcements, fgAnnTypes, markingPeriodClassGradeAvg, mapper, stAnns));
            //    }
            //}
            //return res.OrderBy(t => t.MarkingPeriod.StartDate).ToList();
        } 
    }
}