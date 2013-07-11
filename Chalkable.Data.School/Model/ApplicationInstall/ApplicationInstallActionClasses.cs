using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstallActionClasses
    {
        public Guid Id { get; set; }
        public Guid ClassRef { get; set; }
        public Guid AppInstallActionRef { get; set; }
    }
}
