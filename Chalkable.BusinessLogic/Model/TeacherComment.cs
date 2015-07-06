using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class TeacherComment
    {
        public int? CommentId { get; set; }
        public int? TeacherId { get; set; }
        public string Comment { get; set; }
        public bool IsSystem { get; set; }

        public bool EditableForTeacher
        {
            get { return CommentId.HasValue && TeacherId.HasValue && !IsSystem; }
        }

        public static TeacherComment Create(GradebookCommect sectionComment)
        {
            return new TeacherComment
                {
                    TeacherId = sectionComment.TeacherId,
                    Comment = sectionComment.Comment,
                    CommentId = sectionComment.Id,
                    IsSystem = sectionComment.IsSystem
                };
        }
    }
}
