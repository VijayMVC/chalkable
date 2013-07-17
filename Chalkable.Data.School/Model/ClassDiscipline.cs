using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class ClassDiscipline
    {
        public Guid Id { get; set; }
        public Guid ClassPerson { get; set; }
        public Guid ClassPeriod { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
