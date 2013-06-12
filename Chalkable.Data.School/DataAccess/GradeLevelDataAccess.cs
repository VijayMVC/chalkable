using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradeLevelDataAccess : DataAccessBase
    {
        public GradeLevelDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<GradeLevel> GetGradeLeveles()
        {
            return SelectMany<GradeLevel>();
        }
    }
}
