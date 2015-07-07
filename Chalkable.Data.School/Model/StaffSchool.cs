using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StaffSchool
    {
        [PrimaryKeyFieldAttr]
        public int StaffRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int SchoolRef { get; set; }
    }
}
