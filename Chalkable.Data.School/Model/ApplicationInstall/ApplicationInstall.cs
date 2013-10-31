using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstall
    {
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string SCHOOL_YEAR_REF_FIELD = "SchoolYearRef";
        public const string OWNER_REF_FIELD = "OwnerRef";
        public const string ACTIVE_FIELD = "Active";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public int PersonRef { get; set; }
        public DateTime InstallDate { get; set; }
        public int SchoolYearRef { get; set; }
        public int OwnerRef { get; set; }
        public bool Active { get; set; }
        public int AppInstallActionRef { get; set; }
    }
}
