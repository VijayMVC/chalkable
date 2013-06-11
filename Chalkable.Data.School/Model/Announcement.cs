using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{

    public class Announcement
    {
        public Guid Id { get; set; }
        public Guid PersonRef { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public Guid AnnouncementTypeRef { get; set; }
        public int State { get; set; }
        public int GradingStyle { get; set; }
        public string Subject { get; set; }
        public Guid MarkingPeriodClassRef { get; set; }
        public int Order { get; set; }
        public bool Dropped { get; set; }
    }


}
