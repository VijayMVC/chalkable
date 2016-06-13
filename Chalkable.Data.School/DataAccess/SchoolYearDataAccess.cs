using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolYearDataAccess : DataAccessBase<SchoolYear, int>
    {
        public SchoolYearDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public SchoolYear GetByDate(DateTime date, int schoolId)
        {
            var conds = new AndQueryCondition
                {
                    {SchoolYear.START_DATE_FIELD, date, ConditionRelation.LessEqual},
                    {SchoolYear.END_DATE_FIELD, date, ConditionRelation.GreaterEqual},
                    {SchoolYear.SCHOOL_REF_FIELD, schoolId},
                };
            return SelectOneOrNull<SchoolYear>(conds);
        }

        public SchoolYear GetLast(DateTime tillDate, int schoolId)
        {
            var conds = new AndQueryCondition
            {
                { SchoolYear.START_DATE_FIELD, tillDate, ConditionRelation.LessEqual },
                { SchoolYear.SCHOOL_REF_FIELD, schoolId, ConditionRelation.Equal },
                { SchoolYear.ARCHIVE_DATE, null, ConditionRelation.Equal}
            };
            var q = Orm.SimpleSelect<SchoolYear>(conds);
            q.Sql.AppendFormat("order by {0}  desc", SchoolYear.END_DATE_FIELD);
            return ReadOneOrNull<SchoolYear>(q);
        }
        
        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new SchoolYear {Id = x}).ToList());
        }

        public IList<SchoolYear> GetByIds(IList<int> ids, bool onlyActive = true)
        {
            StringBuilder sqlQuery = new StringBuilder("Select * From [SchoolYear] where Id in("+ids.JoinString(",")+")");
            if (onlyActive)
                sqlQuery.Append(" And ArchiveDate is null");
            return ReadMany<SchoolYear>(new DbQuery(sqlQuery, null));
        }

        public PaginatedList<SchoolYear> GetBySchool(int schoolId)
        {
            return PaginatedSelect< SchoolYear>(new SimpleQueryCondition("SchoolRef", schoolId, ConditionRelation.Equal), "Id", 0, int.MaxValue);
        }

        public StudentSchoolYear GetPreviousStudentSchoolYearOrNull(int studentId)
        {
            var @params = new Dictionary<string, object>
            {
                ["studentId"] = studentId
            };
            using (var reader = ExecuteStoredProcedureReader("spGetPreviousStudentSchoolYear", @params))
            {
                return reader.ReadOrNull<StudentSchoolYear>();
            }
        }
    }
}
