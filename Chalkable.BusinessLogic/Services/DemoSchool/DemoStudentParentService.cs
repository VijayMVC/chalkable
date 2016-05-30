using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStudentParentStorage : BaseDemoIntStorage<StudentParent>
    {
        public DemoStudentParentStorage()
            : base(null, true)
        {
        }

        public StudentParent Add(int studentId, int parentId)
        {
            var studentParent = new StudentParent
            {
                ParentRef = parentId,
                StudentRef = studentId
            };

            return Add(studentParent);
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

    public class DemoStudentParentService : DemoSchoolServiceBase, IStudentParentService
    {
        private DemoStudentParentStorage StudentParentStorage { get; set; }
        public DemoStudentParentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            StudentParentStorage = new DemoStudentParentStorage();
        }

        public StudentParent Add(int studentId, int parentId)
        {
            return StudentParentStorage.Add(studentId, parentId);
        }

        public void Delete(int studentParentId)
        {
            StudentParentStorage.Delete(studentParentId);
        }

        public IList<StudentParentDetails> GetParents(int studentId)
        {
            return StudentParentStorage.GetParents(studentId);
        }

        public void SetParents(int studentId, IList<int> parentIds)
        {
            StudentParentStorage.SetParents(studentId, parentIds);
        }
    }
}
