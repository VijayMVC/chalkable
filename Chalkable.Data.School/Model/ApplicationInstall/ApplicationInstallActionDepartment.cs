using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public  class ApplicationInstallActionDepartment
    {
        public int Id { get; set; }
        public Guid DepartmentRef { get; set; }
        public int AppInstallActionRef { get; set; }
    }
}
