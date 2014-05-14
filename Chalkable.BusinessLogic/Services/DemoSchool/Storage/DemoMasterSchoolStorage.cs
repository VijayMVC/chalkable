using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

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
                LocalId = 1,
                Name = "SMITH"
            };
        }

        public override void Setup()
        {
            var schoolId = Storage.Context.SchoolId.Value;
            Add(new List<Data.Master.Model.School>{CreateMasterSchool(schoolId)});

        }


    }
}
