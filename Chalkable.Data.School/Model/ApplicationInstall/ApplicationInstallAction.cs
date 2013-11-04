using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstallAction
    {
        public const string OWNER_REF_FIELD = "OwnerRef";


        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int OwnerRef { get; set; }
        public int? PersonRef { get; set; }
        public Guid ApplicationRef { get; set; }
        public string Description { get; set; }

        [NotDbFieldAttr]
        public IList<ApplicationInstall> ApplicationInstalls { get; set; } 
    }
}
