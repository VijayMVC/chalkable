using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
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

        public Group AddGroup(string name, IntList studentsIds)
        {
            throw new NotImplementedException();
        }

        public Group EditGroup(int groupId, string name, IntList studentsIds)
        {
            throw new NotImplementedException();
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
            if(string.IsNullOrEmpty(name))
                throw new ChalkableException("Invalid name param. Name parameter is empty");
            var group = GroupStorage.GetById(groupId);
            EnsureInGroupModifyPermission(group);
            group.Name = name;
            GroupStorage.Update(group);
            return group;
        }
        
        public GroupInfo Info(int groupId)
        {
            throw new NotImplementedException();
        }

        public IList<int> GetStudentIdsByGroups(IList<int> groupIds)
        {
            throw new NotImplementedException();
        }

        public IList<Group> GetGroups(int ownerId, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
