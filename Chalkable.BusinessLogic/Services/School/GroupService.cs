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
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new GroupDataAccess(u).Insert(new Group { Name = name }));
        }

        public void DeleteGroup(int groupId)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new GroupDataAccess(u).Delete(groupId));
        }

        public void AssignStudentsToGroup(int groupId, IList<int> studentIds)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            if(studentIds == null || studentIds.Count == 0)
                throw new ChalkableException("Invalid param studentids. Studentids param is empty");
            var studentGroups = studentIds.Select(studentId => new StudentGroup
                {
                    GroupRef = groupId,
                    StudentRef = studentId
                }).ToList();
            DoUpdate(u => new DataAccessBase<StudentGroup, int>(u).Insert(studentGroups));
        }

        public void UnassignStudentsFromGroup(int groupId, IList<int> studentIds)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            if (studentIds == null || studentIds.Count == 0)
                throw new ChalkableException("Invalid param studentids. Studentids param is empty");
            var studentGroups = studentIds.Select(studentId => new StudentGroup
            {
                GroupRef = groupId,
                StudentRef = studentId
            }).ToList();
            DoUpdate(u => new DataAccessBase<StudentGroup, int>(u).Delete(studentGroups));
        }

        public IList<GroupDetails> GetGroupsDetails(int ownerId)
        {
            return DoRead(u => new GroupDataAccess(u).GetGroupsDetails(ownerId));
        }
    }
}
