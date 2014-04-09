using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AttendancesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
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
        public bool IsScheduled { get; set; }

        public static AttendanceSeatingChartViewData Create(SeatingChartInfo seatingChart
            , IList<ClassAttendanceViewData> classAttendance, IList<Person> students)
        {
            var res = new AttendanceSeatingChartViewData
                {
                    Columns = seatingChart.Columns,
                    Rows = seatingChart.Rows,
                    SeatingList = new List<IList<AttendanceSeatingChartItemViewData>>(),
                    NotSeatingStudents = new List<ClassAttendanceViewData>(),
                    IsScheduled = classAttendance.Count > 0
                };
            var notSeatingStudents = students.Where(x => seatingChart.SeatingList.All(y => y.All(z => x.Id != z.StudentId))).ToList();
            foreach (var notSeatingStudent in notSeatingStudents)
            {
                var classAtt = classAttendance.FirstOrDefault(x => x.Student.Id == notSeatingStudent.Id) ??
                    new ClassAttendanceViewData {Student = ShortPersonViewData.Create(notSeatingStudent)};
                res.NotSeatingStudents.Add(classAtt);
            }
            
            classAttendance.Where(x => seatingChart.SeatingList.All(y => y.All(z => x.Student.Id != z.StudentId))).ToList();
            foreach (var seats in seatingChart.SeatingList)
            {
                var seatingItems = new List<AttendanceSeatingChartItemViewData>();
                foreach (var seat in seats)
                {
                    ClassAttendanceViewData classAtt = null;
                    var student = students.FirstOrDefault(x => x.Id == seat.StudentId);
                    if (student != null)
                        classAtt = classAttendance.FirstOrDefault(x => x.Student.Id == student.Id) ??
                                   new ClassAttendanceViewData {Student = ShortPersonViewData.Create(student)};
                    
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