using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GradingViewData
{
    public class StudentProfileGradingSummaryViewData : StudentViewData
    {

        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public IEnumerable<StudentGradings> GradesByGradingPeriod { get; set; } 
        protected StudentProfileGradingSummaryViewData(StudentDetails person)
            : base(person)
        {
        }

        public static StudentProfileGradingSummaryViewData Create(StudentDetails student, 
            StudentGrading gradingSummary, GradingPeriod currentGradingPeriod, IList<GradingPeriod> gradingPeriods)
        {

            var gradings = new List<StudentGradings>();

            foreach (var gp in gradingPeriods)
            {
                var avgs = gradingSummary.StudentAverages.Where(x => x.GradingPeriodId == gp.Id && x.IsGradingPeriodAverage);
                gradings.Add( new StudentGradings
                {
                    GradingPeriod = GradingPeriodViewData.Create(gp),
                    StudentAverages = avgs
                });
            }

            var res = new StudentProfileGradingSummaryViewData(student)
            {
                CurrentGradingPeriod = GradingPeriodViewData.Create(currentGradingPeriod),
                GradesByGradingPeriod = gradings
            };

            return res;
        }
    }

    public class StudentGradings
    {
        public GradingPeriodViewData GradingPeriod { get; set; }
        public IEnumerable<ChalkableStudentAverage> StudentAverages { get; set; } 
    }
}