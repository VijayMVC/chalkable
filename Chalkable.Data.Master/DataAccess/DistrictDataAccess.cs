using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class DistrictDataAccess : DataAccessBase
    {
        public DistrictDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(District district)
        {
            SimpleInsert(district);
        }

        public District GetById(Guid id)
        {
            return SelectOne<District>(new Dictionary<string, object> {{"Id", id}});
        }

        public IList<School> GetSchools(bool empty)
        {
            return SelectMany<School>(new Dictionary<string, object> {{School.IS_EMPTY_FIELD, empty}});
        }
    }
}