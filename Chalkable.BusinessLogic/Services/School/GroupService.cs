using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGroupService
    {
        void AddGroup(string name);
        void DeleteGroup(int groupId);
        void AssignStudentsToGroup(int groupId, IList<int> studentIds);
        void UnassignStudentsFromGroup(int groupId, IList<int> studentIds);

        IList<GroupDetails> GetGroupsDetails(int ownerId);
    }

    public class GroupService : SchoolServiceBase, IGroupService
    {
        public GroupService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddGroup(string name)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new GroupDataAccess(u).Insert(new Group { Name = name, OwnerRef  = Context.PersonId.Value}));
        }

        public void DeleteGroup(int groupId)
        {
            DoUpdate(u =>
                {
                    var da = new GroupDataAccess(u);
                    EnsureInGroupModifyPermission(da.GetById(groupId));
                    da.Delete(groupId);
                });
        }

        public void AssignStudentsToGroup(int groupId, IList<int> studentIds)
        {
            DemandStudentIdsParam(studentIds);
            var studentGroups = BuildStudentGroups(groupId, studentIds);
            DoUpdate(u =>
                {
                    EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                    new DataAccessBase<StudentGroup, int>(u).Insert(studentGroups);
                });
        }

        public void UnassignStudentsFromGroup(int groupId, IList<int> studentIds)
        {
            DemandStudentIdsParam(studentIds);
            var studentGroups = BuildStudentGroups(groupId, studentIds);
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new DataAccessBase<StudentGroup, int>(u).Insert(studentGroups);
            });
        }

        public IList<GroupDetails> GetGroupsDetails(int ownerId)
        {
            return DoRead(u => new GroupDataAccess(u).GetGroupsDetails(ownerId));
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
    }
}
