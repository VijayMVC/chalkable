using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiSeatingChartStorage:BaseDemoStorage<int, SeatingChart>
    {
        public DemoStiSeatingChartStorage(DemoStorage storage)
            : base(storage)
        {

        }

        public SeatingChart GetChart(int classId, int markingPeriodId)
        {
            throw new NotImplementedException();
        }

        public void UpdateChart(int classId, int markingPeriodId, SeatingChart seatingChart)
        {
            throw new NotImplementedException();
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
