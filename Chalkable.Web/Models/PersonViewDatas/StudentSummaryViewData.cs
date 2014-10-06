using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentSummaryViewData : PersonSummaryViewData
    {

        public int GradeLevelNumber { get; set; }
        public string CurrentClassName { get; set; }
        public string CurrentAttendanceLevel { get; set; }
        public int MaxPeriodNumber { get; set; }
        public IList<AnnouncementsClassPeriodViewData> PeriodSection { get; set; }
        public IList<ClassViewData> ClassesSection { get; set; }
        public StudentHoverBoxViewData<TotalAbsencesPerClassViewData> AttendanceBox { get; set; }
        public StudentHoverBoxViewData<DisciplineTotalPerTypeViewData> DisciplineBox { get; set; }
        public StudentHoverBoxViewData<StudentSummeryGradeViewData> GradesBox { get; set; }
        public StudentHoverBoxViewData<StudentSummeryRankViewData> RanksBox { get; set; }
        public IList<StudentHealthConditionViewData> HealthConditions { get; set; }

        protected StudentSummaryViewData(Person person, Room room) : base(person, room)
        {
        }

        public static StudentSummaryViewData Create(StudentSummaryInfo studentSummary, Room room,  ClassDetails currentClass, IList<ClassDetails> classes)
        {
            var res = new StudentSummaryViewData(studentSummary.StudentInfo, room)
                {
                    ClassesSection = ClassViewData.Create(classes),
                    AttendanceBox = StudentHoverBoxViewData<TotalAbsencesPerClassViewData>.Create(studentSummary.DailyAttendance, studentSummary.Attendances, classes),
                    DisciplineBox = StudentHoverBoxViewData<DisciplineTotalPerTypeViewData>.Create(studentSummary.InfractionSummaries, studentSummary.TotalDisciplineOccurrences),
                    GradesBox = StudentHoverBoxViewData<StudentSummeryGradeViewData>.Create(studentSummary.StudentAnnouncements),
                    RanksBox = studentSummary.ClassRank != null ? StudentHoverBoxViewData<StudentSummeryRankViewData>.Create(studentSummary.ClassRank) : null,
                };
            if (currentClass != null)
            {
                res.GradeLevelNumber = currentClass.GradeLevel.Number;
                res.CurrentClassName = currentClass.Name;
            }
            res.CurrentAttendanceLevel = studentSummary.CurrentAttendanceLevel;
            return res;
        }
    }

    public class StudentHoverBoxViewData<T> : HoverBoxesViewData<T>
    {
        public bool IsPassing { get; set; }
        
        public static StudentHoverBoxViewData<StudentSummeryRankViewData> Create(ClassRankInfo rankInfo)
        {
            var res = new StudentHoverBoxViewData<StudentSummeryRankViewData>();
            var rank = rankInfo.Rank;
            res.IsPassing = true;
            res.Title = rank.HasValue ? string.Format("{0} of {1}", rankInfo.Rank, rankInfo.ClassSize) : "";
            return res;
        }

        public static StudentHoverBoxViewData<DisciplineTotalPerTypeViewData> Create(IList<InfractionSummaryInfo> infractionSummaryInfos, int totalDisciplineOccurrences)
        {
            var res = new StudentHoverBoxViewData<DisciplineTotalPerTypeViewData>
                {
                    Hover = DisciplineTotalPerTypeViewData.Create(infractionSummaryInfos).OrderByDescending(x => x.Total).ToList(),
                    Title = totalDisciplineOccurrences.ToString()
                };
            return res;
        }

        public static StudentHoverBoxViewData<TotalAbsencesPerClassViewData> Create(DailyAbsenceSummaryInfo dailyAbsenceSummary
            , IList<ClassAttendanceSummary> attendances, IList<ClassDetails> classDetailses)
        {
            var res = new StudentHoverBoxViewData<TotalAbsencesPerClassViewData>
                {
                    Hover = TotalAbsencesPerClassViewData.Create(attendances, classDetailses),
                };
            if (dailyAbsenceSummary != null && dailyAbsenceSummary.Absences != null)
                res.Title = (dailyAbsenceSummary.Absences + dailyAbsenceSummary.Tardies).ToString();
            return res;
        }

        public static  StudentHoverBoxViewData<StudentSummeryGradeViewData> Create(IList<StudentAnnouncement> studentAnnouncements)
        {
            var firstStudentAn = studentAnnouncements.FirstOrDefault();
            var res = new StudentHoverBoxViewData<StudentSummeryGradeViewData>
            {
                Hover = StudentSummeryGradeViewData.Create(studentAnnouncements),
            };
            if (firstStudentAn != null && !string.IsNullOrEmpty(firstStudentAn.ScoreValue))
            {
                res.Title = firstStudentAn.ScoreValue;
                res.IsPassing = firstStudentAn.NumericScore >= 65;
            }
            return res;
        }
    }


    public class StudentSummeryRankViewData
    {
    }

    public class DisciplineTotalPerTypeViewData
    {
        public DisciplineTypeViewData DisciplineType { get; set; }
        public int Total { get; set; }

        public static IList<DisciplineTotalPerTypeViewData> Create(
            IList<InfractionSummaryInfo> disciplineTotalPerTypes)
        {
           return disciplineTotalPerTypes.Select(x => new DisciplineTotalPerTypeViewData
                {
                    DisciplineType = DisciplineTypeViewData.Create(x.Infraction),
                    Total = x.Occurrences
                }).ToList();
        }
    }

    public class TotalAbsencesPerClassViewData
    {
        public decimal Absences { get; set; }
        public ShortClassViewData Class { get; set; }

        public static IList<TotalAbsencesPerClassViewData> Create(IList<ClassAttendanceSummary> attendances, IList<ClassDetails> classDetailses)
        {
            var atts = attendances.OrderByDescending(x=>x.Absences).Take(5);
            var res = new List<TotalAbsencesPerClassViewData>();
            foreach (var classAttendanceSummary in atts)
            {

                var c = classDetailses.FirstOrDefault(x => x.Id == classAttendanceSummary.ClassId);
                if (c == null) continue;
                res.Add(new TotalAbsencesPerClassViewData
                {
                    Absences = classAttendanceSummary.Absences + classAttendanceSummary.Tardies,
                    Class = ShortClassViewData.Create(c)
                });
            }
            return res;
        }
    }

    public class StudentSummeryGradeViewData
    {
        public string Grade { get; set; }
        public string AnnouncmentTitle { get; set; }
        public int AnnouncementId { get; set; }
        public static StudentSummeryGradeViewData Create(StudentAnnouncement studentAnnouncement)
        {
            var res = new StudentSummeryGradeViewData
            {
                Grade = studentAnnouncement.ScoreValue,
                AnnouncmentTitle = studentAnnouncement.AnnouncementTitle,
                AnnouncementId = studentAnnouncement.AnnouncementId
            };
            return res;
        }
        public static IList<StudentSummeryGradeViewData> Create(IList<StudentAnnouncement> studentAnnouncements)
        {
            return studentAnnouncements.Select(Create).ToList();
        }
    }
}