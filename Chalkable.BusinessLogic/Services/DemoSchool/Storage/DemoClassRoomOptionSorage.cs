using System;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassRoomOptionStorage:BaseDemoIntStorage<ClassroomOption>
    {
        public DemoClassRoomOptionStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }
    
        public override void Setup()
        {
            Add(new ClassroomOption()
            {
                SeatingChartColumns = 3,
                SeatingChartRows = 3,
                AveragingMethod = "P",
                DefaultActivitySortOrder = "A",
                StandardsCalculationMethod = "A",
                StandardsCalculationRule = "G",
                DisplayStudentAverage = true
            });
            Add(new ClassroomOption()
            {
                SeatingChartColumns = 3,
                SeatingChartRows = 3,
                AveragingMethod = "P",
                DefaultActivitySortOrder = "A",
                StandardsCalculationMethod = "A",
                StandardsCalculationRule = "G",
                DisplayStudentAverage = true
            });

        }

        public ClassroomOption GetByClassId(int classId)
        {
            throw new NotImplementedException();
        }
    }
}
