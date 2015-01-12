using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

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

        public void AddMasterSchool(Guid? id)
        {
            if (!id.HasValue)
                throw new ChalkableException("Not valid district id");
            Add(new List<Data.Master.Model.School> { CreateMasterSchool(id.Value) });
        }


    }
}
