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
        IList<int> GetStudentIdsByGroups(IList<int> groupIds);
    }

    public class GroupService : SchoolServiceBase, IGroupService
    {
        public GroupService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Group AddGroup(string name, IntList studentIds)
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
                AssignStudentsToGroup(groupId, studentIds, uow);
                uow.Commit();
                var group = da.GetById(groupId);
                group.StudentCount = studentIds?.Count ?? 0;
                return group;
            }
        }

        public Group EditGroup(int groupId, string name, IntList studentIds)
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
                AssignStudentsToGroup(groupId, studentIds, uow);
                uow.Commit();
                group.StudentCount = studentIds?.Count ?? 0;
                return group;
            }
        }

        private void AssignStudentsToGroup(int groupId, IntList studentIds, UnitOfWork uow)
        {
            EnsureInGroupModifyPermission(new GroupDataAccess(uow).GetById(groupId));
            var da = new DataAccessBase<StudentGroup>(uow);
            var groupStudents = da.GetAll(new AndQueryCondition { { StudentGroup.GROUP_REF_FIELD, groupId } }).Select(x => x.StudentRef).ToList();
            if(groupStudents.Count > 0)
                da.Delete(BuildStudentGroups(groupId, groupStudents));
            if(studentIds != null)
                da.Insert(BuildStudentGroups(groupId, studentIds));
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
            return DoRead(u => new GroupDataAccess(u).GetAll(new AndQueryCondition {{Group.OWNER_REF_FIELD, ownerId}}, filter)).OrderBy(x => x.Name).ToList();
        }

        public GroupInfo Info(int groupId)
        {
            return DoRead(u => new GroupDataAccess(u).GetGroutInfo(groupId));
        }

        public IList<int> GetStudentIdsByGroups(IList<int> groupIds)
        {
            return DoRead(u => new GroupDataAccess(u).GetStudentsByGroups(groupIds));
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
