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
            //todo: filter by marking period
            return data.First(x => x.Value.SectionId == classId).Value;
        }

        public void UpdateChart(int classId, int markingPeriodId, SeatingChart seatingChart)
        {
            //todo: filter by marking period
            if (data.Count(x => x.Value.SectionId == classId) == 0)
            {
                data.Add(GetNextFreeId(), seatingChart);
            }
            var item = data.First(x => x.Value.SectionId == classId).Key;
            data[item] = seatingChart;
        }

        public override void Setup()
        {

            var seats = new List<Seat>();


            for (var i = 0; i < 3; ++i)
            {
                for (var  j = 0; j < 3; ++j)
                {
                    seats.Add(new Seat()
                    {
                        Column = (byte?) (j + 1),
                        Row = (byte?) (i + 1)
                    });
                }
            }

            seats[0].StudentId = 1196;

            data.Add(GetNextFreeId(), new SeatingChart()
            {
                SectionId = 1,
                Columns = 3,
                Rows = 3,
                Seats = seats
            });

            data.Add(GetNextFreeId(), new SeatingChart()
            {
                SectionId = 2,
                Columns = 3,
                Rows = 3,
                Seats = seats
            });
        }
    }
}
