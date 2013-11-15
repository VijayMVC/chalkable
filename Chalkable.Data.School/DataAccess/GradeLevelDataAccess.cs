using System.Collections.Generic;
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

        public IList<GradeLevel> GetGradeLevels(int? schoolId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append("select * from GradeLevel ");
            if (schoolId.HasValue)
            {
                dbQuery.Parameters.Add(SchoolGradeLevel.SCHOOL_REF_FIELD, schoolId);
                dbQuery.Sql.AppendFormat(" where Id in (select [{0}].{1} from [{0}] where [{0}].[{2}] = @{2})"
                                         , "SchoolGradeLevel", SchoolGradeLevel.GRADE_LEVEL_REF_FIELD, SchoolGradeLevel.SCHOOL_REF_FIELD);
            }
            return ReadMany<GradeLevel>(dbQuery);
        }
    }

    public class SchoolGradeLevelDataAccess: BaseSchoolDataAccess<SchoolGradeLevel>
    {
        public SchoolGradeLevelDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }

        public void DeleteSchoolGradeLevel(int gradeLevel)
        {
            SimpleDelete<SchoolGradeLevel>(new AndQueryCondition { { SchoolGradeLevel.GRADE_LEVEL_REF_FIELD, gradeLevel } });
        }
        public bool Exists(int gradeLevel)
        {
            return Exists<SchoolGradeLevel>(new AndQueryCondition {{SchoolGradeLevel.GRADE_LEVEL_REF_FIELD, gradeLevel}});
        }
    }
}
