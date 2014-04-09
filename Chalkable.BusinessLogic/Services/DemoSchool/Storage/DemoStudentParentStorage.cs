using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentParentStorage:BaseDemoStorage<int, StudentParent>
    {
        public DemoStudentParentStorage(DemoStorage storage) : base(storage)
        {
        }

        public StudentParent Add(int studentId, int parentId)
        {
            var studentParent = new StudentParent
            {
                ParentRef = parentId,
                StudentRef = studentId
            };
 
            data.Add(GetNextFreeId(), studentParent);
            return studentParent;
        }

        public IList<StudentParentDetails> GetParents(int studentId)
        {
            var studentParents = data.Where(x => x.Value.StudentRef == studentId).Select(x => x.Value).ToList();


            return studentParents.Select(studentParent => new StudentParentDetails
            {
                Id = studentParent.Id, 
                ParentRef = studentParent.ParentRef, 
                StudentRef = studentParent.StudentRef
            }).ToList();
        }

        public void SetParents(int studentId, IList<int> parentIds)
        {
            foreach (var parentId in parentIds)
            {
                if (data.Count(x => x.Value.StudentRef == studentId && x.Value.ParentRef == parentId) == 0)
                    Add(studentId, parentId);
            }
        }
    }
}
