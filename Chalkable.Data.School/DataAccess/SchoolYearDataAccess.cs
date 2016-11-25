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

        public IList<SchoolYear> GetPreviousSchoolYears(DateTime currentSyStartDate, int schoolId, int count = 1)
        {
            var conds = new AndQueryCondition
            {
                {SchoolYear.END_DATE_FIELD, currentSyStartDate, ConditionRelation.LessEqual },
                {SchoolYear.SCHOOL_REF_FIELD, schoolId, ConditionRelation.Equal }
            };

            var dbQuery = Orm.SimpleSelect<SchoolYear>(conds, count);
            Orm.OrderBy(dbQuery, nameof(SchoolYear), SchoolYear.END_DATE_FIELD, Orm.OrderType.Desc);

            return ReadMany<SchoolYear>(dbQuery);
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new SchoolYear {Id = x}).ToList());
        }

        public IList<SchoolYear> GetByIds(IList<int> ids, bool onlyActive = true)
        {
            if(ids == null || ids.Count == 0)
                return new List<SchoolYear>();

            var @params = new Dictionary<string, object> { ["ids"] = ids };
            var sqlQuery = new StringBuilder($"Select * From [{nameof(SchoolYear)}] where {SchoolYear.ID_FIELD} in(Select * From @ids)");
            if (onlyActive)
                sqlQuery.Append($" And {SchoolYear.ARCHIVE_DATE} is null");

            return ReadMany<SchoolYear>(new DbQuery(sqlQuery, @params));
        }

        public IList<SchoolYear> GetByAcadYears(IList<int> years, bool onlyActive = true)
        {
            if (years == null || years.Count == 0)
                return new List<SchoolYear>();

            var conds = new AndQueryCondition();
            if (onlyActive)
                conds.Add(SchoolYear.ARCHIVE_DATE, null);
            var q = Orm.SimpleSelect<SchoolYear>(conds);
            q.Sql.Append($" And {SchoolYear.ACAD_YEAR_FIELD} in (Select * From @years)");
            q.Parameters.Add("years", years);

            return ReadMany<SchoolYear>(q);
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

        public StudentSchoolYear GetStudentSchoolYear(int studentId, int schoolYearId)
        {
            var queryConditions = new AndQueryCondition
            {
                {StudentSchoolYear.STUDENT_FIELD_REF_FIELD, studentId, ConditionRelation.Equal },
                {StudentSchoolYear.SCHOOL_YEAR_REF_FIELD, schoolYearId, ConditionRelation.Equal }

            };

            var dbQuery = Orm.SimpleSelect<StudentSchoolYear>(queryConditions);

            return ReadOneOrNull<StudentSchoolYear>(dbQuery);
        }

        public IList<SchoolYear> GetSchoolYearsByStudent(int studentId, StudentEnrollmentStatusEnum? enrollmentStatus)
        {
            var conds = new AndQueryCondition
            {
                {StudentSchoolYear.STUDENT_FIELD_REF_FIELD, studentId},
            };
            if (enrollmentStatus.HasValue)
                conds.Add(StudentSchoolYear.ENROLLMENT_STATUS_FIELD, (int) enrollmentStatus.Value);
            var query = Orm.SimpleSelect<StudentSchoolYear>(conds);
            var studentSys = ReadMany<StudentSchoolYear>(query);
            return studentSys.Count > 0 
                ? GetByIds(studentSys.Select(x => x.SchoolYearRef).ToList()) 
                : new List<SchoolYear>();
        }
    }
}
