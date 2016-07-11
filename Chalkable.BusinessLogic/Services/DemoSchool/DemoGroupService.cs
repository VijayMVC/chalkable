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

        
        public IList<Group> GetGroups(int ownerId)
        {
            return GroupStorage.GetAll().Where(x => x.OwnerRef == ownerId).ToList();
        }
        
        public IList<StudentForGroup> GetStudentsForGroup(int groupId, int schoolYearId, int gradeLevelId, IList<int> classesIds, IList<int> coursesIds)
        {
            var studentGroups = StudentGroupStorage.GetAll().Where(x => x.GroupRef == groupId).ToList();
            var students = new DemoStudentStorage().GetAll();
            var studentSchoolYears = new DemoStudentSchoolYearStorage().GetAll().Where(x => x.SchoolYearRef == schoolYearId && x.GradeLevelRef == gradeLevelId).ToList();
            students = students.Where(x => studentSchoolYears.Any(y => y.StudentRef == x.Id)).ToList();
            if ((classesIds != null && classesIds.Count > 0) || (coursesIds != null && coursesIds.Count > 0))
            {
                var classes = new List<Class>();
                var classStorage = new DemoClassStorage();
                if (classesIds != null && classesIds.Count > 0)
                    classes = classes.Union(classStorage.GetAll().Where(c => coursesIds.Contains(c.Id))).ToList();
                if (coursesIds != null)
                    classes = classes.Union(classStorage.GetAll().Where(c => c.CourseRef.HasValue && coursesIds.Contains(c.CourseRef.Value))).ToList();
                classes = classes.Distinct().ToList();

                var classPersons = new DemoClassPersonStorage().GetAll();
                students = students.Where(s => classPersons.Any(cp => classes.Any(c => c.Id == cp.ClassRef) && cp.PersonRef == s.Id)).ToList();
            }
            return students.Select(student => new StudentForGroup
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    BirthDate = student.BirthDate,
                    Gender = student.Gender,
                    HasMedicalAlert = student.HasMedicalAlert,
                    IsAllowedInetAccess = student.IsAllowedInetAccess,
                    PhotoModifiedDate = student.PhotoModifiedDate,
                    SpecialInstructions = student.SpecialInstructions,
                    SpEdStatus = student.SpEdStatus,
                    UserId = student.UserId,
                    GroupRef = studentGroups.Any(x => x.StudentRef == student.Id) ? studentGroups.First(x=>x.StudentRef == student.Id).GroupRef : (int?)null
                }).ToList();
        }

        public GroupExplorer GetGroupExplorerInfo(int groupId)
        {
            throw new NotImplementedException();
        }

        public IList<Group> GetByIds(IList<int> ids)
        {
            throw new NotImplementedException();
        }


        public void AssignStudents(int groupId, IList<int> studentIds)
        {
            throw new NotImplementedException();
        }

        public void AssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId)
        {
            throw new NotImplementedException();
        }

        public void AssignStudentsBySchoolYear(int groupId, int schoolYearId)
        {
            throw new NotImplementedException();
        }

        public void UnssignStudents(int groupId, IList<int> studentIds)
        {
            throw new NotImplementedException();
        }

        public void UnssignGradeLevel(int groupId, int schoolYearId, int gradeLevelId)
        {
            throw new NotImplementedException();
        }

        public void UnssignStudentsBySchoolYear(int groupId, int schoolYearId)
        {
            throw new NotImplementedException();
        }

        public void AssignAll(int groupId)
        {
            throw new NotImplementedException();
        }


        public void UnassignAll(int groupId)
        {
            throw new NotImplementedException();
        }
    }
}
