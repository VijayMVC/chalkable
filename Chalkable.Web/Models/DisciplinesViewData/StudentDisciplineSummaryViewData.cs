using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.GradingViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class StudentDisciplineSummaryViewData
    {
        public ShortPersonViewData Student { get; set; }
        public IList<StudentDisciplineBoxViewData> DisciplineBoxes { get; set; }
        public IList<GradingPeriodViewData> Gradingperiods { get; set; }
        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public string Summary { get; set; }

        protected StudentDisciplineSummaryViewData(Student student, IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions)
        {
            Student = StudentProfileViewData.Create(student, customAlerts, healthConditions);
        }

        public static StudentDisciplineSummaryViewData Create(Student student, IList<InfractionSummaryInfo> infractionsSummary
            , GradingPeriod currentGradingPeriod, IList<GradingPeriod> gradingPeriods, IList<StudentCustomAlertDetail> customAlerts
            , IList<StudentHealthCondition> healthConditions)
        {
            
            string summary = $"{student.FirstName} is doing great. No discipline issues.";
            if (infractionsSummary.Sum(x => x.Occurrences) > 0)
                summary = $"{student.FirstName} is having disciplinary issues";
            var disciplineBoxes = infractionsSummary.Select(StudentDisciplineBoxViewData.Create).ToList();
            var res = new StudentDisciplineSummaryViewData(student, customAlerts, healthConditions)
                          {
                              CurrentGradingPeriod = GradingPeriodViewData.Create(currentGradingPeriod),
                              Summary = summary,
                              DisciplineBoxes = disciplineBoxes,
                              Gradingperiods = GradingPeriodViewData.Create(gradingPeriods)
                          };
            return res;
        }
    }


    public class StudentDisciplineBoxViewData : HoverBoxesViewData<StudentDisciplineHoverViewData>
    {
        public string Name { get; set; }
        public static StudentDisciplineBoxViewData Create(InfractionSummaryInfo infractionSummary)
        {
            var isPassing = false; //TODO: calc when is passing
            return new StudentDisciplineBoxViewData
            {
                Hover = new List<StudentDisciplineHoverViewData>(),
                IsPassing = isPassing,
                Title = infractionSummary.Occurrences.ToString(),
                Name = infractionSummary.Infraction.Name
            };
        }
    }

    public class StudentDisciplineHoverViewData
    {
        public int Value { get; set; }
        public string ClassName { get; set; }
        public const int HOVER_DATA_COUNT = 4;
        private const string OTHER = "Other";

        public static StudentDisciplineHoverViewData Create(List<ClassDisciplineDetails> disciplines)
        {

            var res = new StudentDisciplineHoverViewData
            {
                Value = disciplines.Sum(x=>x.Infractions.Count),
                ClassName = disciplines.First().Class.Name
            };
            return res;
        }
        public static IList<StudentDisciplineHoverViewData> Create(Dictionary<int, List<ClassDisciplineDetails>> disciplinesDictionary)
        {
            var res = disciplinesDictionary.Select(x => Create(x.Value)).OrderByDescending(x => x.Value)
                .ToList();
            if (res.Count > HOVER_DATA_COUNT)
            {
                var rest = res.Skip(HOVER_DATA_COUNT).Sum(x => x.Value);

                res = res.Take(HOVER_DATA_COUNT).ToList();
                res.Add(new StudentDisciplineHoverViewData
                {
                    ClassName = OTHER,
                    Value = rest
                });
            }
            return res;
        }
    }

}