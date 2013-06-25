using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class StudentDailyAttendance
    {
        public Guid Id { get; set; }
        public Guid PersonRef { get; set; }
        public DateTime Date { get; set; }
        public int? Arrival { get; set; }
        public int? TimeIn { get; set; }
        public int? TimeOut { get; set; }
    }
}
