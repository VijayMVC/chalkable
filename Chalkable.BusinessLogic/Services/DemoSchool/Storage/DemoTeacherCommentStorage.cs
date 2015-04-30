using Chalkable.BusinessLogic.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoTeacherCommentStorage : BaseDemoIntStorage<TeacherComment>
    {
        public DemoTeacherCommentStorage(DemoStorage storage) : base(storage, x=>x.CommentId, true)
        {
        }
    }
}
