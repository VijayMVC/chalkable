using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class ClassroomOptionAdapter : SyncModelAdapter<ClassroomOption>
    {
        public ClassroomOptionAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.ClassroomOption Selector(ClassroomOption x)
        {
            return new Data.School.Model.ClassroomOption
            {
                Id = x.SectionID,
                DefaultActivitySortOrder = x.DefaultActivitySortOrder,
                GroupByCategory = x.GroupByCategory,
                AveragingMethod = x.AveragingMethod,
                CategoryAveraging = x.CategoryAveraging,
                IncludeWithdrawnStudents = x.IncludeWithdrawnStudents,
                DisplayStudentAverage = x.DisplayStudentAverage,
                DisplayTotalPoints = x.DisplayTotalPoints,
                RoundDisplayedAverages = x.RoundDisplayedAverages,
                DisplayAlphaGrade = x.DisplayAlphaGrade,
                DisplayStudentNames = x.DisplayStudentNames,
                DisplayMaximumScore = x.DisplayMaximumScore,
                StandardsGradingScaleRef = x.StandardsGradingScaleID,
                StandardsCalculationMethod = x.StandardsCalculationMethod,
                StandardsCalculationRule = x.StandardsCalculationRule,
                StandardsCalculationWeightMaximumValues = x.StandardsCalculationWeightMaximumValues,
                DefaultStudentSortOrder = x.DefaultStudentSortOrder,
                SeatingChartRows = x.SeatingChartRows,
                SeatingChartColumns = x.SeatingChartColumns
            };
        }

        protected override void InsertInternal(IList<ClassroomOption> entities)
        {
            var cro = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassroomOptionService.Add(cro);
        }

        protected override void UpdateInternal(IList<ClassroomOption> entities)
        {
            var cro = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassroomOptionService.Edit(cro);
        }

        protected override void DeleteInternal(IList<ClassroomOption> entities)
        {
            var classroomOptions = entities.Select(x => new Data.School.Model.ClassroomOption { Id = x.SectionID }).ToList();
            ServiceLocatorSchool.ClassroomOptionService.Delete(classroomOptions);
        }
    }
}