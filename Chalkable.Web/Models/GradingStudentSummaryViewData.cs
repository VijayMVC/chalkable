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
            var res = new GradingStudentSummaryViewData {Announcements = AnnouncementViewData.Create(announcements)};
            if (studentStats != null)
            {
                res.TotalAvgPerDate = studentStats.Select(GradingStatsByDateViewData.Create).ToList();
                res.PeersAvgPerDate = studentStats.Select(x => GradingStatsByDateViewData.Create(x.Date, x.PeersAvg)).ToList();
            }
            return res;
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
        public static IList<GradingStatsByDateViewData> Create(IList<GradeAvgPerDate> gradeAvgPerDate)
        {
            return gradeAvgPerDate.Select(Create).ToList();
        }
   }
}