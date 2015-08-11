using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GradingViewData
{
    public class StudentProfileGradingSummaryViewData
    {

        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public IEnumerable<StudentGradings> GradesByGradingPeriod { get; set; } 
        public StudentViewData Student { get; set; }
        
        public static StudentProfileGradingSummaryViewData Create(StudentDetails student, 
            StudentGrading gradingSummary, GradingPeriod currentGradingPeriod, IList<GradingPeriod> gradingPeriods, IList<Class> classes)
        {

            var gradings = new List<StudentGradings>();
            foreach (var gp in gradingPeriods)
            {
                var avgs = gradingSummary.StudentAverages.Where(x => x.GradingPeriodId == gp.Id && x.IsGradingPeriodAverage).ToList();
                foreach (var avg in avgs)
                {
                    avg.ClassName = classes.First(x => x.Id == avg.ClassId).Name;
                }

                gradings.Add( new StudentGradings
                {
                    GradingPeriod = GradingPeriodViewData.Create(gp),
                    StudentAverages = avgs
                });
            }
            return new StudentProfileGradingSummaryViewData
            {
                Student = StudentViewData.Create(student),
                CurrentGradingPeriod = GradingPeriodViewData.Create(currentGradingPeriod),
                GradesByGradingPeriod = gradings
            };;
        }
    }

    public class StudentGradings
    {
        public GradingPeriodViewData GradingPeriod { get; set; }
        public IEnumerable<ChalkableStudentAverage> StudentAverages { get; set; } 
    }
}