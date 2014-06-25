using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.DisciplinesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class FinalGradesViewData
    {
        public IList<GradingPeriodViewData> GradingPeriods { get; set; }
        public GradingPeriodFinalGradeViewData CurrentFinalGrade { get; set; }

        public static FinalGradesViewData Create(IList<GradingPeriodDetails> gradingPeriods, GradingPeriodFinalGradeViewData gradingPeriodFinalGrade)
        {
            return new FinalGradesViewData
                {
                    GradingPeriods = gradingPeriods.Select(GradingPeriodViewData.Create).ToList(),
                    CurrentFinalGrade = gradingPeriodFinalGrade
                };
        }
    }

    public class GradingPeriodFinalGradeViewData
    {
        public GradingPeriodViewData GradingPeriod { get; set; }
        public ShortAverageViewData CurrentAverage { get; set; }
        public IList<ShortAverageViewData> Averages { get; set; }
        public IList<StudentFinalGradeViewData> StudentFinalGrades { get; set; }
        
        public static GradingPeriodFinalGradeViewData Create(FinalGradeInfo finalGrade
            ,  ChalkableAverage average)
        {
            return new GradingPeriodFinalGradeViewData
                {
                    GradingPeriod = GradingPeriodViewData.Create(finalGrade.GradeBook.GradingPeriod),
                    Averages = ShortAverageViewData.Create(finalGrade.Averages),
                    CurrentAverage = average != null ? ShortAverageViewData.Create(average) : null,
                    StudentFinalGrades = StudentFinalGradeViewData.Create(finalGrade, average)
                };
        }
    }

    public class StudentFinalGradeViewData
    {
        public ShortPersonViewData Student { get; set; }
        public StudentAveragesViewData CurrentStudentAverage { get; set; }
        public IList<StudentAveragesViewData> StudentAverages { get; set; }
        public IList<StudentGradingByTypeStatsViewData> StatsByType { get; set; }
        public FinalStudentAttendanceViewData Attendance { get; set; }
        public IList<DisciplineTypeSummaryViewData> Disciplines { get; set; } 

        public static IList<StudentFinalGradeViewData> Create(FinalGradeInfo finalGrade, ChalkableAverage average)
        {
            var res = new List<StudentFinalGradeViewData>();
            var gradeBook = finalGrade.GradeBook;
            foreach (var student in gradeBook.Students)
            {
                var studentFinalGrade = new StudentFinalGradeViewData {Student = ShortPersonViewData.Create(student)};
                if (average != null)
                {
                    var stAvg = gradeBook.Averages.FirstOrDefault(x => x.AverageId == average.AverageId && x.StudentId == student.Id);
                    if (stAvg != null)
                        studentFinalGrade.CurrentStudentAverage = StudentAveragesViewData.Create(stAvg);
                }
                studentFinalGrade.StudentAverages = StudentAveragesViewData.Create(gradeBook.Averages.Where(x=>x.StudentId == student.Id).ToList());
                IList<StudentAnnouncementDetails> stAnns = gradeBook.Announcements.Select(a => a.StudentAnnouncements.First(
                                                                            stAnn => stAnn.StudentId == student.Id)).ToList();
                studentFinalGrade.StatsByType = StudentGradingByTypeStatsViewData.Create(gradeBook.Announcements, stAnns);
                var currentStAttendance = finalGrade.Attendances.FirstOrDefault(x => x.StudentId == student.Id);
                studentFinalGrade.Attendance = FinalStudentAttendanceViewData.Create(currentStAttendance, finalGrade.Attendances);
                var disciplines = finalGrade.Disciplines.Where(x => x.StudentId == student.Id).ToList();
                studentFinalGrade.Disciplines = disciplines.Select(x => new DisciplineTypeSummaryViewData
                    {
                        Count = x.Occurrences,
                        Type = DisciplineTypeViewData.Create(x.Infraction)
                    }).ToList();
                res.Add(studentFinalGrade);
            }
            return res;
        }
    }

    public class FinalStudentAttendanceViewData
    {
        public TotalAttendanceViewData TotalStudentAttendance { get; set; }
        public TotalAttendanceViewData TotalClassAttendance { get; set; }

        public static FinalStudentAttendanceViewData Create(FinalStudentAttendance currentStattendance, IList<FinalStudentAttendance> allAttendances)
        {
            var classAttendance = new FinalStudentAttendance
                {
                    Absenses = allAttendances.Average(x => x.Absenses),
                    Presents = allAttendances.Average(x => x.Presents),
                    Tardies = (int) allAttendances.Average(x => x.Tardies)
                };
            return new FinalStudentAttendanceViewData
                {
                    TotalStudentAttendance = currentStattendance != null ? TotalAttendanceViewData.Create(currentStattendance) : null,
                    TotalClassAttendance = TotalAttendanceViewData.Create(classAttendance)
                };
        }
    }

    public class TotalAttendanceViewData
    {
        public int LateCount { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }

        public static TotalAttendanceViewData Create(FinalStudentAttendance studentAttendance)
        {
            return new TotalAttendanceViewData
                {
                    AbsentCount = (int) studentAttendance.Absenses,
                    LateCount = studentAttendance.Tardies,
                    PresentCount = (int) studentAttendance.Presents,
                };
        }
    }

    public class StudentGradingByTypeStatsViewData
    {
        public int ClassAnnouncementTypeId { get; set; }
        public string ClassAnnouncementTypeName { get; set; }
        public IList<StudentGradingStatsViewData> StudentGradingStats { get; set; }

        public static IList<StudentGradingByTypeStatsViewData> Create(IList<AnnouncementDetails> announcements
            , IList<StudentAnnouncementDetails> studentAnnouncements)
        {
            var res = new List<StudentGradingByTypeStatsViewData>();
            announcements = announcements.Where(x => x.ClassAnnouncementTypeRef.HasValue).ToList();
            var dicbyType = announcements.GroupBy(x => x.ClassAnnouncementTypeRef).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var typeAnns in dicbyType)
            {
                var ann = typeAnns.Value.First();
                res.Add(new StudentGradingByTypeStatsViewData
                    {
                        ClassAnnouncementTypeId = ann.ClassAnnouncementTypeRef.Value,
                        ClassAnnouncementTypeName = ann.ClassAnnouncementTypeName,
                        StudentGradingStats = StudentGradingStatsViewData.Create(typeAnns.Value, studentAnnouncements)
                    });
            }
            return res;
        }

    }
    public class StudentGradingStatsViewData
    {
        public DateTime Date { get; set; }
        public IList<ShortAnnouncementGradeViewData> AnnouncementGrades { get; set; }
        public decimal? Grade { get; set; }

        public static IList<StudentGradingStatsViewData> Create(IList<AnnouncementDetails> announcements
            , IList<StudentAnnouncementDetails> studentAnnouncements)
        {
            var res = new List<StudentGradingStatsViewData>();
            studentAnnouncements = studentAnnouncements.Where(x => x.NumericScore.HasValue).ToList();
            announcements = announcements.Where(x => studentAnnouncements.Any(y => y.AnnouncementId == x.Id)).ToList();
            var dicDateAnns = announcements.GroupBy(x => x.Expires).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var dateAnn in dicDateAnns)
            {
                var anns = dateAnn.Value;
                var stAnns = studentAnnouncements.Where(x => anns.Any(y => y.Id == x.AnnouncementId)).ToList();
                var stIds = stAnns.GroupBy(x => x.StudentId).Select(x => x.Key).ToList();
                var item = new StudentGradingStatsViewData
                    {
                        Date = dateAnn.Key,
                        AnnouncementGrades = dateAnn.Value.Select(x => ShortAnnouncementGradeViewData.Create(x, stAnns, stIds)).ToList()
                    };
                if (stAnns.Any())
                    item.Grade = (stAnns.Max(x => x.NumericScore) + stAnns.Min(x => x.NumericScore)) / 2;
                
                res.Add(item);
            }
            return res;
        } 
    }
}