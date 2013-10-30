using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public  class Phone
    {
        public const string PERSON_REF_FIELD = "PersonRef";
        public int PersonRef { get; set; }
        public string Value { get; set; }
        public PhoneType Type { get; set; }
        public const string DIGIT_ONLY_VALUE_FIELD = "DigitOnlyValue";
        public string DigitOnlyValue { get; set; }
        public bool IsPrimary { get; set; }
    }

    public enum PhoneType
    {
        Home = 0,
        Work = 1,
        Mobile = 2
    }
}
