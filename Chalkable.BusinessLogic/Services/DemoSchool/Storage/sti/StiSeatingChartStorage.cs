using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //todo: filter by marking period
            if (data.Count(x => x.Value.Key == markingPeriodId && x.Value.Value.SectionId == classId) == 0)
            {
                Add(new KeyValuePair<int, SeatingChart>(markingPeriodId, seatingChart));
            }
            var item = data.First(x => x.Value.Value.SectionId == classId && x.Value.Key == markingPeriodId).Key;
            data[item] = new KeyValuePair<int, SeatingChart>(markingPeriodId, seatingChart);
        }

        public void Setup()
        {

            var seats = new List<Seat>();


            for (var i = 0; i < 3; ++i)
            {
                for (var  j = 0; j < 3; ++j)
                {
                    seats.Add(new Seat
                    {
                        Column = (byte?) (j + 1),
                        Row = (byte?) (i + 1)
                    });
                }
            }

            seats[0].StudentId = 1196;

            Add(new KeyValuePair<int, SeatingChart>(1, new SeatingChart()
            {
                SectionId = 1,
                Columns = 3,
                Rows = 3,
                Seats = seats
            }));

            Add(new KeyValuePair<int, SeatingChart>(2, new SeatingChart()
            {
                SectionId = 1,
                Columns = 3,
                Rows = 3,
                Seats = seats
            }));

            Add(new KeyValuePair<int, SeatingChart>(1, new SeatingChart()
            {
                SectionId = 2,
                Columns = 3,
                Rows = 3,
                Seats = seats
            }));

            Add(new KeyValuePair<int, SeatingChart>(2, new SeatingChart()
            {
                SectionId = 2,
                Columns = 3,
                Rows = 3,
                Seats = seats
            }));

        }
    }
}
