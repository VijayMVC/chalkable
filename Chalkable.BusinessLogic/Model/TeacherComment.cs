using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class TeacherComment
    {
        public int CommentId { get; set; }
        public int TeacherId { get; set; }
        public string Comment { get; set; }
        public bool IsSystem { get; set; }

        public static TeacherComment Create(SectionComment sectionComment)
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
