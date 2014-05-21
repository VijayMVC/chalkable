using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using ISchoolService = Chalkable.BusinessLogic.Services.School.ISchoolService;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoSchoolService : DemoSchoolServiceBase, ISchoolService
    {
        public DemoSchoolService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(Data.School.Model.School school)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            Storage.SchoolStorage.Add(school);
           
            ServiceLocator.ServiceLocatorMaster.SchoolService.Add(Context.DistrictId.Value, school.Id, school.Name);
        }

        public void Add(IList<Data.School.Model.School> schools)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            
            Storage.SchoolStorage.Add(schools);
            ServiceLocator.ServiceLocatorMaster.SchoolService.Add(schools.Select(x => new SchoolInfo
            {
                LocalId = x.Id,
                Name = x.Name
            }).ToList(), Context.DistrictId.Value);
        }

        public void Edit(IList<Data.School.Model.School> schools)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();

            Storage.SchoolStorage.Update(schools);
            ServiceLocator.ServiceLocatorMaster.SchoolService.Edit(schools.Select(x => new SchoolInfo
            {
                LocalId = x.Id,
                Name = x.Name
            }).ToList(), Context.DistrictId.Value);
        }

        public void Delete(IList<int> ids)
        {
            Storage.SchoolStorage.Delete(ids);
        }

        public IList<Data.School.Model.School> GetSchools()
        {
            return Storage.SchoolStorage.GetAll();
        }

        public void AddSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            Storage.SchoolOptionStorage.Add(schoolOptions);
        }

        public void EditSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            Storage.SchoolOptionStorage.Update(schoolOptions);
        }

        public void DeleteSchoolOptions(IList<int> ids)
        {
            Storage.SchoolOptionStorage.Delete(ids);
        }

        public SchoolOption GetSchoolOption()
        {
            if(!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException(); 
            return Storage.SchoolOptionStorage.GetById(Context.SchoolLocalId.Value);
        }
    }
}