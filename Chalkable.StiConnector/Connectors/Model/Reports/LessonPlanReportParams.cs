using System;

namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class LessonPlanReportParams
    {
        public string DistrictName { get; set; }
        public string Header { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
 
        //TODO : ask Jonathan about this field
        public int[] XMLActivityAttribute { get; set; }

        public int[] XMLActivityCategory { get; set; }

        ///<summary>
        // enum PublicPrivateTextOptions 
        //    Public = 0,
        //    Private = 1,
        //    Both = 2
        /// </summary>
        public int PublicPrivateText { get; set; }

        public int? MaxCount { get; set; } //TODO: ask Jonathan is this minum number of days with plans 
        public bool IncludeActivities { get; set; }
        public int? SectionId { get; set; }
        public int? StaffFilterId { get; set; }
        public int AcadSessionId { get; set; }
        
        ///<summary>
        // enum SortActivityOptions 
        //    Date = 0,
        //    Category = 1
        /// </summary>
        public int SortActivities { get; set; }
        ///<summary>
        // enum SortSectionOptions 
        //    TeacherSection = 0,
        //    TeacherCourse = 1,
        //    TeacherPeriod = 2,
        //    Section = 3
        /// </summary>
        public int SortSections { get; set; }

        public int? StaffId { get; set; }   
        public bool IncludeStandards { get; set; }        
    }
}
