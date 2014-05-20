using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolService
    {
        void Add(Data.School.Model.School school);
        void Add(IList<Data.School.Model.School> schools);
        void Edit(IList<Data.School.Model.School> schools);
        void Delete(IList<int> ids);
        IList<Data.School.Model.School> GetSchools();

        void AddSchoolOptions(IList<SchoolOption> schoolOptions);
        void EditSchoolOptions(IList<SchoolOption> schoolOptions);
        void DeleteSchoolOptions(IList<int> ids);
    }

    public class SchoolService : SchoolServiceBase, ISchoolService
    {
        public SchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(Data.School.Model.School school)
        {
            Add(new List<Data.School.Model.School> {school});
        }

        public IList<Data.School.Model.School> GetSchools()
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetAll();
            }
        }

        public void Add(IList<Data.School.Model.School> schools)
        {
            var schoolInfos = schools.Select(x => new SchoolInfo
                {
                    LocalId = x.Id,
                    Name = x.Name
                }).ToList();
            ModifySchool(da => da.Insert(schools), (iSchoolS, districtId) => iSchoolS.Add(schoolInfos, districtId));
        }

        public void Edit(IList<Data.School.Model.School> schools)
        {
            var schoolInfos = schools.Select(x => new SchoolInfo
                {
                    LocalId = x.Id,
                    Name = x.Name
                }).ToList();
            ModifySchool(da => da.Update(schools), (iSchoolS, districtId) => iSchoolS.Edit(schoolInfos, districtId));
        }

        public void Delete(IList<int> ids)
        {
            ModifySchool(da => da.Delete(ids), (schoolS, districtId) => schoolS.Delete(ids, districtId));
        }

        private void ModifySchool(Action<SchoolDataAccess> modifySchool, Action<Master.ISchoolService, Guid> modifyMasterSchool)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                modifySchool(da);
                uow.Commit();
            }
            modifyMasterSchool(ServiceLocator.ServiceLocatorMaster.SchoolService, Context.DistrictId.Value);
        }

        public void AddSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            ModifySchoolOptions(da=>da.Insert(schoolOptions));
        }
        public void EditSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            ModifySchoolOptions(da => da.Update(schoolOptions));
        }
        public void DeleteSchoolOptions(IList<int> ids)
        {
            ModifySchoolOptions(da=>da.Delete(ids));
        }
        private void ModifySchoolOptions(Action<SchoolOptionDataAccess> modify)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new SchoolOptionDataAccess(uow);
                modify(da);
                uow.Commit();
            }
        }
    }
}