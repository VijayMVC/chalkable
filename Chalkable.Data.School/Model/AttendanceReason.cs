using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class AttendanceReason
    {
        public int Id { get; set; }
        public AttendanceTypeEnum AttendanceType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
