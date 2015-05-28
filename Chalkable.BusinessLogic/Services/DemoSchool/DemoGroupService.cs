using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
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

        public void AddGroup(string name)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            BaseSecurity.IsDistrictAdmin(Context); // only admin can create group ... think do we need this for demo 
            GroupStorage.Add(new Group {Name = name, OwnerRef = Context.PersonId.Value});
        }

        public void DeleteGroup(int groupId)
        {
            EnsureInGroupModifyPermission(GroupStorage.GetById(groupId));
            GroupStorage.Delete(groupId);
        }

        public void AssignStudentsToGroup(int groupId, IList<int> studentIds)
        {
            DemandStudentIdsParam(studentIds);
            EnsureInGroupModifyPermission(GroupStorage.GetById(groupId));
            StudentGroupStorage.Add(BuildStudentGroups(groupId, studentIds));
        }

        public void UnassignStudentsFromGroup(int groupId, IList<int> studentIds)
        {
            DemandStudentIdsParam(studentIds);
            EnsureInGroupModifyPermission(GroupStorage.GetById(groupId));
            StudentGroupStorage.Delete(BuildStudentGroups(groupId, studentIds));
        }

        public IList<GroupDetails> GetGroupsDetails(int ownerId)
        {
            var groups = GroupStorage.GetAll().Where(g => g.OwnerRef == ownerId).ToList();
            var studentGroups = StudentGroupStorage.GetAll().ToList();
            var students = new DemoStudentStorage().GetAll().Select(x=>DemoStudentService.BuildStudentDetailsData(x, null)).ToList();
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

        private void DemandStudentIdsParam(IList<int> studentIds)
        {
            if (studentIds == null || studentIds.Count == 0)
                throw new ChalkableException("Invalid param studentids. Studentids param is empty");
        }
        private void EnsureInGroupModifyPermission(Group gGroup)
        {
            if (gGroup.OwnerRef != Context.PersonId)
                throw new ChalkableException("Only owner can modify group");
        }
        private IList<StudentGroup> BuildStudentGroups(int groupId, IList<int> studentIds)
        {
            return studentIds.Select(studentId => new StudentGroup
            {
                GroupRef = groupId,
                StudentRef = studentId
            }).ToList();
        }
        
        public Group EditGroup(int groupId, string name)
        {
            throw new NotImplementedException();
        }

        public void UpdateStudentGroups(int groupId, IList<int> studentIds)
        {
            throw new NotImplementedException();
        }


        public IList<Group> GetGroups(int ownerId)
        {
            throw new NotImplementedException();
        }
    }
}
