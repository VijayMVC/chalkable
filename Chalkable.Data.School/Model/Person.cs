using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Person
    {
        public Guid Id { get; set; }
        public int RoleRef { get; set; }
        public DateTime? LastMailNotification { get; set; }
        public int? SisId { get; set; }

        public IList<Address> Addresses { get; set; } 
    }

    public class Address
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
        public Guid PersonRef { get; set; }
        public int? SisId { get; set; }

        [DataEntityAttr]
        public Person Person { get; set; }
    }
}
