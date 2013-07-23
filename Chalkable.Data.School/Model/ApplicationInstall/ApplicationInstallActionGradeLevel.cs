using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstallActionGradeLevel
    {
        public Guid Id { get; set; }
        public const string GRADE_LEVEL_REF_FIELD = "GradeLevelRef";
        public Guid GradeLevelRef { get; set; }
        public Guid AppInstallActionRef { get; set; }
    }
}
