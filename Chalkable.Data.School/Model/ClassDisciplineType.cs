using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassDisciplineType
    {
        public Guid Id { get; set; }
        public const string CLASS_DISCIPLINE_REF = "ClassDisciplineRef";
        public Guid ClassDisciplineRef { get; set; }
        public const string DISCIPLINE_TYPE_REF = "DisciplineTypeRef";
        public Guid DisciplineTypeRef { get; set; }
    }

    public class ClassDisciplineTypeDetails : ClassDisciplineType
    {
        public ClassDisciplineDetails ClassDiscipline { get; set; }
        [DataEntityAttr]
        public DisciplineType DisciplineType { get; set; }
    }
}
