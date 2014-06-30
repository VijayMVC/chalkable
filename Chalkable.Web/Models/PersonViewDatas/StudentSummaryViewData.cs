using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Mapping;
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
        public string CurrentAttendanceType { get; set; }
        public int MaxPeriodNumber { get; set; }
        public IList<AnnouncementsClassPeriodViewData> PeriodSection { get; set; }
        public IList<ClassViewData> ClassesSection { get; set; } 
        public StudentHoverBoxViewData<AttendanceTotalPerTypeViewData> AttendanceBox { get; set; }
        public StudentHoverBoxViewData<DisciplineTotalPerTypeViewData> DisciplineBox { get; set; }
        public StudentHoverBoxViewData<StudentSummeryGradeViewData> GradesBox { get; set; }
        public StudentHoverBoxViewData<StudentSummeryRankViewData> RanksBox { get; set; }
        protected StudentSummaryViewData(Person person, Room room) : base(person, room)
        {
        }

        public static StudentSummaryViewData Create(StudentSummeryInfo studentSummary, Room room,  ClassDetails currentClass, IList<ClassDetails> classes)
        {
            var res = new StudentSummaryViewData(studentSummary.StudentInfo, room)
                {
                    ClassesSection = ClassViewData.Create(classes),
                    AttendanceBox = StudentHoverBoxViewData<AttendanceTotalPerTypeViewData>.Create(studentSummary.DailyAbsence),
                    DisciplineBox = StudentHoverBoxViewData<DisciplineTotalPerTypeViewData>.Create(studentSummary.InfractionSummaries, studentSummary.TotalDisciplineOccurrences),
                    //GradesBox = StudentHoverBoxViewData<StudentSummeryGradeViewData>.Create(lastGrades, mapper),
                    RanksBox = StudentHoverBoxViewData<StudentSummeryRankViewData>.Create(studentSummary.ClassRank),
                };
            if (currentClass != null)
            {
                res.GradeLevelNumber = currentClass.GradeLevel.Number;
                res.CurrentClassName = currentClass.Name;
            }
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
            res.IsPassing = rank > 50;
            res.Title = rank.HasValue ? rank.ToString() : "";
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

        public static StudentHoverBoxViewData<AttendanceTotalPerTypeViewData> Create(DailyAbsenceSummaryInfo dailyAbsenceSummary)
        {
           // var totalAbsentsAndLates = attDic.Where(x => ClassAttendance.IsAbsentOrLateLevel(x.Key)).Sum(x => x.Value);
            var res = new StudentHoverBoxViewData<AttendanceTotalPerTypeViewData>
                {
                    //Hover = AttendanceTotalPerTypeViewData.Create(attDic),
                    Title = dailyAbsenceSummary.Absences.ToString()
                };
            return res;
        }

        public static  StudentHoverBoxViewData<StudentSummeryGradeViewData> Create(IList<StudentAnnouncementGrade> studentAnnouncements, IGradingStyleMapper mapper)
        {
            var firstStudentAn = studentAnnouncements.FirstOrDefault();
            var res = new StudentHoverBoxViewData<StudentSummeryGradeViewData>
            {
                Hover = StudentSummeryGradeViewData.Create(studentAnnouncements, mapper),
            };
            if (firstStudentAn != null)
            {
                res.Title = mapper.Map(firstStudentAn.Announcement.GradingStyle, firstStudentAn.NumericScore).ToString();
                res.IsPassing = firstStudentAn.NumericScore >= 65;
            }
            return res;
        }
    }


    public class StudentSummeryRankViewData
    {
        public int MarkingPeriodId { get; set; }
        public string MarkingPeiordName { get; set; }
        public int? Rank { get; set; }
        public static IList<StudentSummeryRankViewData> Create(IList<StudentGradingRank> currentStudentRanks, IList<StudentGradingRank> allStudentsRanks)
        {
            var rankByMps = currentStudentRanks;
            var res = new List<StudentSummeryRankViewData>();
            foreach (var rankByMp in rankByMps)
            {
                var allStudentsByMp = allStudentsRanks.Where(x => x.MarkingPeriodId == rankByMp.MarkingPeriodId).ToList();
                var studentSummeryRank = new StudentSummeryRankViewData
                                        {
                                            MarkingPeriodId = rankByMp.MarkingPeriodId,
                                            MarkingPeiordName = rankByMp.MarkingPeriodName,
                                        };
                var lowRankStCount = allStudentsByMp.Count(x => x.StudentId != rankByMp.StudentId && x.Rank >= rankByMp.Rank);
                var allStudentsCount = allStudentsByMp.Count;
                studentSummeryRank.Rank = allStudentsCount > 0 ? 100 * lowRankStCount / allStudentsCount : default(int?);
                res.Add(studentSummeryRank);
            }
            return res;
        }
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

    public class AttendanceTotalPerTypeViewData
    {
        public string Level { get; set; }
        public int AttendanceCount { get; set; }

        public static IList<AttendanceTotalPerTypeViewData> Create(IDictionary<string, int> attTypeDic)
        {
            return attTypeDic.Select(x => new AttendanceTotalPerTypeViewData
                {
                    AttendanceCount = x.Value,
                    Level = x.Key
                }).ToList();
        }
    }


    public class StudentSummeryGradeViewData
    {
        public int? Grade { get; set; }
        public int GradingStyle { get; set; }
        public int? AnnouncementTypeId { get; set; }
        public string AnnouncmentTypeName { get; set; }
        public static StudentSummeryGradeViewData Create(StudentAnnouncementGrade studentAnnouncement, IGradingStyleMapper gradingMapper)
        {
            var gradingStyle = studentAnnouncement.Announcement.GradingStyle;
            var res = new StudentSummeryGradeViewData
            {
                GradingStyle = (int)gradingStyle,
                Grade = gradingMapper.Map(gradingStyle, studentAnnouncement.NumericScore),
                AnnouncementTypeId = studentAnnouncement.Announcement.ChalkableAnnouncementType,
                AnnouncmentTypeName = studentAnnouncement.Announcement.ClassAnnouncementTypeName
            };
            return res;
        }
        public static IList<StudentSummeryGradeViewData> Create(IList<StudentAnnouncementGrade> studentAnnouncements, IGradingStyleMapper mapper)
        {
            var res = studentAnnouncements.Select(x => Create(x, mapper));
            return res.ToList();
        }
    }
}