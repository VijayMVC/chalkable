using System.Collections.Generic;
using System.Linq;
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
        public IList<StudentClassAttendanceOldViewData> NotSeatingStudents { get; set; }
        public bool IsScheduled { get; set; }
        public bool IsDailyAttendancePeriod { get; set; }

        public static AttendanceSeatingChartViewData Create(SeatingChartInfo seatingChart
            , IList<StudentClassAttendanceOldViewData> classAttendance, IList<StudentDetails> students)
        {
            var res = new AttendanceSeatingChartViewData
                {
                    Columns = seatingChart.Columns,
                    Rows = seatingChart.Rows,
                    SeatingList = new List<IList<AttendanceSeatingChartItemViewData>>(),
                    NotSeatingStudents = new List<StudentClassAttendanceOldViewData>(),
                    IsScheduled = classAttendance.Count > 0,
                    IsDailyAttendancePeriod = classAttendance.Count > 0 && classAttendance.First().IsDailyAttendancePeriod
            };
            var notSeatingStudents = students.Where(x => seatingChart.SeatingList.All(y => y.All(z => x.Id != z.StudentId))).ToList();
            foreach (var notSeatingStudent in notSeatingStudents)
            {
                var classAtt = classAttendance.FirstOrDefault(x => x.Student.Id == notSeatingStudent.Id) ??
                    new StudentClassAttendanceOldViewData { Student = StudentViewData.Create(notSeatingStudent) };
                res.NotSeatingStudents.Add(classAtt);
            }
            
            classAttendance.Where(x => seatingChart.SeatingList.All(y => y.All(z => x.Student.Id != z.StudentId))).ToList();
            foreach (var seats in seatingChart.SeatingList)
            {
                var seatingItems = new List<AttendanceSeatingChartItemViewData>();
                foreach (var seat in seats)
                {
                    StudentClassAttendanceOldViewData classAtt = null;
                    var student = students.FirstOrDefault(x => x.Id == seat.StudentId);
                    if (student != null)
                        classAtt = classAttendance.FirstOrDefault(x => x.Student.Id == student.Id) ??
                                   new StudentClassAttendanceOldViewData { Student = StudentViewData.Create(student) };
                    
                    seatingItems.Add(AttendanceSeatingChartItemViewData.Create(seat, classAtt));
                }
                res.SeatingList.Add(seatingItems);
            }
            return res;
        }
    }

    public class AttendanceSeatingChartItemViewData : BaseChartItemViewData<StudentClassAttendanceOldViewData>
    {
        public static AttendanceSeatingChartItemViewData Create(SeatInfo seat, StudentClassAttendanceOldViewData attendance)
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