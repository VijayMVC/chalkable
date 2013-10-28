﻿using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentSchoolYearDataAccess : DataAccessBase<StudentSchoolYear, int>
    {
        public StudentSchoolYearDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
