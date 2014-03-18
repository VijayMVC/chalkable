using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoMasterSchoolStorage
    {
        public PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count)
        {
            throw new NotImplementedException();
        }

        public IList<Data.Master.Model.School> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Add(Data.Master.Model.School school)
        {
            throw new NotImplementedException();
        }

        public Data.Master.Model.School GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Data.Master.Model.School GetByIdOrNull(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(IList<Data.Master.Model.School> schools)
        {
            throw new NotImplementedException();
        }

        public void Update(List<Data.Master.Model.School> schools)
        {
            throw new NotImplementedException();
        }
    }
}
