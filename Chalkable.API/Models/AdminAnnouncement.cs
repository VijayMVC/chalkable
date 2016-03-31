using System;

namespace Chalkable.API.Models
{
    public class AdminAnnouncement : ShortAnnouncement
    {
        public DateTime? ExpiresDate { get; set; }
    }
}
