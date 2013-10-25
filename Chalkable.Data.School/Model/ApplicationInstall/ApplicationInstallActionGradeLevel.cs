using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.ApplicationInstall
{
    public class ApplicationInstallActionGradeLevel
    {
        public const string GRADE_LEVEL_REF_FIELD = "GradeLevelRef";
   
        public int Id { get; set; }
        public int GradeLevelRef { get; set; }
        public int AppInstallActionRef { get; set; }
    }
}
