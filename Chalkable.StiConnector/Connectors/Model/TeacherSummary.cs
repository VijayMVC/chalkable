using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class TeacherSummary
    {

        /// <summary>
        /// Total student daily absence values for the day
        /// </summary>
        public decimal AbsenceCount { get; set; }

        /// <summary>
        /// Average of every student average for the class for the current or closest grading. 
        /// </summary>
        public decimal? Average { get; set; }

        public IEnumerable<Section> Classes { get; set; }

        /// <summary>
        /// Total number of discipline infractions that were entered for the day
        /// </summary>
        public int DisciplineCount { get; set; }

        /// <summary>
        /// Total number of students enrolled in the class
        /// </summary>
        public int EnrollmentCount { get; set; }


        /// <summary>
        /// The Id of the primary teacher.  
        /// </summary>
        public int? TeacherId { get; set; }

        /// <summary>
        /// The display name of the primary teacher. 
        /// </summary>
        public string TeacherName { get; set; }

        /// <summary>
        /// The gender of the primary teacher. 
        /// </summary>
        public string TeacherGender { get; set; }

    }
}
