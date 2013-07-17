using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public  class Phone
    {
        public Guid Id { get; set; }
        public Guid PersonRef { get; set; }
        public string Value { get; set; }
        public PhoneType Type { get; set; }
        public string DigitOnlyValue { get; set; }
        public bool IsPrimary { get; set; }
        public int? SisId { get; set; }
    }

    public enum PhoneType
    {
        Home = 0,
        Work = 1,
        Mobile = 2
    }
}
