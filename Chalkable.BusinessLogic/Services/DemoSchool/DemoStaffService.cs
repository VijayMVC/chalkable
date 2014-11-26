using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStaffService : DemoSchoolServiceBase, IStaffService
    {
        public DemoStaffService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(IList<Staff> staffs)
        {
            throw new NotImplementedException();
        }

        public void Edit(IList<Staff> staffs)
        {
            throw new NotImplementedException();

        }

        public void Delete(IList<Staff> staffs)
        {
            throw new NotImplementedException();
        }

        public IList<Staff> GetStaffs()
        {
            return Storage.StaffStorage.GetAll();
        }

        public Staff GetStaff(int staffId)
        {
            return Storage.StaffStorage.GetById(staffId);
        }

        public void AddStaffSchools(IList<StaffSchool> staffSchools)
        {
            throw new NotImplementedException();
        }

        public void EditStaffSchools(IList<StaffSchool> staffSchools)
        {
            throw new NotImplementedException();
        }

        public void DeleteStaffSchools(IList<StaffSchool> staffSchools)
        {
            throw new NotImplementedException();
        }

        public IList<StaffSchool> GetStaffSchools()
        {
            return Storage.StaffSchoolStorage.GetAll();
        }

        public PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName,
            int start, int count)
        {
            var staffs = Storage.StaffStorage.GetAll().AsEnumerable();
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.ToLower().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                staffs = staffs.Where(x => words.Any(w => x.FirstName.ToLower().Contains(w)));
            }
            var classTeachers = Storage.ClassTeacherStorage.GetClassTeachers(null, null);
            if (classId.HasValue)
                classTeachers = classTeachers.Where(x => x.ClassRef == classId.Value).ToList();
            if (studentId.HasValue)
            {
                var classPerson = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery {PersonId = studentId});
                classTeachers = classTeachers.Where(ct => classPerson.Any(cp => cp.ClassRef == ct.ClassRef)).ToList();
            }
            staffs = staffs.Where(st => classTeachers.Any(ct => ct.PersonRef == st.Id));
            staffs = orderByFirstName ? staffs.OrderBy(x => x.FirstName) : staffs.OrderBy(x => x.LastName);
            return new PaginatedList<Staff>(staffs.ToList(), start / count, count);
        }
    }
}
