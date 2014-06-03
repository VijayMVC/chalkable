using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class SeatingChartInfo
    {
        public byte Columns { get; set; }
        public byte Rows { get; set; }
        public IList<IList<SeatInfo>> SeatingList { get; set; }
        public int ClassId { get; set; }

        public static SeatingChartInfo Create(SeatingChart seatingChart)
        {
            var res = new SeatingChartInfo
                {
                    Columns = seatingChart.Columns,
                    Rows = seatingChart.Rows,
                    ClassId = seatingChart.SectionId,
                    SeatingList = new List<IList<SeatInfo>>()
                };
            var index = 0;
            for (byte row = 1; row <= res.Rows; row++)
            {
                var seats = new List<SeatInfo>();
                for (byte column = 1; column <= res.Columns; column++)
                {
                    var seatInfo = new SeatInfo { Row = row, Column = column, Index = index };
                    var seat = seatingChart.Seats.FirstOrDefault(x => x.Column == column && x.Row == row);
                    if (seat != null)
                        seatInfo.StudentId = seat.StudentId;
                    seats.Add(seatInfo);
                    index++;
                }
                res.SeatingList.Add(seats);
            }
            return res;
        }
    }
    public class SeatInfo
    {
        public int Index { get; set; }
        public byte Column { get; set; }
        public byte Row { get; set; }
        public int? StudentId { get; set; }
    }
}
