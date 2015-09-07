using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentCommentInfo
    {
        public StudentDetails Student { get; set; }
        public string Comment { get; set; }
    }
}
