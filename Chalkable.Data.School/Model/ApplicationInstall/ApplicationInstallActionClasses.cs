using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstallActionClasses
    {
        public const string CLASS_REF_FIELD = "ClassRef";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public int ClassRef { get; set; }
        public int AppInstallActionRef { get; set; }
    }
}
