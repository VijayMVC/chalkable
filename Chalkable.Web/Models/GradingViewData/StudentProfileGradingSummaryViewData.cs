using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GradingViewData
{
    public class StudentProfileGradingSummaryViewData
    {
        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public IList<StudentGradingsViewData> GradesByGradingPeriod { get; set; } 
        public StudentViewData Student { get; set; }
        
        public static StudentProfileGradingSummaryViewData Create(Student student, StudentGrading gradingSummary, GradingPeriod currentGradingPeriod
            , IList<GradingPeriod> gradingPeriods, IList<Class> classes, IEnumerable<int> enrolledClassIds
            , IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions
            , IList<StudentHealthFormInfo> healthForms)
        {

            var avgs = gradingSummary.StudentAverages.Where(x => enrolledClassIds.Contains(x.ClassId)).ToList();

            var gradings = new List<StudentGradingsViewData>();
            foreach (var gp in gradingPeriods)
            {
                var gpAvgs = avgs.Where(x => x.GradingPeriodId == gp.Id).ToList();
                var classAvgs = gpAvgs.GroupBy(x => x.ClassId)
                    .Select(x => StudentClassAvgViewData.Create(classes.First(y => y.Id == x.Key), x));

                gradings.Add( new StudentGradingsViewData
                {
                    GradingPeriod = GradingPeriodViewData.Create(gp),
                    ClassAvgs = classAvgs.ToList()
                });
            }
            return new StudentProfileGradingSummaryViewData
            {
                Student = StudentProfileViewData.Create(student, customAlerts, healthConditions, healthForms),
                CurrentGradingPeriod = GradingPeriodViewData.Create(currentGradingPeriod),
                GradesByGradingPeriod = gradings.ToList()
            };;
        }
    }


    public class StudentClassAvgViewData
    {
        public StudentAveragesViewData GradingPeriodAvg { get; set; }
        public IList<StudentAveragesViewData> StudentAverages { get; set; }
        public ShortClassViewData Class { get; set; }

        public static StudentClassAvgViewData Create(Class @class, IEnumerable<ChalkableStudentAverage> studentAverages)
        {
            return new StudentClassAvgViewData
            {
                Class = ShortClassViewData.Create(@class),
                StudentAverages = StudentAveragesViewData.Create(studentAverages.Where(z => !z.IsGradingPeriodAverage)),
                GradingPeriodAvg = StudentAveragesViewData.Create(studentAverages.First(z => z.IsGradingPeriodAverage))
            };
        }
    }

    public class StudentGradingsViewData
    {
        public GradingPeriodViewData GradingPeriod { get; set; }
        public IList<StudentClassAvgViewData> ClassAvgs { get; set; } 
    }
}