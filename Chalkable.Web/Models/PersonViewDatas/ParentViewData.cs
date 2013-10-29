using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentParentViewData 
    {
        public Guid Id { get; set; }
        public PersonInfoViewData Parent { get; set; }


        protected StudentParentViewData(StudentParentDetails studentParent)
        {
            Id = studentParent.Id;
            Parent = PersonInfoViewData.Create(studentParent.Parent);
        }

        public static StudentParentViewData Create(StudentParentDetails studentParent)
        {
            return new StudentParentViewData(studentParent);
        }
        public static IList<StudentParentViewData> Create(IList<StudentParentDetails> studentParents)
        {
            return studentParents.Select(Create).ToList();
        } 
        
    }
}