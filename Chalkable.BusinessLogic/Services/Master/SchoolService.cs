using System;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ISchoolService
    {
        Data.Master.Model.School GetById(Guid id);
        Data.Master.Model.School GetByIdOrNull(Guid id);
        void Update(Data.Master.Model.School school);
        PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count);
        void Add(Guid districtId, int localId, string name);
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

        public void Add(Guid districtId, int localId, string name)
        {
            using (var uow = Update())
            {
                var school = new Data.Master.Model.School
                    {
                        DistrictRef = districtId,
                        Id = Guid.NewGuid(),
                        LocalId = localId,
                        Name = name
                    };
                new SchoolDataAccess(uow).Insert(school);
                uow.Commit();
            }
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
    }
}