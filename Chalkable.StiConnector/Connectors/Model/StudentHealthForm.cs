using System;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentHealthForm
    {
        /// <summary>
        /// The id of the student's health form.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the form template that was used to generate the student's health form.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The id of the student who has a health form.
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// The date that a staff member read the student's health form.
        /// </summary>
        public DateTime? VerifiedDate { get; set; }

    }
}
