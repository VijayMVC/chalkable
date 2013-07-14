using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstall
    {
        public Guid Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public Guid PersonRef { get; set; }
        public DateTime InstallDate { get; set; }
        public Guid SchoolYearRef { get; set; }
        public Guid OwnerRef { get; set; }
        public bool Active { get; set; }
        public Guid AppInstallActionRef { get; set; }
    }
}
