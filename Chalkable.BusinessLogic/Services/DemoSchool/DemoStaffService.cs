using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStaffSchoolStorage : BaseDemoIntStorage<StaffSchool>
    {
        public DemoStaffSchoolStorage()
            : base(null, true)
        {
        }
    }

    public class DemoStaffStorage : BaseDemoIntStorage<Staff>
    {
        public DemoStaffStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoStaffService : DemoSchoolServiceBase, IStaffService
    {
        private DemoStaffStorage StaffStorage { get; set; }
        private DemoStaffSchoolStorage StaffSchoolStorage { get; set; }
        public DemoStaffService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            StaffStorage = new DemoStaffStorage();
            StaffSchoolStorage = new DemoStaffSchoolStorage();
        }

        public void Add(IList<Staff> staffs)
        {
            StaffStorage.Add(staffs);
        }

        public void Edit(IList<Staff> staffs)
        {
            StaffStorage.Update(staffs);
        }

        public void Delete(IList<Staff> staffs)
        {
            StaffStorage.Delete(staffs);
        }

        public IList<Staff> GetStaffs()
        {
            return StaffStorage.GetAll();
        }

        public Staff GetStaff(int staffId)
        {
            return StaffStorage.GetById(staffId);
        }

        public void AddStaffSchools(IList<StaffSchool> staffSchools)
        {
            StaffSchoolStorage.Add(staffSchools);
        }

        public void EditStaffSchools(IList<StaffSchool> staffSchools)
        {
            StaffSchoolStorage.Update(staffSchools);
        }

        public void DeleteStaffSchools(IList<StaffSchool> staffSchools)
        {
            StaffSchoolStorage.Delete(staffSchools);
        }

        public IList<StaffSchool> GetStaffSchools()
        {
            return StaffSchoolStorage.GetAll();
        }

        public PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName,
            int start, int count)
        {
            var staffs = StaffStorage.GetAll().AsEnumerable();
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                staffs = staffs.Where(x => words.Any(w => x.FirstName.ToLower().Contains(w)));
            }
            var classTeachers = ServiceLocator.ClassService.GetClassTeachers(null, null);
            if (classId.HasValue)
                classTeachers = classTeachers.Where(x => x.ClassRef == classId.Value).ToList();
            if (studentId.HasValue)
            {
                var classPerson = ServiceLocator.ClassService.GetClassPersons(studentId.Value, null);
                classTeachers = classTeachers.Where(ct => classPerson.Any(cp => cp.ClassRef == ct.ClassRef)).ToList();
            }
            staffs = staffs.Where(st => classTeachers.Any(ct => ct.PersonRef == st.Id));
            staffs = orderByFirstName ? staffs.OrderBy(x => x.FirstName) : staffs.OrderBy(x => x.LastName);
            return new PaginatedList<Staff>(staffs.ToList(), start / count, count);
            
        }

        public IList<TeacherStatsInfo> GetTeachersStats(int schoolYearId, string filter, int? start, int? count, TeacherSortType? sortType)
        {
            throw new NotImplementedException();
        }
    }
}
