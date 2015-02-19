﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ABToCCMappingDataAccess : DataAccessBase<ABToCCMapping, Guid>
    {
        public ABToCCMappingDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
