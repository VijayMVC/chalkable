using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AppSetting
    {
        [PrimaryKeyFieldAttr]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
