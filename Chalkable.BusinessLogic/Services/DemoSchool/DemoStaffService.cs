using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
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
    }
}
