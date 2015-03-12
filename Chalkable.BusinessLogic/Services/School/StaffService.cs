﻿using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
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
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StaffDataAccess(u).Insert(staffs));
        }
        public void Edit(IList<Staff> staffs)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StaffDataAccess(u).Update(staffs));
        }
        public void Delete(IList<Staff> staffs)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StaffDataAccess(u).Delete(staffs));
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
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StaffSchool>(u).Insert(staffSchools));
        }

        public void EditStaffSchools(IList<StaffSchool> staffSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StaffSchool>(u).Update(staffSchools));
        }

        public void DeleteStaffSchools(IList<StaffSchool> staffSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StaffSchool>(u).Delete(staffSchools));
        }

        public IList<StaffSchool> GetStaffSchools()
        {
            return DoRead(uow => new DataAccessBase<StaffSchool>(uow).GetAll());
        }
        
        public PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName,
            int start, int count)
        {
            schoolYearId = schoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            return DoRead(u => new StaffDataAccess(u).SearchStaff(schoolYearId, classId, studentId, filter, orderByFirstName,
                            start, count));
        }

    }
}
