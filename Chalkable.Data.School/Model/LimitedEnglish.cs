using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class LimitedEnglish
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SifCode { get; set; }
        public string NcesCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }
}
