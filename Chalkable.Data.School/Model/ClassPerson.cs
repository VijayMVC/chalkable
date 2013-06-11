using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassPerson
    {
        public Guid Id { get; set; }
        public Guid PersonRef { get; set; }
        public Guid ClassRef { get; set; }

        [DataEntityAttr]
        public Person Person { get; set; }
        [DataEntityAttr]
        public Class Class { get; set; }
    }
}
