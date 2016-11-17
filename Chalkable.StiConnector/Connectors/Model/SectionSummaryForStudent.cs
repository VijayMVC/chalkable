namespace Chalkable.StiConnector.Connectors.Model
{
    public class SectionSummaryForStudent
    {
        /// <summary>
        /// Total student daily absence values
        /// </summary>
        public decimal AbsenceCount { get; set; }

        public string AlphaGrade { get; set; }

        /// <summary>
        /// Student average for the class for the current or closest grading. 
        /// </summary>
        public decimal? Average { get; set; }

        /// <summary>
        /// Total number of discipline infractions that were entered for the class
        /// </summary>
        public int DisciplineCount { get; set; }

        /// <summary>
        /// Total number of days enrolled in the class
        /// </summary>
        public int EnrollmentCount { get; set; }

        /// <summary>
        /// The section id of the class. 
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// The section name of the class. 
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// The teacher's gender.  Used to help detemine which icon to display in Chalkable
        /// </summary>
        public string TeacherGender { get; set; }

        /// <summary>
        /// The Id of the primary teacher.  
        /// </summary>
        public int? TeacherId { get; set; }

        /// <summary>
        /// The name of the primary teacher.  Format will be Title FirstName LastName 
        /// </summary>
        public string TeacherName { get; set; }
    }
}
