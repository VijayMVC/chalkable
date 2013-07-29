using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstallAction
    {
        public Guid Id { get; set; }
        public const string OWNER_REF_FIELD = "OwnerRef";
        public Guid OwnerRef { get; set; }
        public Guid? PersonRef { get; set; }
        public Guid ApplicatioinRef { get; set; }
        public string Description { get; set; }
    }
}
