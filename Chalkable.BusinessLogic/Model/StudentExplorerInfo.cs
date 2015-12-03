using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Logic.Comperators;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentExplorerInfo
    {
        public StudentDetails Student { get; set; }
        public IList<StudentClassExplorerInfo> ClassesGradingInfo { get; set; }

        public static StudentExplorerInfo Create(StudentDetails student, IList<ClassDetails> classDetailses, 
            IList<StudentAverage> mostRecentAvgWithGrades, IList<StandardScore> standardScores  
            , IList<AnnouncementComplex> announcements, IList<Standard> standards)
        {
            var res = new StudentExplorerInfo { Student = student};
            var classesInfo = StudentClassExplorerInfo.Create(classDetailses, mostRecentAvgWithGrades, standardScores, announcements, standards);
            res.ClassesGradingInfo = classesInfo;
            return res;
        }
    }

    public class StudentClassExplorerInfo
    {
        public Class ClassInfo { get; set; }
        public decimal? Avg { get; set; }
        public IList<GradingStandardInfo> Standards { get; set; }
        public AnnouncementComplex MostImportantAnnouncement { get; set; }
        public IList<ChalkableStudentAverage> StudentAverages { get; set; }

        public static IList<StudentClassExplorerInfo> Create(IList<ClassDetails> classDetailses, IList<StudentAverage> mostRecentAveragesWithGrade
                                                             , IList<StandardScore> standardScores
                                                             , IList<AnnouncementComplex> announcements, IList<Standard> standards)
        {
            var res = new List<StudentClassExplorerInfo>();
            foreach (var classDetailse in classDetailses)
            {
                var avg = mostRecentAveragesWithGrade.FirstOrDefault(x => x.SectionId == classDetailse.Id);
                var gradeValue = avg != null ? (avg.EnteredNumericAverage ?? avg.CalculatedNumericAverage) : null;
                var stScores = standardScores.Where(x => x.SectionId == classDetailse.Id).ToList();
                var importantItem = announcements.FirstOrDefault(x => x.ClassAnnouncementData.ClassRef == classDetailse.Id);
                res.Add(Create(classDetailse, gradeValue, stScores, standards, importantItem));                   
            }
            return OrderClassGradeInfo(res);
        }
 
        public static StudentClassExplorerInfo Create(Class classInfo, decimal? avg, IList<StandardScore> standardScores
            , IList<Standard> standards, AnnouncementComplex importantAnnouncement)
        {
            var res = new StudentClassExplorerInfo
                {
                    ClassInfo = classInfo,
                    MostImportantAnnouncement = importantAnnouncement,
                    Avg = avg,
                    Standards = OrderGradingStandards(GradingStandardInfo.Create(standardScores, standards))
                };
            return res;
        }

        private static IList<GradingStandardInfo> OrderGradingStandards(IList<GradingStandardInfo> gradingStandards)
        {
            if (gradingStandards.Count > 0)
            {
                gradingStandards = gradingStandards.OrderBy(x => x.Standard.Name).ToList();
                var graded = gradingStandards.Where(x => !string.IsNullOrEmpty(x.AlphaGradeName)).ToList();
                if (graded.Count > 0)
                    graded =
                        graded.OrderBy(x => x.AlphaGradeName, new AlphaGradeNameComperator())
                            .ThenBy(x => x.Standard.Name)
                            .ToList();
                return graded.Concat(gradingStandards.Where(x => string.IsNullOrEmpty(x.AlphaGradeName))).ToList();
            }
            return new List<GradingStandardInfo>();
        } 

        private static IList<StudentClassExplorerInfo> OrderClassGradeInfo(
            IList<StudentClassExplorerInfo> classGradeInfo)
        {
            classGradeInfo = classGradeInfo.OrderBy(x => x.ClassInfo.Name).ToList();
            var notGraded = classGradeInfo.Where(x => !x.Avg.HasValue).ToList();
            var graded = classGradeInfo.Where(x => x.Avg.HasValue).OrderByDescending(x=>x.Avg).ThenBy(x=>x.ClassInfo.Name).ToList();
            return graded.Concat(notGraded).ToList();
        }
    }
}
