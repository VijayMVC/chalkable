using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AlphaGradeDataAccess : BaseSchoolDataAccess<AlphaGrade>
    {
        public AlphaGradeDataAccess(UnitOfWork unitOfWork, int? localSchoolId) : base(unitOfWork, localSchoolId)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x=> new AlphaGrade{Id = x}).ToList());
        }

        public IList<AlphaGrade> GetList()
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat("select AlphaGrade.* from AlphaGrade");
            var conds = FilterBySchool(null);
            conds.BuildSqlWhere(dbQuery, "AlphaGrade");
            dbQuery.Sql.AppendFormat("and exists(select * from GradingScaleRange where GradingScaleRange.[{0}]=AlphaGrade.[{1}])"
                , GradingScaleRange.ALPHA_GRADE_REF_FIELD, AlphaGrade.ID_FIELD);
            return ReadMany<AlphaGrade>(dbQuery);
        } 

        public IList<AlphaGrade> GetForClass(int classId)
        {
            var sql = @"select AlphaGrade.* from AlphaGrade
                        join GradingScaleRange on GradingScaleRange.[{0}] = AlphaGrade.[{1}]
                        join Class on Class.[{2}] = GradingScaleRange.[{3}]";
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(sql, GradingScaleRange.ALPHA_GRADE_REF_FIELD, AlphaGrade.ID_FIELD
                                     , Class.GRADING_SCALE_REF_FIELD, GradingScaleRange.GRADING_SCALE_REF_FIELD);
            var conds = new AndQueryCondition {{Class.ID_FIELD, classId}};
            conds.BuildSqlWhere(dbQuery, "Class");
            Orm.OrderBy(dbQuery, "GradingScaleRange", GradingScaleRange.HIGH_VALUE_FIELD, Orm.OrderType.Desc);
            return ReadMany<AlphaGrade>(dbQuery);
        }

        public IList<AlphaGrade> GetForClassStandards(int classId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select AlphaGrade.* from AlphaGrade
                                       join GradingScaleRange on GradingScaleRange.[{0}] = AlphaGrade.[{1}]
                                       join ClassroomOption on ClassroomOption.[{2}] = GradingScaleRange.[{3}]"
                                     , GradingScaleRange.ALPHA_GRADE_REF_FIELD, AlphaGrade.ID_FIELD
                                     , ClassroomOption.STANDARD_GRADING_SCALE_REF_FIELD,
                                     GradingScaleRange.GRADING_SCALE_REF_FIELD);
            var conds = new AndQueryCondition {{ClassroomOption.ID_FIELD, classId}};
            conds.BuildSqlWhere(dbQuery, "ClassroomOption");
            Orm.OrderBy(dbQuery, "GradingScaleRange", GradingScaleRange.HIGH_VALUE_FIELD, Orm.OrderType.Desc);
            return ReadMany<AlphaGrade>(dbQuery);
        }

    }
}
