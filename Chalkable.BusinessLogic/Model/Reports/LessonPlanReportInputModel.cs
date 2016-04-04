using System;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class LessonPlanReportInputModel : BaseReportInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IntList AnnouncementAttributes { get; set; }
        public IntList AnnouncementTypes { get; set; }
        
        public int PublicPrivateText { get; set; }
        public bool IncludeAnnouncements { get; set; }
        public bool IncludeStandards { get; set; }   

        public int SortItems { get; set; }
        public int SortClasses { get; set; }
        public int? MaxCount { get; set; }
    }
}
