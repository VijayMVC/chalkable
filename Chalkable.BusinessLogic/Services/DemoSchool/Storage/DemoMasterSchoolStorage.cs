using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoMasterSchoolStorage:BaseDemoStorage<Guid, Data.Master.Model.School>
    {
        public DemoMasterSchoolStorage(DemoStorage storage) : base(storage)
        {
        }

        public PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count)
        {
            throw new NotImplementedException();
        }

        public void Add(Data.Master.Model.School school)
        {
            if (!data.ContainsKey(school.Id))
                data[school.Id] = school;
        }

        public Data.Master.Model.School GetByIdOrNull(Guid id)
        {
            return data.ContainsKey(id) ? data[id] : null;
        }

        public void Add(IList<Data.Master.Model.School> schools)
        {
            foreach (var school in schools)
            {
                Add(school);
            }
        }

        public void Update(List<Data.Master.Model.School> schools)
        {
            foreach (var school in schools)
            {
                Update(school);
            }
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

        public void Update(Data.Master.Model.School school)
        {
            if (data.ContainsKey(school.Id))
                data[school.Id] = school;
        }


        public override void Setup()
        {
            var schoolId = Storage.Context.SchoolId.Value;
            Add(new List<Data.Master.Model.School>{CreateMasterSchool(schoolId)});

        }


    }
}
