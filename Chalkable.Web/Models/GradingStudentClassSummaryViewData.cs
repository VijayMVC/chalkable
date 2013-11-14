﻿using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class GradingStudentClassSummaryViewData
    {
        public MarkingPeriodViewData CurrentMarkingPeriod { get; set; }
        public IList<GradingStatsByDateViewData> AvgPerDate { get; set; }
        public IList<GradingClassSummaryViewData> GradingPerMp { get; set; }
        
        public static GradingStudentClassSummaryViewData Create(StudentClassGradeStats gradingStatsPerDate
            , MarkingPeriod currentMp, IList<GradingClassSummaryViewData> gradingPerMp)
        {
            return new GradingStudentClassSummaryViewData
                {
                    CurrentMarkingPeriod = MarkingPeriodViewData.Create(currentMp),
                    AvgPerDate = GradingStatsByDateViewData.Create(gradingStatsPerDate.GradeAvgPerDates),
                    GradingPerMp = gradingPerMp
                };
        }
    }
}