using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class DistrictDataAccess : DataAccessBase<District>
    {
        public DistrictDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}