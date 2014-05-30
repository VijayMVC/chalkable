using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAlphaGradeStorage:BaseDemoIntStorage<AlphaGrade>
    {
        public DemoAlphaGradeStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<AlphaGrade> GetForClassStandarts(int classId)
        {
            /*
             * var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select AlphaGrade.* from AlphaGrade
                                       join GradingScaleRange on GradingScaleRange.[{0}] = AlphaGrade.[{1}]
                                       join ClassroomOption on ClassroomOption.[{2}] = GradingScaleRange.[{3}]"
                                     , GradingScaleRange.ALPHA_GRADE_REF_FIELD, AlphaGrade.ID_FIELD
                                     , ClassroomOption.STANDARD_GRADING_SCALE_REF_FIELD,
                                     GradingScaleRange.GRADING_SCALE_REF_FIELD);
            var conds = new AndQueryCondition {{ClassroomOption.ID_FIELD, classId}};
            conds.BuildSqlWhere(dbQuery, "ClassroomOption");
             */
            //throw new NotImplementedException();
            return new List<AlphaGrade>();
        }

        public IList<AlphaGrade> GetForClass(int classId)
        {
            /*var sql = @"select AlphaGrade.* from AlphaGrade
                        join GradingScaleRange on GradingScaleRange.[{0}] = AlphaGrade.[{1}]
                        join Class on Class.[{2}] = GradingScaleRange.[{3}]";
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(sql, GradingScaleRange.ALPHA_GRADE_REF_FIELD, AlphaGrade.ID_FIELD
                                     , Class.GRADING_SCALE_REF_FIELD, GradingScaleRange.GRADING_SCALE_REF_FIELD);
            var conds = new AndQueryCondition {{Class.ID_FIELD, classId}};
            conds.BuildSqlWhere(dbQuery, "Class");
            return ReadMany<AlphaGrade>(dbQuery);*/
            //throw new NotImplementedException();
            return new List<AlphaGrade>();
        }
    }
}
