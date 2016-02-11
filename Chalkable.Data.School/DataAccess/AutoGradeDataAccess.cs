using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AutoGradeDataAccess : DataAccessBase<AutoGrade, int>
    {
        public AutoGradeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AutoGrade GetAutoGrade(int announcementApplicationId, int studentId)
        {
            var res = BuildAutoGradesSelect();
            var conds = new AndQueryCondition
                {
                    {AutoGrade.ANNOUNCEMENT_APPLICATION_REF_FIELD, announcementApplicationId},
                    {AutoGrade.STUDENT_REF_FIELD, studentId}
                };
            conds.BuildSqlWhere(res, typeof (AutoGrade).Name);
            return ReadOneOrNull<AutoGrade>(res, true);
        }

        public void DiscardAutoGrades(IList<AutoGrade> autoGrades)
        {
            SimpleDelete<AutoGrade>(autoGrades);
        }

        public IList<AutoGrade> GetAutoGrades(int announcementApplicationId)
        {
            var res = BuildAutoGradesSelect();
            var conds = new AndQueryCondition
                {
                    {AutoGrade.ANNOUNCEMENT_APPLICATION_REF_FIELD, announcementApplicationId}
                };
            conds.BuildSqlWhere(res, typeof (AutoGrade).Name);
            return ReadMany<AutoGrade>(res);
        }

        public IList<AutoGrade> GetAutoGradesByAnnouncementId(int announcementId)
        {
            var query = BuildAutoGradesSelect();
            var conds = new AndQueryCondition {{nameof(AnnouncementApplication.AnnouncementRef), announcementId}};
            conds.BuildSqlWhere(query, typeof (AnnouncementApplication).Name);
            return ReadMany<AutoGrade>(query, true);
        } 

        private DbQuery BuildAutoGradesSelect()
        {
            var query = new DbQuery();
            var types = new List<Type> { typeof(AutoGrade), typeof(AnnouncementApplication) };
            query.Sql.AppendFormat(Orm.SELECT_FORMAT, Orm.ComplexResultSetQuery(types), types[0].Name)
                 .Append(" ").AppendFormat(Orm.SIMPLE_JOIN_FORMAT, types[1].Name, nameof(AnnouncementApplication.Id)
                                           , types[0].Name, AutoGrade.ANNOUNCEMENT_APPLICATION_REF_FIELD);
            query.Sql.Append(" ");
            return query;
        }
    }
}
