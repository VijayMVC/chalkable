using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class StudentParent
    {
        public Guid Id { get; set; }
        public Guid ParentRef { get; set; }
        public Guid StudentRef { get; set; }
    }

    public class StudentParentDetails : StudentParent
    {
        public PersonDetails Parent { get; set; }
        public PersonDetails Student { get; set; }
    }
}
