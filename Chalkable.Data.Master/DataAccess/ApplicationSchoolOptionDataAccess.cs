﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ApplicationSchoolOptionDataAccess : DataAccessBase<ApplicationSchoolOption>
    {
        public ApplicationSchoolOptionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void BanSchoolsByIds(Guid applicationId, IList<Guid> schoolIds)
        {
            var @params = new Dictionary<string, object>
            {
                ["@applicationId"] = applicationId,
                ["@schoolIds"] = schoolIds
            };
            ExecuteStoredProcedure("spBanSchoolsByIds", @params);
        }
    }
}