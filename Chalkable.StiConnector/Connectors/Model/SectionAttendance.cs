using System;
using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentSectionAttendance
    {

        /// <summary>
        /// Indicates whether or not the student was absent on the previous day the class met.
        /// </summary>
        /// <remarks>This should be used to give the user a visual cue that the student was 
        /// absent the previous day the class met.  This will  help in identifying which 
        /// students to collect a note from</remarks>
        public bool AbsentPreviousDay { get; set; }

        /// <summary>
        /// The category of the absence.  Value will be either "Excused" or "Unexcused"
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The level of the absence. The ClassroomLevel should be used when displaying attendance
        /// information to a user.  Value will always be either "Absent", "Missing", "Tardy", or "Present".
        /// </summary>
        public string ClassroomLevel { get; set; }

        /// <summary>
        /// The date of attendance
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// The  period absence level.  This value should be used when determining which absence
        /// reasons to show "All Period", "All Period Other", "Half Period", "Half Period Other", or "Tardy"
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Indicates whether or not the user should be able to update the student's section attendance. 
        /// If ReadOnly is true, a user should not be able to edit the student's section attendance
        /// </summary>        
        public bool ReadOnly { get; set; }

        /// <summary>
        /// A description of why a user should not be able to update the student's section attendance
        /// </summary>
        public string ReadOnlyReason { get; set; }

        /// <summary>
        /// The id of the absence reason
        /// </summary>
        public short ReasonId { get; set; }

        /// <summary>
        /// The id of the section
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// The id of the student
        /// </summary>
        public int StudentId { get; set; }

    }

    public class SectionAttendance
    {

        public string Date { get; set; }

        /// <summary>
        /// Indicates whether or not the section meets during the daily attendance period
        /// </summary>
        public bool IsDailyAttendancePeriod { get; set; }

        /// <summary>
        /// Indicates whether or not the attendance has been previously posted for the section on the specific date
        /// </summary>
        public bool IsPosted { get; set; }

        /// <summary>
        /// Indicates whether or not the MergeRosters setting has been turned on for this section. 
        /// When true, the StudentAttendance list will contain students from all the sections taught  
        /// by the primary teacher that meet on the same daytype and in the same period as the 
        /// specified section
        /// </summary>        
        public bool MergeRosters { get; set; }

        /// <summary>
        ///  Indicates whether or not the user should be able to post/re-post the attendance
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        ///If the SectionAttendance is marked as read-only, this is a text description of why it is readonly
        /// </summary>
        public string ReadOnlyReason { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        ///A list of all students enrolled in the section and their current attendance status
        /// </summary>
        public IEnumerable<StudentSectionAttendance> StudentAttendance { get; set; }

    }
}