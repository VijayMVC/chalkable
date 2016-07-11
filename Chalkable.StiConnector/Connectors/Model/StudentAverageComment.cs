using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentAverageComment
    {
        /// <summary>
        /// Id of the average
        /// </summary>
        public int AverageId { get; set; }

        /// <summary>
        /// Id of the comment (GradingCommentId)
        /// </summary>
        public int? CommentId { get; set; }

        /// <summary>
        /// The code for the comment.  This is the code that prints on the report card.
        /// </summary>
        public string CommentCode { get; set; }

        /// <summary>
        /// The text for the comment.  Example: "Nice Job"
        /// </summary>
        public string CommentText { get; set; }

        /// <summary>
        /// Id of the header (GradingCommentHeaderId)
        /// </summary>
        public int HeaderId { get; set; }

        /// <summary>
        /// The sequence that the grading comment should appear
        /// </summary>
        public short HeaderSequence { get; set; }

        /// <summary>
        /// Text of the grading comment header
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
    }
}
