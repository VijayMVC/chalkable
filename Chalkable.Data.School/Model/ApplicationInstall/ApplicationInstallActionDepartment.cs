using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public  class ApplicationInstallActionDepartment
    {
        public Guid Id { get; set; }
        public Guid DepartmentRef { get; set; }
        public Guid AppInstallActionRef { get; set; }
    }
}
