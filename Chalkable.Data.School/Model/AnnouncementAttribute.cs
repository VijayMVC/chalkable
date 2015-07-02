using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementAttribute
    {
        public const string IS_ACTIVE_FIELD = "IsActive";
        public const string ID_FIELD = "Id";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }
}
