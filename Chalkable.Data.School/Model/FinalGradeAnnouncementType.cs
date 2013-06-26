using System;

namespace Chalkable.Data.School.Model
{
    public class FinalGradeAnnouncementType
    {
        public Guid Id { get; set; }
        public int AnnouncementTypeRef { get; set; }
        public Guid FinalGradeRef { get; set; }
        public int PercentValue { get; set; }
        public GradingStyleEnum GradingStyle { get; set; }
        public bool DropLowest { get; set; }
    }
}
