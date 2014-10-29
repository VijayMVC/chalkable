using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStaffService
    {
        void Add(IList<Staff> staffs);
        void Edit(IList<Staff> staffs);
        void Delete(IList<Staff> staffs);
        IList<Staff> GetStaffs();

        void AddStaffSchools(IList<StaffSchool> staffSchools);
        void EditStaffSchools(IList<StaffSchool> staffSchools);
        void DeleteStaffSchools(IList<StaffSchool> staffSchools);
        IList<StaffSchool> GetStaffSchools();
    }

    public class StaffService : SchoolServiceBase, IStaffService
    {
        public StaffService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<Staff> staffs)
        {
            ModifyStaff(da => da.Insert(staffs));
        }
        public void Edit(IList<Staff> staffs)
        {
            ModifyStaff(da => da.Update(staffs));
        }
        public void Delete(IList<Staff> staffs)
        {
            ModifyStaff(da=>da.Delete(staffs));
        }

        private void ModifyStaff(Action<StaffDataAccess> action)
        {
            Modify(uow => action(new StaffDataAccess(uow)));
        }
        private void ModifyStaffSchool(Action<StaffSchoolDataAccess> action)
        {
            Modify(uow => action(new StaffSchoolDataAccess(uow, Context.SchoolLocalId)));
        }
        private void Modify(Action<UnitOfWork> modifyAction)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                modifyAction(uow);
                uow.Commit();
            }          
        }

        public IList<Staff> GetStaffs()
        {
            throw new NotImplementedException();
        }

        public void AddStaffSchools(IList<StaffSchool> staffSchools)
        {
            ModifyStaffSchool(da => da.Insert(staffSchools));
        }

        public void EditStaffSchools(IList<StaffSchool> staffSchools)
        {
            ModifyStaffSchool(da => da.Update(staffSchools));
        }

        public void DeleteStaffSchools(IList<StaffSchool> staffSchools)
        {
            ModifyStaffSchool(da => da.Delete(staffSchools));
        }

        public IList<StaffSchool> GetStaffSchools()
        {
            throw new NotImplementedException();
        }
    }
}
