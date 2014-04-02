using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentParentStorage:BaseDemoStorage<int, StudentParent>
    {
        private int index = 0;

        public DemoStudentParentStorage(DemoStorage storage) : base(storage)
        {
        }

        public StudentParent Add(int studentId, int parentId)
        {
            data.Add(index++, new StudentParent
            {
                ParentRef = parentId,
                StudentRef = studentId
            });
        }

        public void Delete(int studentParentId)
        {
            data.Remove(studentParentId);
        }

        public IList<StudentParentDetails> GetParents(int studentId)
        {

            return data.Where(x => x.Value.StudentRef == studentId).Select(x => x.Value).ToList();
        }

        public void SetParents(int studentId, IList<int> parentIds)
        {
            throw new System.NotImplementedException();
        }
    }
}
