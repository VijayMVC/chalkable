using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class SchoolOptionAdapter : SyncModelAdapter<SchoolOption>
    {
        public SchoolOptionAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.SchoolOption Selector(SchoolOption schoolOption)
        {
            return new Data.School.Model.SchoolOption
            {
                Id = schoolOption.SchoolID,
                AllowDualEnrollment = schoolOption.AllowDualEnrollment,
                AllowScoreEntryForUnexcused = schoolOption.AllowScoreEntryForUnexcused,
                AllowSectionAverageModification = schoolOption.AllowSectionAverageModification,
                AveragingMethod = schoolOption.AveragingMethod,
                BaseHoursOffset = schoolOption.BaseHoursOffset,
                BaseMinutesOffset = schoolOption.BaseMinutesOffset,
                CategoryAveraging = schoolOption.CategoryAveraging,
                CompleteStudentScheduleDefinition = schoolOption.CompleteStudentScheduleDefinition,
                DefaultCombinationIndex = schoolOption.DefaultCombinationIndex,
                DisciplineOverwritesAttendance = schoolOption.DisciplineOverwritesAttendance,
                EarliestPaymentDate = schoolOption.EarliestPaymentDate,
                IncludeReportCardCommentsInGradebook = schoolOption.IncludeReportCardCommentsInGradebook,
                LockCategories = schoolOption.LockCategories,
                MergeRostersForAttendance = schoolOption.MergeRostersForAttendance,
                NextReceiptNumber = schoolOption.NextReceiptNumber,
                ObservesDst = schoolOption.ObservesDst,
                StandardsCalculationMethod = schoolOption.StandardsCalculationMethod,
                StandardsCalculationRule = schoolOption.StandardsCalculationRule,
                StandardsCalculationWeightMaximumValues = schoolOption.StandardsCalculationWeightMaximumValues,
                StandardsGradingScaleRef = schoolOption.StandardsGradingScaleID,
                TimeZoneName = schoolOption.TimeZoneName
            };
        }

        protected override void InsertInternal(IList<SchoolOption> entities)
        {
            var res = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SchoolService.AddSchoolOptions(res);
        }

        protected override void UpdateInternal(IList<SchoolOption> entities)
        {
            var res = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SchoolService.EditSchoolOptions(res);
        }

        protected override void DeleteInternal(IList<SchoolOption> entities)
        {
            var schoolOptions = entities.Select(x => new Data.School.Model.SchoolOption { Id = x.SchoolID }).ToList();
            ServiceLocatorSchool.SchoolService.DeleteSchoolOptions(schoolOptions);
        }
    }
}