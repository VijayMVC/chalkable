using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiSeatingChartStorage:BaseDemoIntStorage<KeyValuePair<int, SeatingChart>>
    {
        public DemoStiSeatingChartStorage(DemoStorage storage)
            : base(storage, null, true)
        {

        }

        public SeatingChart GetChart(int classId, int markingPeriodId)
        {
            return data.First(x => x.Value.Key == markingPeriodId && x.Value.Value.SectionId == classId).Value.Value;
        }

        public void UpdateChart(int classId, int markingPeriodId, SeatingChart seatingChart)
        {
            if (data.Count(x => x.Value.Key == markingPeriodId && x.Value.Value.SectionId == classId) == 0)
            {
                Add(new KeyValuePair<int, SeatingChart>(markingPeriodId, seatingChart));
            }
            var item = data.First(x => x.Value.Value.SectionId == classId && x.Value.Key == markingPeriodId).Key;
            data[item] = new KeyValuePair<int, SeatingChart>(markingPeriodId, seatingChart);
        }
    }
}
