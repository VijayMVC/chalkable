using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Chlk
{
    public class AnnouncementApplication
    {
        public const string ID_FIELD = "Id";
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public const string ACTIVE_FIELD = "Active";
    
        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public Guid ApplicationRef { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
    }
}
