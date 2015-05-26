using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGroupService
    {
        void AddGroup(string name);
        Group EditGroup(int groupId, string name);
        void DeleteGroup(int groupId);
        void UpdateStudentGroups(int groupId, IList<int> studentIds);
        
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

        public Group EditGroup(int groupId, string name)
        {
            using (var uow = Update())
            {
                var da = new GroupDataAccess(uow);
                var group = da.GetById(groupId);
                EnsureInGroupModifyPermission(group);
                group.Name = name;
                uow.Commit();
                return group;
            }
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

        public void UpdateStudentGroups(int groupId, IList<int> studentIds)
        {
            DoUpdate(u =>
                {
                    EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                    new StudentGroupDataAccess(u).ReCreateStudentGroups(groupId, studentIds ?? new List<int>());
                });
        }

        public IList<GroupDetails> GetGroupsDetails(int ownerId)
        {
            return DoRead(u => new GroupDataAccess(u).GetGroupsDetails(ownerId));
        }

        private void EnsureInGroupModifyPermission(Group gGroup)
        {
            if (gGroup.OwnerRef != Context.PersonId)
                throw new ChalkableException("Only owner can modify group");
        }
    }
}
