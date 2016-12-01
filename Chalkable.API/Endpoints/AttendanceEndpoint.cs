using System;
using System.Threading.Tasks;
using Chalkable.API.Models.StudentAttendance;

namespace Chalkable.API.Endpoints
{
    public class AttendanceEndpoint : Base
    {
        public AttendanceEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task<StudentDateAttendance> GetStudentAttendance(int studentId, DateTime? date)
        {
            var url = "/Attendance/StudentAttendance.json";
            return await Connector.Get<StudentDateAttendance>($"{url}?studentId={studentId}&date={date}");
        }
    }
}
