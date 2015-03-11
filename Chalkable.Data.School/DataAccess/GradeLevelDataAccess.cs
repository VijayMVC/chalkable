﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradeLevelDataAccess : DataAccessBase<GradeLevel, int>
    {
        public GradeLevelDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x=>new GradeLevel{Id = x}).ToList());
        }
    }

    public class SchoolGradeLevelDataAccess: BaseSchoolDataAccess<SchoolGradeLevel>
    {
        public SchoolGradeLevelDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }

        public void DeleteSchoolGradeLevel(int gradeLevel)
        {
            SimpleDelete(new AndQueryCondition { { SchoolGradeLevel.GRADE_LEVEL_REF_FIELD, gradeLevel } });
        }
    }
}
