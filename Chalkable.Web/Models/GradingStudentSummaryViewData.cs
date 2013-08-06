using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class GradingStudentSummaryViewData
    {
        public IList<AnnouncementViewData> Announcements { get; set; }
        public IList<GradingStatsByDateViewData> TotalAvgPerDate { get; set; }
        public IList<GradingStatsByDateViewData> PeersAvgPerDate { get; set; }

        public static GradingStudentSummaryViewData Create(IList<AnnouncementComplex> announcements, IList<StudentGradeAvgPerDate> studentStats)
        {
            return new GradingStudentSummaryViewData
                {
                    Announcements = AnnouncementViewData.Create(announcements),
                    TotalAvgPerDate = studentStats.Select(GradingStatsByDateViewData.Create).ToList(),
                    PeersAvgPerDate = studentStats.Select(x => GradingStatsByDateViewData.Create(x.Date, x.PeersAvg)).ToList()
                };
        }
    }


    public class GradingStatsByDateViewData
    {
        public DateTime Date { get; set; }
        public int? Avg { get; set; }

        public static GradingStatsByDateViewData Create(DateTime date, int? avg)
        {
            return new GradingStatsByDateViewData {Avg = avg, Date = date};
        }
        public static GradingStatsByDateViewData Create(GradeAvgPerDate gradeAvgPerDate)
        {
            return Create(gradeAvgPerDate.Date, gradeAvgPerDate.Avg);
        }
   }
}