using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.DataAccess;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ISchoolService
    {
        Data.Master.Model.School GetById(Guid id);
        Data.Master.Model.School GetByIdOrNull(Guid id);
        void Update(Data.Master.Model.School school);
        PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count);
        IList<Data.Master.Model.School> GetAll();
        void Add(Guid districtId, int localId, string name);
        void Add(IList<SchoolInfo> schools, Guid districtId);
        void Edit(IList<SchoolInfo> schoolInfos, Guid districtId);
        void Delete(IList<int> localIds, Guid districtId);
    }

    public class SchoolService : MasterServiceBase, ISchoolService
    {
        public SchoolService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public void Update(Data.Master.Model.School school)
        {
            using (var uow = Update())
            {
                new SchoolDataAccess(uow).Update(school);
                uow.Commit();
            }
        }

        public PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count)
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetSchools(districtId, start, count);
            }
        }

        public IList<Data.Master.Model.School> GetAll()
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetAll();
            }
        }

        public void Add(Guid districtId, int localId, string name)
        {
            Add(new List<SchoolInfo>{new SchoolInfo
                    {
                        LocalId = localId,
                        Name = name
                    }}, districtId);
        }


        //TODO: add district data to school
        public Data.Master.Model.School GetById(Guid id)
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetById(id);
            }
        }

        public Data.Master.Model.School GetByIdOrNull(Guid id)
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetByIdOrNull(id);
            }
        }


        public void Add(IList<SchoolInfo> schools, Guid districtId)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolDataAccess(uow).Insert(schools.Select(x => new Data.Master.Model.School
                    {
                        Name = x.Name,
                        LocalId = x.LocalId,
                        DistrictRef = districtId,
                        Id = Guid.NewGuid()
                    }).ToList());
                uow.Commit();
            }
        }

        public void Edit(IList<SchoolInfo> schoolInfos, Guid districtId)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                var schools = da.GetSchools(districtId, 0, int.MaxValue).ToList();
                schools = schools.Where(x => schoolInfos.Any(y => y.LocalId == x.LocalId)).ToList();
                foreach (var school in schools)
                {
                   var si = schoolInfos.FirstOrDefault(x=>x.LocalId == school.LocalId);
                    if (si != null)
                        school.Name = si.Name;
                }
                da.Update(schools);
                uow.Commit();
            }
        }

        public void Delete(IList<int> localIds, Guid districtId)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                var schools = da.GetSchools(districtId, 0, int.MaxValue).ToList();
                schools = schools.Where(x => localIds.Contains(x.LocalId)).ToList();
                da.Delete(schools.Select(x=>x.Id).ToList());
                uow.Commit();
            }
        }
    }

    public class SchoolInfo
    {
        public int LocalId { get; set; }
        public string Name { get; set; }
    }
}