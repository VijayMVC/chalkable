using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassDiscipline
    {
        public Guid Id { get; set; }
        public Guid ClassPersonRef { get; set; }
        public Guid ClassPeriodRef { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

    public class ClassDisciplineDetails : ClassDiscipline
    {
        public Person Student { get; set; }
        [DataEntityAttr]
        public ClassPerson ClassPerson { get; set; }
        [DataEntityAttr]
        public ClassPeriod ClassPeriod { get; set; }
        [DataEntityAttr]
        public Class Class { get; set; }

        public IList<ClassDisciplineTypeDetails> DisciplineTypes { get; set; } 
    }
}
