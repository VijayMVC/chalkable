using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public  class Phone
    {
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string DIGIT_ONLY_VALUE_FIELD = "DigitOnlyValue";
        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        public string Value { get; set; }
        public PhoneType Type { get; set; }
        [PrimaryKeyFieldAttr]
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
