using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
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
        Staff GetStaff(int staffId);

        void AddStaffSchools(IList<StaffSchool> staffSchools);
        void EditStaffSchools(IList<StaffSchool> staffSchools);
        void DeleteStaffSchools(IList<StaffSchool> staffSchools);
        IList<StaffSchool> GetStaffSchools();
        PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName, int start, int count);
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
            DoUpdate(modifyAction);
        }

        public Staff GetStaff(int staffId)
        {
            return DoRead(uow => new StaffDataAccess(uow).GetById(staffId));
        }

        public IList<Staff> GetStaffs()
        {
            return DoRead(uow => new StaffDataAccess(uow).GetAll());
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
            return DoRead(uow => new StaffSchoolDataAccess(uow, Context.SchoolLocalId).GetAll());
        }
        
        public PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName,
            int start, int count)
        {
            return DoRead(u => new StaffDataAccess(u).SearchStaff(schoolYearId, classId, studentId, filter, orderByFirstName,
                            start, count));
        }

    }
}
