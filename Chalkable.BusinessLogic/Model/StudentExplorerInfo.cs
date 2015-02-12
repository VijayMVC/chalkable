using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentExplorerInfo
    {
        public StudentDetails Student { get; set; }
        public IList<StudentClassExplorerInfo> ClassesGradingInfo { get; set; }

        public static StudentExplorerInfo Create(StudentDetails student, IList<ClassDetails> classDetailses, StudentExplorerDashboard studentExplorer
            , IList<AnnouncementComplex> announcements, IList<Standard> standards)
        {
            var res = new StudentExplorerInfo { Student = student};
            var classesInfo = StudentClassExplorerInfo.Create(classDetailses, studentExplorer, announcements, standards);
            res.ClassesGradingInfo = classesInfo.OrderBy(x => x.Avg).ToList();
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

        public static IList<StudentClassExplorerInfo> Create(IList<ClassDetails> classDetailses, StudentExplorerDashboard studentExplorer
                                                             , IList<AnnouncementComplex> announcements, IList<Standard> standards)
        {
            var res = new List<StudentClassExplorerInfo>();
            foreach (var classDetailse in classDetailses)
            {
                if (studentExplorer != null)
                {
                    var averages = studentExplorer.Averages.Where(avg=>avg.SectionId == classDetailse.Id).ToList();
                    var standardScores = studentExplorer.Standards.Where(x => x.SectionId == classDetailse.Id).ToList();
                    var importantItem = announcements.FirstOrDefault(x => x.ClassRef == classDetailse.Id);
                    res.Add(Create(classDetailse, averages, standardScores, standards, importantItem));                   
                }
                else res.Add(Create(classDetailse, new List<StudentAverage>(), new List<StandardScore>(), standards, null));

            }
            return res;
        }
 

        public static StudentClassExplorerInfo Create(Class classInfo, IList<StudentAverage> studentAverages, IList<StandardScore> standardScores
            , IList<Standard> standards, Announcement importantAnnouncement)
        {
            var res = new StudentClassExplorerInfo
                {
                    ClassInfo = classInfo,
                    StudentAverages = studentAverages.Select(ChalkableStudentAverage.Create).ToList(),
                };
            var generalAvg = res.StudentAverages.FirstOrDefault(x => x.IsGradingPeriodAverage);
            if (generalAvg != null)
                res.Avg = generalAvg.EnteredAvg ?? generalAvg.CalculatedAvg; 
            var gradingStandardsInfo = new List<GradingStandardInfo>();
            foreach (var standardScore in standardScores)
            {
                var standard = standards.FirstOrDefault(st => st.Id == standardScore.StandardId);
                gradingStandardsInfo.Add(GradingStandardInfo.Create(standardScore, standard));
            }
            res.Standards = gradingStandardsInfo.OrderBy(x => x.NumericGrade).ToList();
            return res;
        }
    }
}
