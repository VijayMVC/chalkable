using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Web.Models.AttendancesViewData;

namespace Chalkable.Web.Models
{
    public class BaseChartViewData
    {
        public byte Columns { get; set; }
        public byte Rows { get; set; }
    }
    


    public class BaseChartItemViewData<TInfo>
    {
        public int Index { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public TInfo Info { get; set; }
    }

    public class AttendanceSeatingChartViewData
    {
        public byte Columns { get; set; }
        public byte Rows { get; set; }
        public IList<IList<AttendanceSeatingChartItemViewData>> SeatingList { get; set; } 
        public IList<ClassAttendanceViewData> NotSeatingStudents { get; set; } 

        public static AttendanceSeatingChartViewData Create(SeatingChartInfo seatingChart,
                                                            IList<ClassAttendanceViewData> classAttendance)
        {
            var res = new AttendanceSeatingChartViewData
                {
                    Columns = seatingChart.Columns,
                    Rows = seatingChart.Rows,
                    SeatingList = new List<IList<AttendanceSeatingChartItemViewData>>(),
                    NotSeatingStudents = classAttendance.Where(x => seatingChart.SeatsList.All(y => y.All(z=>x.Student.Id != z.StudentId))).ToList()
                };
            foreach (var seats in seatingChart.SeatsList)
            {
                var seatingItems = new List<AttendanceSeatingChartItemViewData>();
                foreach (var seat in seats)
                {
                    var classAtt = classAttendance.FirstOrDefault(x => x.Student.Id == seat.StudentId);
                    seatingItems.Add(AttendanceSeatingChartItemViewData.Create(seat, classAtt));
                }
                res.SeatingList.Add(seatingItems);
            }
            return res;
        }
    }

    public class AttendanceSeatingChartItemViewData : BaseChartItemViewData<ClassAttendanceViewData>
    {
        public static AttendanceSeatingChartItemViewData Create(SeatInfo seat, ClassAttendanceViewData attendance)
        {
            return new AttendanceSeatingChartItemViewData
                {
                    Column = seat.Column,
                    Index = seat.Index,
                    Row = seat.Row,
                    Info = attendance
                };
        }
    }
}