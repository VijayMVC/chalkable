using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassTeacher
    {
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string CLASS_REF_FIELD = "ClassRef";

        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int ClassRef { get; set; }
        public bool IsHighlyQualified { get; set; }
        public bool IsCertified { get; set; }
        public bool IsPrimary { get; set; }
    }

}
