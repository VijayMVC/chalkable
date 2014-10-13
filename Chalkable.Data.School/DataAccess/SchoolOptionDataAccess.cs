﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolOptionDataAccess : DataAccessBase<SchoolOption,int>
    {
        public SchoolOptionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new SchoolOption {Id = x}).ToList());
        }
    }
}
