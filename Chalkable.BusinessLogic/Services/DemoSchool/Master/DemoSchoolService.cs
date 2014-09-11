using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;


namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoSchoolService : DemoMasterServiceBase, Services.Master.ISchoolService
    {
        public DemoSchoolService(IServiceLocatorMaster serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public void Update(Data.Master.Model.School school)
        {
            Storage.MasterSchoolStorage.Update(school);
        }

        public PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count)
        {
            return Storage.MasterSchoolStorage.GetSchools(districtId, start, count);
        }

        public IList<Data.Master.Model.School> GetAll()
        {
            return Storage.MasterSchoolStorage.GetAll();
        }

        public void Add(Guid districtId, int localId, string name)
        {

            var school = new Data.Master.Model.School
            {
                DistrictRef = districtId,
                Id = Guid.NewGuid(),
                LocalId = localId,
                Name = name
            };

            Storage.MasterSchoolStorage.Add(school);
        }

        public void Add(IList<SchoolInfo> schools, Guid districtId)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.MasterSchoolStorage.Add(schools.Select(x => new Data.Master.Model.School
            {
                Name = x.Name,
                LocalId = x.LocalId,
                DistrictRef = districtId,
                Id = Guid.NewGuid()
            }).ToList());
        }

        public void Edit(IList<SchoolInfo> schoolInfos, Guid districtId)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            var schools = Storage.MasterSchoolStorage.GetSchools(districtId, 0, int.MaxValue).ToList();
            schools = schools.Where(x => schoolInfos.Any(y => y.LocalId == x.LocalId)).ToList();
            foreach (var school in schools)
            {
                var si = schoolInfos.FirstOrDefault(x => x.LocalId == school.LocalId);
                if (si != null)
                    school.Name = si.Name;
            }
            Storage.MasterSchoolStorage.Update(schools);
        }

        public void Delete(IList<int> localIds, Guid districtId)
        {
            throw new NotImplementedException();
        }

        public void Add(IList<Data.Master.Model.School> schools)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.MasterSchoolStorage.Add(schools);
        }
        
        public Data.Master.Model.School GetById(Guid districtRef, int localId)
        {
            throw new NotImplementedException();
        }

        public Data.Master.Model.School GetByIdOrNull(Guid id)
        {
            return Storage.MasterSchoolStorage.GetByIdOrNull(id);
        }
    }
}