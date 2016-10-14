using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementApplication
    {
        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public Guid ApplicationRef { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    public class AnnouncementApplicationRecipient
    {
        public int AnnouncementAppicationId { get; set; }
        public string ClassName { get; set; }
        public int? StudentCount { get; set; }
    }
}
