using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{

        public class SystemSetting
        {
            public const string CATEGORY_FIELD = "Category";
            public const string SETTING_FIELD = "Setting";
            
            [PrimaryKeyFieldAttr]
            public string Category { get; set; }
            [PrimaryKeyFieldAttr]
            public string Setting { get; set; }
            public string Value { get; set; }
        }

}
