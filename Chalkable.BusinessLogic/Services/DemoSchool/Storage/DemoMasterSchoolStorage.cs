using System;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoMasterSchoolStorage:BaseDemoGuidStorage<Data.Master.Model.School>
    {
        public DemoMasterSchoolStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count)
        {
            throw new NotImplementedException();
        }

        public Data.Master.Model.School GetByIdOrNull(Guid id)
        {
            return data.ContainsKey(id) ? data[id] : null;
        }

        public static Data.Master.Model.School CreateMasterSchool(Guid id)
        {
            return new Data.Master.Model.School
            {
                DistrictRef = id,
                Id = id,
                LocalId = DemoSchoolConstants.SchoolId,
                Name = "SMITH"
            };
        }

        public void AddMasterSchool()
        {
            /*if (!Storage.Context.SchoolLocalId.HasValue)
                throw new Exception("Context doesn't have valid school local id");
            var schoolId = Storage.Context.SchoolId.Value;
            Add(new List<Data.Master.Model.School>{CreateMasterSchool(schoolId)});*/
            throw new NotImplementedException();
        }


    }
}
