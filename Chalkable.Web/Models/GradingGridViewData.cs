using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{

    public class GradingGridSummaryViewData
    {
        public GradingPeriodViewData GradingPeriod { get; set; }
        public int? Avg { get; set; }

        protected GradingGridSummaryViewData(GradingPeriod gradingPeriod, int? avg)
        {
            Avg = avg;
            GradingPeriod = GradingPeriodViewData.Create(gradingPeriod);
        }
        public static GradingGridSummaryViewData Create(GradingPeriod gradingPeriod, int? avg)
        {
            return new GradingGridSummaryViewData(gradingPeriod, avg);
        }
    }

    public class GradingGridsViewData
    {
        public GradingGridViewData CurrentGradingGrid { get; set; }
        public IList<GradingPeriodViewData> GradingPeriods { get; set; }
        public IList<AnnouncementStandardViewData> Standards { get; set; }
        public IList<ClassAnnouncementTypeViewData> ClassAnnouncementTypes { get; set; }

        public static GradingGridsViewData Create(ChalkableGradeBook grid, IList<GradingPeriodDetails> gradingPeriods
            , IList<Standard> standards, IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            return new GradingGridsViewData
                {
                    CurrentGradingGrid = grid != null ? GradingGridViewData.Create(grid) : null,
                    GradingPeriods = gradingPeriods.Select(GradingPeriodViewData.Create).ToList(),
                    Standards = AnnouncementStandardViewData.Create(standards),
                    ClassAnnouncementTypes = ClassAnnouncementTypeViewData.Create(classAnnouncementTypes)
                };
        }
    }

    public class GradingGridViewData : GradingGridSummaryViewData
    {
        protected GradingGridViewData(ChalkableGradeBook gradeBook)
            : base(gradeBook.GradingPeriod, gradeBook.Avg)
        {
        }

        public IList<GradeStudentViewData> Students { get; set; }
        public IList<ShortAnnouncementGradeViewData> GradingItems { get; set; }
        public IList<StudentTotalAveragesViewData> TotalAvarages { get; set; } 

        public bool DisplayAlphaGrades { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }

        public IList<TotalPointViewData> TotalPoints { get; set; }
        
        public static GradingGridViewData Create(ChalkableGradeBook gradeBook)
        {
            var res = new GradingGridViewData(gradeBook)
                {
                    Students = new List<GradeStudentViewData>(),
                    DisplayAlphaGrades = gradeBook.Options.DisplayAlphaGrades,
                    DisplayStudentAverage = gradeBook.Options.DisplayStudentAverage,
                    DisplayTotalPoints = gradeBook.Options.DisplayTotalPoints,
                    IncludeWithdrawnStudents = gradeBook.Options.IncludeWithdrawnStudents,
                };
            foreach (var student in gradeBook.Students)
            {
                var ann = gradeBook.Announcements.FirstOrDefault();
                bool isWithdrawn = ann != null && ann.StudentAnnouncements.FirstOrDefault() != null
                                   && ann.StudentAnnouncements.First().Withdrawn;
                res.Students.Add(GradeStudentViewData.Create(student, isWithdrawn));

            }
            var stIds = res.Students.Select(x => x.StudentInfo.Id).ToList();
            res.TotalAvarages = StudentTotalAveragesViewData.Create(gradeBook.Averages, stIds);
            if (gradeBook.Options.DisplayTotalPoints && gradeBook.Announcements.Count > 0)
            {
                res.TotalPoints = new List<TotalPointViewData>();
                foreach (var stId in stIds)
                {
                    var scoredAnns = gradeBook.Announcements.Where(x =>!x.Dropped && x.StudentAnnouncements.Any(y => y.Student.Id == stId 
                                                                    && y.IncludeInTotalPoint)).ToList();
                    var totalPoint = scoredAnns.Sum(x => x.StudentAnnouncements.First(y => y.Student.Id == stId).NumericScore ?? 0);
                    var maxTotalPoint = scoredAnns.Where(x => x.MaxScore.HasValue).Sum(x => x.MaxScore.Value);
                    res.TotalPoints.Add(TotalPointViewData.Create((int)totalPoint, (int)maxTotalPoint));
                }

            }
            res.GradingItems = gradeBook.Announcements
                                        .Select(x => ShortAnnouncementGradeViewData.Create(x, x.StudentAnnouncements, stIds))
                                        .ToList();

            return res;
        }
        public static IList<GradingGridViewData> Create(IList<ChalkableGradeBook> gradeBooks)
        {
            return gradeBooks.Select(Create).ToList();
        }
    }

    public class TotalPointViewData
    {
        public int TotalPoint { get; set; }
        public int MaxTotalPoint { get; set; }

        public static TotalPointViewData Create(int totalPoint, int maxTotalPoint)
        {
            return new TotalPointViewData {TotalPoint = totalPoint, MaxTotalPoint = maxTotalPoint};
        }
    }

    public class StudentTotalAveragesViewData
    {
        public IList<StudentAveragesViewData> Averages { get; set; }
        public int AverageId { get; set; }
        public string AverageName { get; set; }
        public bool IsGradingPeriodAverage { get; set; }
        public decimal? TotalAverage { get; set; }

        public static StudentTotalAveragesViewData Create(IList<ChalkableStudentAverage> averages)
        {
            var res = new StudentTotalAveragesViewData
                {
                    Averages = averages.Select(StudentAveragesViewData.Create).ToList(),
                };
            if (averages.Count > 0)
            {
                res.AverageId = averages.First().AverageId;
                res.AverageName = averages.First().AverageName;
                res.IsGradingPeriodAverage = averages.First().IsGradingPeriodAverage;
                res.TotalAverage = averages.Average(x => (x.EnteredAvg ?? x.CalculatedAvg));
            }
            return res;
        }

        public static IList<StudentTotalAveragesViewData> Create(IList<ChalkableStudentAverage> averages,
                                                                 IList<int> studentIds)
        {
            var res = new List<StudentTotalAveragesViewData>();
            var avgDic = averages.GroupBy(x => x.AverageId).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var kv in avgDic)
            {
                var stAvarages = studentIds.Select(studentId => kv.Value.FirstOrDefault(x => x.StudentId == studentId))
                                    .Where(stAvg => stAvg != null).ToList();

                res.Add(Create(stAvarages));
            }
            return res;
        } 
    }

    public class ShortAverageViewData
    {
        public int AverageId { get; set; }
        public string AverageName { get; set; }      
  
        public static ShortAverageViewData Create(ChalkableAverage average)
        {
            return new ShortAverageViewData
                {
                    AverageId = average.AverageId,
                    AverageName = average.AverageName
                };
        }
        public static IList<ShortAverageViewData> Create(IList<ChalkableAverage> averages)
        {
            return averages.Select(Create).ToList();
        }
    }

    public class StudentAveragesViewData : ShortAverageViewData
    {
        public decimal? CalculatedAvg { get; set; }
        public string CalculatedAlphaGrade { get; set; }
        public int StudentId { get; set; }
        public decimal? EnteredAvg { get; set; }
        public string EnteredAlphaGrade { get; set; }
        public bool IsGradingPeriodAverage { get; set; }
        public bool IsExempt { get; set; }
        public bool MayBeExempt { get; set; }
        public IList<StudentAverageCommentViewData> Codes { get; set; }
        public string Note { get; set; }

        public static StudentAveragesViewData Create(ChalkableStudentAverage studentAverage)
        {
            var res = new StudentAveragesViewData
                {
                    AverageId = studentAverage.AverageId,
                    CalculatedAlphaGrade = studentAverage.CalculatedAlphaGrade != null ? studentAverage.CalculatedAlphaGrade.Name : null,
                    CalculatedAvg = studentAverage.CalculatedAvg,
                    EnteredAlphaGrade = studentAverage.EnteredAlphaGrade != null ? studentAverage.EnteredAlphaGrade.Name : null,
                    EnteredAvg = studentAverage.EnteredAvg,
                    StudentId = studentAverage.StudentId,
                    AverageName = studentAverage.AverageName,
                    IsExempt = studentAverage.Exempt,
                    MayBeExempt = studentAverage.MayBeExempt,
                    Note = studentAverage.Note
                };
            if (studentAverage.Comments != null)
                res.Codes = studentAverage.Comments.Select(StudentAverageCommentViewData.Create).ToList();
            return res;
        }

        public static IList<StudentAveragesViewData> Create(IList<ChalkableStudentAverage> studentAverages)
        {
            return studentAverages.Select(Create).ToList();
        } 
    }

    public class StudentAverageCommentViewData
    {
        public int HeaderId { get; set; }
        public string HeaderName { get; set; }
        public GradingCommentViewData GradingComment { get; set; }

        public static StudentAverageCommentViewData Create(ChalkableStudentAverageComment studentAverageComment)
        {
            var res = new StudentAverageCommentViewData
                {
                    HeaderId = studentAverageComment.HeaderId,
                    HeaderName = studentAverageComment.HeaderText,
                };
            if (studentAverageComment.GradingComment != null)
                res.GradingComment = GradingCommentViewData.Create(studentAverageComment.GradingComment);
            return res;
        }
    }

    public class GradeStudentViewData
    {
        public bool? IsWithDrawn { get; set; }
        public ShortPersonViewData StudentInfo { get; set; }

        public static GradeStudentViewData Create(Person person, bool? isWithDrawn)
        {
            return new GradeStudentViewData {StudentInfo = ShortPersonViewData.Create(person), IsWithDrawn = isWithDrawn};
        }
    }

    public class ShortStudentsAnnouncementsViewData
    {
        public IList<ShortStudentAnnouncementViewData> Items { get; set; }
        public static ShortStudentsAnnouncementsViewData Create(IList<StudentAnnouncementDetails> studentAnnouncements, IList<int> studentIds)
        {
            var res = new ShortStudentsAnnouncementsViewData  { Items = new List<ShortStudentAnnouncementViewData>() };
            foreach (var studentId in studentIds)
            {
                var stAnn = studentAnnouncements.FirstOrDefault(x => x.StudentId == studentId);
                if(stAnn != null)
                    res.Items.Add(ShortStudentAnnouncementViewData.Create(stAnn));
            }
            return res;
        }
    }

    public class ShortAnnouncementGradeViewData : AnnouncementShortViewData
    {
        public ShortStudentsAnnouncementsViewData StudentAnnouncements { get; set; }

        protected ShortAnnouncementGradeViewData(AnnouncementComplex announcement) : base(announcement)
        {
        }

        public static ShortAnnouncementGradeViewData Create(AnnouncementComplex announcement, 
            IList<StudentAnnouncementDetails> studentAnnouncements, IList<int> studentIds)
        {
            studentAnnouncements = studentAnnouncements.Where(x => x.AnnouncementId == announcement.Id).ToList();
            return new ShortAnnouncementGradeViewData(announcement)
                {
                    StudentAnnouncements = ShortStudentsAnnouncementsViewData.Create(studentAnnouncements, studentIds)
                };
        }
    }
}