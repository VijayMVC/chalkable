using System;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentHealthFormReadReceipt
    {
        /// <summary>
        /// The id of the academic session when the staff member read the student's health form.
        /// </summary>
        public int AcadSessionId { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// The id of the staff member who read the student's health form.
        /// </summary>
        public int StaffId { get; set; }
        /// <summary>
        /// The id of the student's health form that was read by a staff member.
        /// </summary>
        public int StudentHealthFormId { get; set; }
        /// <summary>
        /// The id of the student who has a health form.
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// The date that the staff member read the health form.
        /// </summary>
        public DateTime VerifiedDate { get; set; }

    }
}
