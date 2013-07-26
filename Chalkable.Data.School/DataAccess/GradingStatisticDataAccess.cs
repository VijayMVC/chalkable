using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public  class GradingStatisticDataAccess : StudentAnnouncementDataAccess
    {
        public GradingStatisticDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        private DbQuery BuilGradeStatisticQuery(StringBuilder sql, GradingStatisticQuery query)
        {
            var conds = new Dictionary<string, object>();
            if (query.ClassId.HasValue)
            {
                conds.Add(ClassPerson.CLASS_REF_FIELD, query.ClassId);
                sql.AppendFormat(" and [ClassPerson].[{0}] =@{0}", ClassPerson.CLASS_REF_FIELD);
            }
            if (query.TeacherId.HasValue)
            {
                conds.Add(Class.TEACHER_REF_FIELD, query.TeacherId);
                sql.AppendFormat(" and exists(select * from Class where [{0}] = [ClassPerson].[{1}] and [{2}] = @{2})"
                    , Class.ID_FIELD, ClassPerson.CLASS_REF_FIELD, Class.TEACHER_REF_FIELD);
            }
            if (query.Role == CoreRoles.STUDENT_ROLE.Id)
            {
                conds.Add(Person.ID_FIELD, query.CallerId);
                sql.AppendFormat(" and [Person].[{0}] = @{0}", Person.ID_FIELD);
            }
            if (query.MarkingPeriodIds != null && query.MarkingPeriodIds.Count > 0)
            {
                var mpidsStr = query.MarkingPeriodIds.Select(x => "'" + x.ToString() + "'").ToString();
                sql.AppendFormat(" and [MarkingPeriodClass].[{0}] in ({1})", MarkingPeriodClass.MARKING_PERIOD_REF_FIELD, mpidsStr);
            }
            if (query.SchoolYearId.HasValue)
            {
                conds.Add(Class.SCHOOL_YEAR_REF, query.SchoolYearId);
                sql.AppendFormat(" and exists(select * from Class where [{0}] = [ClassPerson].[{1}] and [{2}] = @{2})"
                                 , Class.ID_FIELD, ClassPerson.CLASS_REF_FIELD, Class.SCHOOL_YEAR_REF);
            }
            return new DbQuery { Sql = sql.ToString(), Parameters = conds };
        }

        private DbQuery BuildStudentGradeAvgPerMPCDbQuey(GradingStatisticQuery query)
        {
            var sql = new StringBuilder();
            var types = new List<Type> { typeof(Person), typeof(MarkingPeriodClass) };
            sql.AppendFormat(@"select {0}, dbo.fnCalcStudentGradeAvgForFinalGrade(Person.Id, MarkingPeriodClass.Id) as [Avg] 
                             from Person 
                             join ClassPerson on ClassPerson.PersonRef = Person.Id
                             join MarkingPeriodClass on MarkingPeriodClass.ClassRef = ClassPerson.ClassRef"
                             , Orm.ComplexResultSetQuery(types));
            sql.Append(" where 1=1 and ");
            return BuilGradeStatisticQuery(sql, query);
        }

        //TODO: add method to Orm for function building
        public IList<StudentGradeAvgPerMPC> CalcStudentGradeAvgPerMPC(GradingStatisticQuery query)
        {
            var dbQuery = BuildStudentGradeAvgPerMPCDbQuey(query);
            return ReadMany<StudentGradeAvgPerMPC>(dbQuery, true);
        }

        public IList<StudentGradeAvgPerClass> CalcStudentGradeAvgPerClass(GradingStatisticQuery query)
        {
            var innerQuery = BuildStudentGradeAvgPerMPCDbQuey(query);
            var personType = typeof (Person);
            var mpcType = typeof (MarkingPeriodClass);
            var fields = new List<string>
                {
                    Orm.FullFieldName(personType, Person.ID_FIELD),
                    Orm.FullFieldName(personType, Person.FIRST_NAME_FIELD),
                    Orm.FullFieldName(personType, Person.LAST_NAME_FIELD),
                    Orm.FullFieldName(personType, Person.GENDER_FIELD),
                    Orm.FullFieldName(mpcType, MarkingPeriodClass.CLASS_REF_FIELD)
                };
            var resultSetStr = fields.Select(x => string.Format("x.{0} as {0}", x)).JoinString(",");
            fields.Add(Orm.FullFieldName(mpcType, MarkingPeriodClass.ID_FIELD));
            var groupBySetStr = fields.Select(x => string.Format("x.{0}", x)).JoinString(",");
            var sql = @"select {1}, AVG(x.[Avg]) as [Avg]                                
                        from ({0})x  group by {2}";
            sql = string.Format(sql, innerQuery, resultSetStr, groupBySetStr);

            return ReadMany<StudentGradeAvgPerClass>(new DbQuery {Sql = sql, Parameters = innerQuery.Parameters}, true);
        }
    }

    public class GradingStatisticQuery
    {
        public IList<Guid> MarkingPeriodIds { get; set; }
        public Guid? SchoolYearId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid CallerId { get; set; }
        public int Role { get; set; }
    }
}
