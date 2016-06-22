using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentSummaryViewData : StudentProfileViewData
    {
        public string CurrentClassName { get; set; }
        public string CurrentAttendanceLevel { get; set; }
        public int MaxPeriodNumber { get; set; }
        public IList<ClassViewData> ClassesSection { get; set; }
        public StudentHoverBoxViewData<TotalAbsencesPerClassViewData> AttendanceBox { get; set; }
        public StudentHoverBoxViewData<DisciplineTypeSummaryViewData> DisciplineBox { get; set; }
        public StudentHoverBoxViewData<StudentSummaryGradeViewData> GradesBox { get; set; }
        public StudentHoverBoxViewData<StudentSummaryRankViewData> RanksBox { get; set; }

        private const string NO_CLASS_SCHEDULED = "No Class Scheduled";

        public int? RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomNumber { get; set; }

        protected StudentSummaryViewData(StudentDetails student, Room room, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions) : base(student, customAlerts, healthConditions)
        {
            if (room == null) return;
            RoomId = room.Id;
            RoomName = room.Description;
            RoomNumber = room.RoomNumber;
        }

        public static StudentSummaryViewData Create(StudentSummaryInfo studentSummary, Room room,  ClassDetails currentClass, IList<ClassDetails> classes
            , IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions, bool isStudent = false)
        {
            var res = new StudentSummaryViewData(studentSummary.StudentInfo, room, customAlerts, healthConditions)
            {
                ClassesSection = ClassViewData.Create(classes),
                AttendanceBox =
                    StudentHoverBoxViewData<TotalAbsencesPerClassViewData>.Create(studentSummary.DailyAttendance,
                        studentSummary.Attendances, classes),
                DisciplineBox =
                    StudentHoverBoxViewData<DisciplineTypeSummaryViewData>.Create(studentSummary.InfractionSummaries,
                        studentSummary.TotalDisciplineOccurrences),
                GradesBox =
                    StudentHoverBoxViewData<StudentSummaryGradeViewData>.Create(studentSummary.StudentAnnouncements),
                RanksBox =
                    studentSummary.ClassRank != null
                        ? StudentHoverBoxViewData<StudentSummaryRankViewData>.Create(studentSummary.ClassRank)
                        : null,
                CurrentClassName = NO_CLASS_SCHEDULED,
            };
            if (currentClass != null)
                res.CurrentClassName = currentClass.Name;

            res.CurrentAttendanceLevel = studentSummary.CurrentAttendanceLevel;

            if(isStudent)
                ClearAlertsForStudent(res);

            return res;
        }

        private static void ClearAlertsForStudent(StudentSummaryViewData vData)
        {
            vData.HasMedicalAlert = false;
            vData.SpEdStatus = null;
            vData.SpecialInstructions = null;
            vData.IsAllowedInetAccess = false;
        }
    }

    public class StudentHoverBoxViewData<T> : HoverBoxesViewData<T>
    {
        public static StudentHoverBoxViewData<StudentSummaryRankViewData> Create(ClassRankInfo rankInfo)
        {
            var res = new StudentHoverBoxViewData<StudentSummaryRankViewData>();
            var rank = rankInfo.Rank;
            res.IsPassing = true;
            res.Title = rank.HasValue ? string.Format("{0} of {1}", rankInfo.Rank, rankInfo.ClassSize) : "";
            return res;
        }

        public static StudentHoverBoxViewData<DisciplineTypeSummaryViewData> Create(IList<InfractionSummaryInfo> infractionSummaryInfos, int totalDisciplineOccurrences)
        {
            var res = new StudentHoverBoxViewData<DisciplineTypeSummaryViewData>
                {
                    Hover = DisciplineTypeSummaryViewData.Create(infractionSummaryInfos).OrderByDescending(x => x.Total).ToList(),
                    Title = totalDisciplineOccurrences.ToString(CultureInfo.InvariantCulture)
                };
            return res;
        }

        public static StudentHoverBoxViewData<TotalAbsencesPerClassViewData> Create(DailyAbsenceSummaryInfo dailyAbsenceSummary
            , IList<ShortStudentClassAttendanceSummary> attendances, IList<ClassDetails> classDetailses)
        {
            var res = new StudentHoverBoxViewData<TotalAbsencesPerClassViewData>
                {
                    Hover = TotalAbsencesPerClassViewData.Create(attendances, classDetailses),
                };
            decimal totalAbsencesCount = 0;
            if (dailyAbsenceSummary != null && dailyAbsenceSummary.Absences.HasValue)
                totalAbsencesCount += dailyAbsenceSummary.Absences.Value; // Excluded tardies because of Jonathan Whitehurst's comment on CHLK-3184 
            //totalAbsencesCount += res.Hover.Sum(x => x.Absences);
            res.Title = totalAbsencesCount.ToString(CultureInfo.InvariantCulture);
            return res;
        }

        public static  StudentHoverBoxViewData<StudentSummaryGradeViewData> Create(IList<StudentAnnouncement> studentAnnouncements)
        {
            var firstStudentAn = studentAnnouncements.FirstOrDefault();
            var res = new StudentHoverBoxViewData<StudentSummaryGradeViewData>
            {
                Hover = StudentSummaryGradeViewData.Create(studentAnnouncements),
            };
            if (!string.IsNullOrEmpty(firstStudentAn?.ScoreValue))
            {
                res.Title = firstStudentAn.ScoreValue;
                res.IsPassing = firstStudentAn.NumericScore >= 65;
            }
            return res;
        }
    }


    public class StudentSummaryRankViewData
    {
    }

    public class TotalAbsencesPerClassViewData
    {
        public decimal Absences { get; set; }
        public ShortClassViewData Class { get; set; }

        public static IList<TotalAbsencesPerClassViewData> Create(IList<ShortStudentClassAttendanceSummary> attendances, IList<ClassDetails> classDetailses)
        {
            var atts = attendances.OrderByDescending(x=>x.Absences);
            var res = new List<TotalAbsencesPerClassViewData>();
            foreach (var classAttendanceSummary in atts)
            {
                if (classAttendanceSummary.Absences <= 0) continue;
                var c = classDetailses.FirstOrDefault(x => x.Id == classAttendanceSummary.ClassId);
                if (c == null) continue;
                res.Add(new TotalAbsencesPerClassViewData
                {
                    Absences = classAttendanceSummary.Absences ?? 0, // Excluded tardies because of Jonathan Whitehurst's comment on CHLK-3184 
                    Class = ShortClassViewData.Create(c)
                });
            }
            return res;
        }
    }

    public class StudentSummaryGradeViewData
    {
        public string Grade { get; set; }
        public string AnnouncementTitle { get; set; }
        public int AnnouncementId { get; set; }
        public static StudentSummaryGradeViewData Create(StudentAnnouncement studentAnnouncement)
        {
            var res = new StudentSummaryGradeViewData
            {
                Grade = studentAnnouncement.ScoreValue,
                AnnouncementTitle = studentAnnouncement.AnnouncementTitle,
                AnnouncementId = studentAnnouncement.AnnouncementId
            };
            return res;
        }
        public static IList<StudentSummaryGradeViewData> Create(IList<StudentAnnouncement> studentAnnouncements)
        {
            return studentAnnouncements.Select(Create).ToList();
        }
    }
}