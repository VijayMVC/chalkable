using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class FinalGradeAnnouncementType
    {
        public const string ID_FIELD = "Id";
        public Guid Id { get; set; }
        public const string ANNOUNCEMENT_TYPE_ID_FIELD = "AnnouncementTypeRef";
        public int AnnouncementTypeRef { get; set; }
        public const string FINAL_GRADE_ID_FIELD = "FinalGradeRef";
        public Guid FinalGradeRef { get; set; }
        public int PercentValue { get; set; }
        public GradingStyleEnum GradingStyle { get; set; }
        public bool DropLowest { get; set; }

        [DataEntityAttr]
        public AnnouncementType AnnouncementType { get; set; }
    }
}
