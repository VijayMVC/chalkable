using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGroupStorage : BaseDemoIntStorage<Group>
    {
        public DemoGroupStorage()
            : base(x => x.Id, true)
        {
        }
    }

    public class DemoStudentGroupStorage : BaseDemoIntStorage<StudentGroup>
    {
        public DemoStudentGroupStorage()
            : base(x => x.GroupRef)
        {
        }
    }


    public class DemoGroupService : DemoSchoolServiceBase, IGroupService
    {
        private DemoGroupStorage GroupStorage { get; set; }
        private DemoStudentGroupStorage StudentGroupStorage { get; set; }

        public DemoGroupService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            GroupStorage = new DemoGroupStorage();
            StudentGroupStorage = new DemoStudentGroupStorage();
        }

        //TODO: security 
        public void AddGroup(string name)
        {
            GroupStorage.Add(new Group {Name = name});
        }

        public void DeleteGroup(int groupId)
        {
            GroupStorage.Delete(groupId);
        }

        public void AssignStudentsToGroup(int groupId, IList<int> studentIds)
        {
            if (studentIds == null || studentIds.Count == 0)
                throw new ChalkableException("Invalid param studentids. Studentids param is empty");
            var studentGroups = studentIds.Select(studentId => new StudentGroup
            {
                GroupRef = groupId,
                StudentRef = studentId
            }).ToList();
            StudentGroupStorage.Add(studentGroups);
        }

        public void UnassignStudentsFromGroup(int groupId, IList<int> studentIds)
        {
            if (studentIds == null || studentIds.Count == 0)
                throw new ChalkableException("Invalid param studentids. Studentids param is empty");
            var studentGroups = studentIds.Select(studentId => new StudentGroup
            {
                GroupRef = groupId,
                StudentRef = studentId
            }).ToList();
            StudentGroupStorage.Delete(studentGroups);
        }

        public IList<GroupDetails> GetGroupsDetails(int ownerId)
        {
            var groups = GroupStorage.GetAll().Where(g => g.OwnerRef == ownerId).ToList();
            var studentGroups = StudentGroupStorage.GetAll().ToList();
            var students = new DemoStudentStorage().GetAll();
            var res = new List<GroupDetails>();
            foreach (var demoGroup in groups)
            {
                var studentForGroup = students.Where(s => studentGroups.Any(sg => sg.StudentRef == s.Id && sg.GroupRef == demoGroup.Id)).ToList();
                res.Add(new GroupDetails
                    {
                        Id = demoGroup.Id,
                        Name = demoGroup.Name,
                        OwnerRef = demoGroup.OwnerRef,
                        Students = studentForGroup
                    });
            }
            return res;
        }
    }
}
