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
        public const string CLASS_PERSON_REF_FIELD = "ClassPersonRef";
        public Guid ClassPersonRef { get; set; }
        public const string CLASS_PERIOD_REF_FIELD = "ClassPeriodRef";
        public Guid ClassPeriodRef { get; set; }
        public const string DATE_FIELD = "Date";
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

    public class ClassDisciplineDetails : ClassDiscipline
    {
        public Person Student { get; set; }

        private ClassPerson classPerson;
        [DataEntityAttr]
        public ClassPerson ClassPerson
        {
            get { return classPerson; }
            set
            {
                classPerson = value;
                //if (value != null && value.Id != Guid.Empty)
                //    ClassPersonRef = value.Id;
            }
        }

        private ClassPeriod classPeriod;
        [DataEntityAttr]
        public ClassPeriod ClassPeriod
        {
            get { return classPeriod; }
            set
            {
                classPeriod = value;
                if (value != null && value.Id != Guid.Empty)
                    ClassPeriodRef = value.Id;
            }
        }
        [DataEntityAttr]
        public Class Class { get; set; }

        public IList<ClassDisciplineTypeDetails> DisciplineTypes { get; set; } 
    }
}
