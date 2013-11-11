using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AttendanceReason
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; }
    }

    public class AttendacneLevelReason
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AttendanceReasonId { get; set; }
        public string Level { get; set; }
        public bool IsDefault { get; set; }
    }
}
