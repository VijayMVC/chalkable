using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentParentStorage
    {
        public StudentParent Add(int studentId, int parentId)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int studentParentId)
        {
            throw new System.NotImplementedException();
        }

        public IList<StudentParentDetails> GetParents(int studentId)
        {
            throw new System.NotImplementedException();
        }

        public void SetParents(int studentId, IList<int> parentIds)
        {
            throw new System.NotImplementedException();
        }
    }
}
