using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentCommentViewData : ShortPersonViewData
    {
        public string Comment { get; set; }

        protected StudentCommentViewData(Person person) : base(person)
        {
        }

        public static IList<StudentCommentViewData> Create(IList<StudentCommentInfo> studentComments)
        {
            return studentComments.Select(x => new StudentCommentViewData(x.Student) {Comment = x.Comment}).ToList();
        }
    }
}