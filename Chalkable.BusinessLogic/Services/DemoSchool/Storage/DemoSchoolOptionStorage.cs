using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolOptionStorage:BaseDemoIntStorage<SchoolOption>
    {
        public DemoSchoolOptionStorage(DemoStorage storage)
            : base(storage, x => x.Id)
        {
        }

        public override void Setup()
        {
            Add(new SchoolOption
            {
                Id = DemoSchoolConstants.SchoolId,
                AllowSectionAverageModification = true,
                DefaultCombinationIndex = 1,
                AllowScoreEntryForUnexcused = true,
                DisciplineOverwritesAttendance = false,
                AllowDualEnrollment = true,
                AveragingMethod = "A",
                CategoryAveraging = false,
                StandardsCalculationMethod = "A",
                StandardsCalculationRule = "G",
                StandardsCalculationWeightMaximumValues = false,
                LockCategories = false,
                IncludeReportCardCommentsInGradebook = false,
                MergeRostersForAttendance = true
            });
        }
    }
}
