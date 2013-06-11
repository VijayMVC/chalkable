using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Address
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
        public Guid PersonRef { get; set; }
        public int? SisId { get; set; }
    }

    public enum AddressType
    {
        Home,
        Work
    }
}
