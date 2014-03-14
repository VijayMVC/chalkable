using System;
using System.Collections.Generic;
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
        void Add(IList<Data.Master.Model.School> schools);
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
            Add(new List<Data.Master.Model.School>{new Data.Master.Model.School
                    {
                        DistrictRef = districtId,
                        Id = Guid.NewGuid(),
                        LocalId = localId,
                        Name = name
                    }});
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


        public void Add(IList<Data.Master.Model.School> schools)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolDataAccess(uow).Insert(schools);
                uow.Commit();
            }
        }
    }
}