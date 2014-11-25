using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName,
            int start, int count)
        {
            throw new NotImplementedException();
        }
    }
}
