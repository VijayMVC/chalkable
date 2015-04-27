using System;

namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class LessonPlanReportParams
    {
        /// <summary>
        /// Start Date of the report
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// The end date of the report
        /// </summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Ids of the activity attributes that should be included in the report
        /// </summary>
        public int[] ActivityAttributeIds { get; set; }
        
        /// <summary>
        /// Ids of the activity categories that should be included in the report
        /// </summary>
        public int[] ActivityCategoryIds { get; set; }
        
        ///<summary>
        // enum PublicPrivateTextOptions 
        //    Public = 0,
        //    Private = 1,
        //    Both = 2
        /// </summary>
        public int PublicPrivateText { get; set; }

        /// <summary>
        /// (optional) Only print sections where the number of days with activities scheduled is greater than the MaxCount.  
        /// Example. Section A has activities on 4 seperate days.  Section B has activities on 10 seperate days.  
        /// If the user enters 6, Section B will print.  Section A will not.
        /// </summary>
        public int? MaxCount { get; set; }

        public bool IncludeActivities { get; set; }
        /// <summary>
        /// Id of the section
        /// </summary>
        public int? SectionId { get; set; }
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
