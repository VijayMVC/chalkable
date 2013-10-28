using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class StudentParent
    {
        public int Id { get; set; }
        public int ParentRef { get; set; }
        public int StudentRef { get; set; }
    }

    public class StudentParentDetails : StudentParent
    {
        public PersonDetails Parent { get; set; }
        public PersonDetails Student { get; set; }
    }
}
