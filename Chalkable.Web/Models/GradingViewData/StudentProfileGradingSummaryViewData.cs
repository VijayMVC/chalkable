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
        
        public static StudentProfileGradingSummaryViewData Create(Student student, StudentGrading gradingSummary, GradingPeriod currentGradingPeriod
            , IList<GradingPeriod> gradingPeriods, IList<Class> classes, IEnumerable<int> enrolledClassIds
            , IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions)
        {

            var avgs = gradingSummary.StudentAverages.Where(x => enrolledClassIds.Contains(x.ClassId)).ToList();

            var gradings = new List<StudentGradings>();
            foreach (var gp in gradingPeriods)
            {
                var gpAvgs = avgs.Where(x => x.GradingPeriodId == gp.Id).ToList();


                var classAvgs = gpAvgs.GroupBy(x => x.ClassId).Select(x => new StudentClassAvg()
                {
                    Class = classes.First(y => y.Id == x.Key),
                    StudentAverages = x.Where(z => !z.IsGradingPeriodAverage),
                    GradingPeriodAvg = x.First(z => z.IsGradingPeriodAverage)
                });

                gradings.Add( new StudentGradings
                {
                    GradingPeriod = GradingPeriodViewData.Create(gp),
                    ClassAvgs = classAvgs
                });
            }
            return new StudentProfileGradingSummaryViewData
            {
                Student = StudentProfileViewData.Create(student, customAlerts, healthConditions),
                CurrentGradingPeriod = GradingPeriodViewData.Create(currentGradingPeriod),
                GradesByGradingPeriod = gradings
            };;
        }
    }


    public class StudentClassAvg
    {
        public ChalkableStudentAverage GradingPeriodAvg { get; set; }
        public IEnumerable<ChalkableStudentAverage> StudentAverages { get; set; }
        public Class Class { get; set; }
    }

    public class StudentGradings
    {
        public GradingPeriodViewData GradingPeriod { get; set; }
        public IEnumerable<StudentClassAvg> ClassAvgs { get; set; } 
    }
}