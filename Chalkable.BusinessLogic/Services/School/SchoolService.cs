using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolService
    {
        void Add(Data.School.Model.School school);
        IList<Data.School.Model.School> GetSchools();
    }

    public class SchoolService : SchoolServiceBase, ISchoolService
    {
        public SchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(Data.School.Model.School school)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                da.Insert(school);
                uow.Commit();
            }
            ServiceLocator.ServiceLocatorMaster.SchoolService.Add(Context.DistrictId.Value, school.Id, school.Name);
        }

        public IList<Data.School.Model.School> GetSchools()
        {
            if (BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetAll();
            }
        }
    }
}