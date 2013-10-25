using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstallActionClasses
    {
        public const string CLASS_REF_FIELD = "ClassRef";
      
        public Guid Id { get; set; }
        public int ClassRef { get; set; }
        public int AppInstallActionRef { get; set; }
    }
}
