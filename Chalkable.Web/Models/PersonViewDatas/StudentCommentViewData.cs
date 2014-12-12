using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentCommentViewData : StudentViewData
    {
        public string Comment { get; set; }

        protected StudentCommentViewData(StudentDetails student) : base(student)
        {
        }

        public static IList<StudentCommentViewData> Create(IList<StudentCommentInfo> studentComments)
        {
            return studentComments.Select(x => new StudentCommentViewData(x.Student) {Comment = x.Comment}).ToList();
        }
    }
}