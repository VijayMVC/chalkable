using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGroupService
    {
        void AddGroup(string name);
        Group EditGroup(int groupId, string name);
        void DeleteGroup(int groupId);
        
        void AssignStudents(int groupId, IList<int> studentIds);
        void AssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId);
        void AssignStudentsBySchoolYear(int groupId, int schoolYearId);
        void AssignAllSchools(int groupId);

        void UnssignStudents(int groupId, IList<int> studentIds);
        void UnssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId);
        void UnssignStudentsBySchoolYear(int groupId, int schoolYearId);
        void UnassignAllSchools(int groupId);

        IList<GroupDetails> GetGroupsDetails(int ownerId);
        IList<Group> GetGroups(int ownerId);

        IList<StudentForGroup> GetStudentsForGroup(int groupId, int schoolYearId, int gradeLevelId, IList<int> classesIds, IList<int> coursesIds);
        GroupExplorer GetGroupExplorerInfo(int groupId);
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
            if (string.IsNullOrEmpty(name))
                throw new ChalkableException("Invalid name param. Name parameter is empty");
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


        public IList<GroupDetails> GetGroupsDetails(int ownerId)
        {
            return DoRead(u => new GroupDataAccess(u).GetGroupsDetails(ownerId));
        }

        public IList<Group> GetGroups(int ownerId)
        {
            return DoRead(u => new GroupDataAccess(u).GetAll(new AndQueryCondition {{Group.OWNER_REF_FIELD, ownerId}}));
        }

        private void EnsureInGroupModifyPermission(Group gGroup)
        {
            if (gGroup.OwnerRef != Context.PersonId)
                throw new ChalkableException("Only owner can modify group");
        }
        
        public IList<StudentForGroup> GetStudentsForGroup(int groupId, int schoolYearId, int gradeLevelId, IList<int> classesIds, IList<int> coursesIds)
        {
            return DoRead( u => new GroupDataAccess(u).GetStudentForGroup(groupId, schoolYearId, gradeLevelId, classesIds, coursesIds));
        }

        public GroupExplorer GetGroupExplorerInfo(int groupId)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            return DoRead(u => new GroupDataAccess(u).GetGroupExplorerData(groupId, Context.PersonId.Value, Context.NowSchoolTime.Date));
        }


        public void AssignStudents(int groupId, IList<int> studentIds)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new DataAccessBase<StudentGroup>(u).Delete(BuildStudentGroups(groupId, studentIds));
            });
        }

        public void AssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId)
        {
            using (var u = Update())
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                var studentIds = new StudentDataAccess(u).GetEnrollmentStudentsIds(schoolYearId, gradeLevelId);
                new DataAccessBase<StudentGroup>(u).Delete(BuildStudentGroups(groupId, studentIds));
                u.Commit();
            }
        }

        public void AssignStudentsBySchoolYear(int groupId, int schoolYearId)
        {
            using (var u = Update())
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                var studentIds = new StudentDataAccess(u).GetEnrollmentStudentsIds(schoolYearId, null);
                new DataAccessBase<StudentGroup>(u).Delete(BuildStudentGroups(groupId, studentIds));
                u.Commit();
            }
        }

        public void UnssignStudents(int groupId, IList<int> studentIds)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new DataAccessBase<StudentGroup>(u).Delete(BuildStudentGroups(groupId, studentIds));
            });
        }

        public void UnssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId)
        {
            using (var u = Update())
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                var studentIds = new StudentDataAccess(u).GetEnrollmentStudentsIds(schoolYearId, gradeLevelId);
                new DataAccessBase<StudentGroup>(u).Delete(BuildStudentGroups(groupId, studentIds));
                u.Commit();
            }
        }

        public void UnssignStudentsBySchoolYear(int groupId, int schoolYearId)
        {
            using (var u = Update())
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                var studentIds = new StudentDataAccess(u).GetEnrollmentStudentsIds(schoolYearId, null);
                new DataAccessBase<StudentGroup>(u).Delete(BuildStudentGroups(groupId, studentIds));
                u.Commit();
            }
        }

        private IList<StudentGroup> BuildStudentGroups(int groupId, IEnumerable<int> studentIds)
        {
            return studentIds.Select(x => new StudentGroup {GroupRef = groupId, StudentRef = x}).ToList();
        }


        public void AssignAllSchools(int groupId)
        {
            throw new System.NotImplementedException();
        }

        public void UnassignAllSchools(int groupId)
        {
            throw new System.NotImplementedException();
        }
    }
}
