using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGroupService
    {
        Group AddGroup(string name, IntList studentsIds);
        Group EditGroup(int groupId, string name, IntList studentsIds);
        void DeleteGroup(int groupId);

        IList<Group> GetGroups(int ownerId, string filter);
        GroupInfo Info(int groupId);
    }

    public class GroupService : SchoolServiceBase, IGroupService
    {
        public GroupService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Group AddGroup(string name, IntList studentsIds)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            if (string.IsNullOrEmpty(name))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Name"));

            BaseSecurity.EnsureDistrictAdmin(Context);

            using (var uow = Update())
            {
                var da = new GroupDataAccess(uow);
                var groupId = da.InsertWithEntityId(new Group { Name = name, OwnerRef = Context.PersonId.Value });
                AssignStudentsToGroup(groupId, studentsIds);
                uow.Commit();
                return da.GetById(groupId);
            }
        }

        public Group EditGroup(int groupId, string name, IntList studentsIds)
        {
            if (string.IsNullOrEmpty(name))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Name"));
            using (var uow = Update())
            {
                var da = new GroupDataAccess(uow);
                var group = da.GetById(groupId);
                EnsureInGroupModifyPermission(group);
                group.Name = name;
                da.Update(group);
                AssignStudentsToGroup(groupId, studentsIds);
                uow.Commit();
                return group;
            }
        }

        public void AssignStudentsToGroup(int groupId, IntList studentIds)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                var da = new DataAccessBase<StudentGroup>(u);
                var studentGroups = da.GetAll(new AndQueryCondition { { StudentGroup.GROUP_REF_FIELD, groupId } }).Select(x => x.StudentRef);
                da.Delete(BuildStudentGroups(groupId, studentGroups));
                da.Insert(BuildStudentGroups(groupId, studentIds));
            });
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


        public IList<Group> GetGroups(int ownerId, string filter)
        {
            return DoRead(u => new GroupDataAccess(u).GetAll(new AndQueryCondition {{Group.OWNER_REF_FIELD, ownerId}}, filter));
        }

        public GroupInfo Info(int groupId)
        {
            return DoRead(u => new GroupDataAccess(u).GetGroutInfo(groupId));
        }

        private void EnsureInGroupModifyPermission(Group gGroup)
        {
            if (gGroup.OwnerRef != Context.PersonId)
                throw new ChalkableSecurityException("Only owner can modify group");
        }

        private IList<StudentGroup> BuildStudentGroups(int groupId, IEnumerable<int> studentIds)
        {
            return studentIds.Select(x => new StudentGroup {GroupRef = groupId, StudentRef = x}).ToList();
        }
    }
}
