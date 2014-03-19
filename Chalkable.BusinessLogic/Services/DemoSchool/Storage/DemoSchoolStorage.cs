using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolStorage
    {
        public void Add(Data.School.Model.School school)
        {
            throw new NotImplementedException();
        }

        public IList<Data.School.Model.School> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Data.Master.Model.School school)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count)
        {
            throw new NotImplementedException();
        }

        public void Add(IList<Data.School.Model.School> schools)
        {
            throw new NotImplementedException();
        }

        public void Update(IList<Data.School.Model.School> schools)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
