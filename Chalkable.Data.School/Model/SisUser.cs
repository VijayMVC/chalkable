using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class SisUser
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool LockedOut { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsSystem { get; set; }
    }
}