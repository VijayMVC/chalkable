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
            Storage.DemoStaffStorage.Add(staffs);
        }

        public void Edit(IList<Staff> staffs)
        {
            Storage.DemoStaffStorage.Update(staffs);

        }

        public void Delete(IList<Staff> staffs)
        {
            Storage.DemoStaffStorage.Delete(staffs);
        }

        public IList<Staff> GetStaffs()
        {
            return Storage.DemoStaffStorage.GetAll();
        }

        public void AddStaffSchools(IList<StaffSchool> staffSchools)
        {
            Storage.DemoStaffSchoolStorage.Add(staffSchools);
        }

        public void EditStaffSchools(IList<StaffSchool> staffSchools)
        {
            Storage.DemoStaffSchoolStorage.Update(staffSchools);
        }

        public void DeleteStaffSchools(IList<StaffSchool> staffSchools)
        {
            Storage.DemoStaffSchoolStorage.Delete(staffSchools);
        }

        public IList<StaffSchool> GetStaffSchools()
        {
            return Storage.DemoStaffSchoolStorage.GetAll();
        }
    }
}
