using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models
{
    public class TeacherCommentViewData
    {
        public int? CommentId { get; set; }
        public string Comment { get; set; }
        public bool IsSystem { get; set; }
        public int? TeacherId { get; set; }
        public bool EditableForTeacher { get; set; }

        public static TeacherCommentViewData Create(TeacherComment teacherComment)
        {
            return new TeacherCommentViewData
                {
                    Comment = teacherComment.Comment,
                    CommentId = teacherComment.CommentId,
                    IsSystem = teacherComment.IsSystem,
                    TeacherId = teacherComment.TeacherId,
                    EditableForTeacher = teacherComment.EditableForTeacher
                };
        }

        public static IList<TeacherCommentViewData> Create(IList<TeacherComment> teacherComments)
        {
            return teacherComments.Select(Create).ToList();
        } 
    }
}