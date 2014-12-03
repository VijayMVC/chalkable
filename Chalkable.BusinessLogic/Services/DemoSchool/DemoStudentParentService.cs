using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStudentParentService : DemoSchoolServiceBase, IStudentParentService
    {
        public DemoStudentParentService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public StudentParent Add(int studentId, int parentId)
        {
            return Storage.StudentParentStorage.Add(studentId, parentId);
        }

        public void Delete(int studentParentId)
        {
            Storage.StudentParentStorage.Delete(studentParentId);
        }

        public IList<StudentParentDetails> GetParents(int studentId)
        {
            return Storage.StudentParentStorage.GetParents(studentId);
        }

        public void SetParents(int studentId, IList<int> parentIds)
        {
            Storage.StudentParentStorage.SetParents(studentId, parentIds);
        }
    }
}
