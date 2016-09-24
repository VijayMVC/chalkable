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
        void AddGroup(string name);
        Group EditGroup(int groupId, string name);
        void DeleteGroup(int groupId);
        
        void AssignStudents(int groupId, IList<int> studentIds);
        void AssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId);
        void AssignStudentsBySchoolYear(int groupId, int schoolYearId);
        void AssignAll(int groupId);

        void UnssignStudents(int groupId, IList<int> studentIds);
        void UnssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId);
        void UnssignStudentsBySchoolYear(int groupId, int schoolYearId);
        void UnassignAll(int groupId);
        
        IList<Group> GetGroups(int ownerId, string filter);
        IList<StudentForGroup> GetStudentsForGroup(int groupId, int schoolYearId, int gradeLevelId, IList<int> classesIds, IList<int> coursesIds);
        GroupExplorer GetGroupExplorerInfo(int groupId);

        IList<Group> GetByIds(IList<int> ids);
        IList<int> GetStudentIdsByGroups(IList<int> groupIds);
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
            if (string.IsNullOrEmpty(name))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Name"));

            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new GroupDataAccess(u).Insert(new Group { Name = name, OwnerRef  = Context.PersonId.Value}));
        }

        public Group EditGroup(int groupId, string name)
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


        public IList<Group> GetGroups(int ownerId, string filter)
        {
            return DoRead(u => new GroupDataAccess(u).GetAll(new AndQueryCondition {{Group.OWNER_REF_FIELD, ownerId}}, filter));
        }

        private void EnsureInGroupModifyPermission(Group gGroup)
        {
            if (gGroup.OwnerRef != Context.PersonId)
                throw new ChalkableSecurityException("Only owner can modify group");
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

        public IList<Group> GetByIds(IList<int> ids)
        {
            return DoRead(u => new GroupDataAccess(u).GetByIds(ids));
        }

        public IList<int> GetStudentIdsByGroups(IList<int> groupIds)
        {
            return DoRead(u => new GroupDataAccess(u).GetStudentsByGroups(groupIds));
        }


        public void AssignStudents(int groupId, IList<int> studentIds)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                var da = new StudentGroupDataAccess(u);
                var studentGroups = da.GetAll(new AndQueryCondition {{StudentGroup.GROUP_REF_FIELD, groupId}});
                studentIds = studentIds.Where(id => studentGroups.All(sg => sg.StudentRef != id)).ToList();
                da.Insert(BuildStudentGroups(groupId, studentIds));
            });
        }

        public void UnssignStudents(int groupId, IList<int> studentIds)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new StudentGroupDataAccess(u).Delete(BuildStudentGroups(groupId, studentIds));
            });
        }

        public void AssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new StudentGroupDataAccess(u).AssignStudentsBySchoolYear(groupId, schoolYearId, gradeLevelId);
            });
        }

        public void AssignStudentsBySchoolYear(int groupId, int schoolYearId)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new StudentGroupDataAccess(u).AssignStudentsBySchoolYear(groupId, schoolYearId, null);
            });
        }

        public void UnssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new StudentGroupDataAccess(u).UnassignStudentsBySchoolYear(groupId, schoolYearId, gradeLevelId);
            });
        }

        public void UnssignStudentsBySchoolYear(int groupId, int schoolYearId)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new StudentGroupDataAccess(u).UnassignStudentsBySchoolYear(groupId, schoolYearId, null);
            });
        }

        private IList<StudentGroup> BuildStudentGroups(int groupId, IEnumerable<int> studentIds)
        {
            return studentIds.Select(x => new StudentGroup {GroupRef = groupId, StudentRef = x}).ToList();
        }


        public void AssignAll(int groupId)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new StudentGroupDataAccess(u).AssignAllStudentsToGroup(groupId, Context.NowSchoolTime.Date);
            });

        }

        public void UnassignAll(int groupId)
        {
            DoUpdate(u =>
            {
                EnsureInGroupModifyPermission(new GroupDataAccess(u).GetById(groupId));
                new StudentGroupDataAccess(u).UnassignAllStudentsFromGroup(groupId);
            });
        }
    }
}
