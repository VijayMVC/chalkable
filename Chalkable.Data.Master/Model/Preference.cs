using System;
using System.Text;

namespace Chalkable.Data.Master.Model
{

    public enum PreferenceCategoryEnum
    {
        Common = 0,
        EmailText = 1,
        ControllerDescriptions = 2
    }
    public enum PreferenceTypeEnum
    {
        ShortText = 0,
        LongText = 1,
    }

    public class Preference
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsPublic { get; set; }
        public PreferenceCategoryEnum Category { get; set; }
        public PreferenceTypeEnum Type { get; set; }
        public string Hint { get; set; }


    }


    
}
