using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentSummaryInfo
    {
        public StudentDetails StudentInfo { get; set; }
        public ClassRankInfo ClassRank { get; set; }
        public int? CurrentSectionId { get; set; }
        public string CurrentAttendanceLevel { get; set; }
        public DailyAbsenceSummaryInfo DailyAttendance { get; set; }
        public int TotalDisciplineOccurrences { get; set; }
        public IList<InfractionSummaryInfo> InfractionSummaries { get; set; }
        public IList<StudentAnnouncement> StudentAnnouncements { get; set; }
        public IList<ShortStudentClassAttendanceSummary> Attendances { get; set; } 

        public static StudentSummaryInfo Create(StudentDetails student, NowDashboard nowDashboard
            , IList<Data.School.Model.Infraction> infractions, IList<AnnouncementComplex> anns, IMapper<StudentAnnouncement, Score> mapper)
        {
            var res = new StudentSummaryInfo
                {
                    StudentInfo = student, 
                    ClassRank = nowDashboard.ClassRank != null ? ClassRankInfo.Create(nowDashboard.ClassRank) : null,
                    CurrentSectionId = nowDashboard.CurrentSectionId,
                    TotalDisciplineOccurrences = nowDashboard.Infractions.Any() ? nowDashboard.Infractions.Sum(x=>x.Occurrences) : 0,
                    InfractionSummaries = InfractionSummaryInfo.Create(nowDashboard.Infractions.ToList(), infractions),
                    StudentAnnouncements = new List<StudentAnnouncement>(),
                    Attendances = ShortStudentClassAttendanceSummary.Create(nowDashboard.SectionAttendance.ToList()),
                    CurrentAttendanceLevel = nowDashboard.CurrentAttendanceStatus
                };
            if (nowDashboard.DailyAttendance == null)
                nowDashboard.DailyAttendance = new StudentDailyAbsenceSummary{ Absences = 0, Tardies = 0, Presents = 0 };
            res.DailyAttendance = DailyAbsenceSummaryInfo.Create(nowDashboard.DailyAttendance);

            var scores = nowDashboard.Scores.Where(x => !string.IsNullOrEmpty(x.ScoreValue)).ToList();
            foreach (var score in scores)
            {
                var ann = anns.FirstOrDefault(x => x.ClassAnnouncementData.SisActivityId == score.ActivityId);
                if(ann == null) continue;
                var studentAnn = new StudentAnnouncement {AnnouncementId = ann.Id};
                mapper.Map(studentAnn, score);
                res.StudentAnnouncements.Add(studentAnn);
            }              
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
}
