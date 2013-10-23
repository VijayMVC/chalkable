using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.FinalGradesViewData;

namespace Chalkable.Web.Models
{
    public class GradingStudentClassSummaryViewData
    {
        public MarkingPeriodViewData CurrentMarkingPeriod { get; set; }
        public IList<GradingStatsByDateViewData> AvgPerDate { get; set; }
        public IList<FinalGradeAnnTypeGradeStatsViewData> AnnTypesGradeStats { get; set; }
        public IList<GradingClassSummaryViewData> GradingPerMp { get; set; }
        
        public static GradingStudentClassSummaryViewData Create(StudentClassGradeStats gradingStatsPerDate
            , MarkingPeriod currentMp,  IList<FinalGradeAnnouncementType> fgAnnTypes
            , IList<GradingClassSummaryViewData> gradingPerMp)
        {
            return new GradingStudentClassSummaryViewData
                {
                    CurrentMarkingPeriod = MarkingPeriodViewData.Create(currentMp),
                    AvgPerDate = GradingStatsByDateViewData.Create(gradingStatsPerDate.GradeAvgPerDates),
                    AnnTypesGradeStats = FinalGradeAnnTypeGradeStatsViewData.Create(gradingStatsPerDate.AnnTypesGradeStats, fgAnnTypes),
                    GradingPerMp = gradingPerMp
                };
        }
    }
}