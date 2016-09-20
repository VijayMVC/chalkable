using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class ApplicationSchoolOption
    {
        [PrimaryKeyFieldAttr]
        public Guid ApplicationRef { get; set; }
        [PrimaryKeyFieldAttr]
        public Guid SchoolRef { get; set; }
        public bool Banned { get; set; }
    }
}
