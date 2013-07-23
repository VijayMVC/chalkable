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
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public Guid ApplicationRef { get; set; }
        public const string PERSON_REF_FIELD = "PersonRef";
        public Guid PersonRef { get; set; }
        public DateTime InstallDate { get; set; }
        public const string SCHOOL_YEAR_REF_FIELD = "SchoolYearRef";
        public Guid SchoolYearRef { get; set; }
        public const string OWNER_REF_FIELD = "OwnerRef";
        public Guid OwnerRef { get; set; }
        public const string ACTIVE_FIELD = "Active";
        public bool Active { get; set; }
        public Guid AppInstallActionRef { get; set; }
    }
}
