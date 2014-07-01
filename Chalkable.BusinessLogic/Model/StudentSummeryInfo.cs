using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentSummeryInfo
    {
        public Person StudentInfo { get; set; }
        public ClassRankInfo ClassRank { get; set; }
        public DailyAbsenceSummaryInfo DailyAttendance { get; set; }
        public int TotalDisciplineOccurrences { get; set; }
        public IList<InfractionSummaryInfo> InfractionSummaries { get; set; }
        public IList<StudentAnnouncementGrade> StudentAnnouncements { get; set; }

        public IList<ClassAttendanceSummary> Attendances { get; set; } 

        public static StudentSummeryInfo Create(Person student, NowDashboard nowDashboard, IList<Data.School.Model.Infraction> infractions)
        {
            var res = new StudentSummeryInfo
                {
                    StudentInfo = student, 
                    ClassRank = ClassRankInfo.Create(nowDashboard.ClassRank),
                    TotalDisciplineOccurrences = nowDashboard.Infractions.Sum(x=>x.Occurrences),
                    InfractionSummaries = InfractionSummaryInfo.Create(nowDashboard.Infractions.ToList(), infractions),
                    StudentAnnouncements = new List<StudentAnnouncementGrade>(),
                    Attendances = ClassAttendanceSummary.Create(nowDashboard.SectionAttendance.ToList())
                };
            if (nowDashboard.DailyAttendance == null)
                nowDashboard.DailyAttendance = new DailyAbsenceSummary {Absences = 0, Tardies = 0};
            res.DailyAttendance = DailyAbsenceSummaryInfo.Create(nowDashboard.DailyAttendance);
            return res;
        }
    }

    public class ClassRankInfo
    {
        public short? ClassSize { get; set; }
        public short? Rank { get; set; }

        public static ClassRankInfo Create(ClassRank classRank)
        {
            return new ClassRankInfo
                {
                    Rank = classRank.Rank,
                    ClassSize = classRank.ClassSize
                };
        }
    }

    public class DailyAbsenceSummaryInfo
    {
        public decimal? Absences { get; set; }
        public int? Tardies { get; set; }

        public static DailyAbsenceSummaryInfo Create(DailyAbsenceSummary dailyAbsenceSummary)
        {
            return new DailyAbsenceSummaryInfo
                {
                    Absences = dailyAbsenceSummary.Absences,
                    Tardies = dailyAbsenceSummary.Tardies
                };
        }
    }

    public class InfractionSummaryInfo
    {
        public int Occurrences { get; set; }
        public Data.School.Model.Infraction Infraction { get; set; }

        public static IList<InfractionSummaryInfo> Create(IList<InfractionSummary> infractionSummaries
            , IList<Data.School.Model.Infraction> infractions)
        {
            return infractionSummaries.Select(x => new InfractionSummaryInfo
                {
                    Infraction = infractions.First(infraction => x.InfractionId == infraction.Id), 
                    Occurrences = x.Occurrences
                }).ToList();
        } 
    }

    public class ClassAttendanceSummary
    {
        public int ClassId { get; set; }
        public decimal Absences { get; set; }
        public int Tardies { get; set; }

        public static IList<ClassAttendanceSummary> Create(IList<SectionAbsenceSummary> absences)
        {
            return absences.Select(x => new ClassAttendanceSummary()
                {
                    ClassId = x.SectionId,
                    Absences = x.Absences ?? 0,
                    Tardies = x.Tardies ?? 0
                }).ToList();
        }
    }
}
