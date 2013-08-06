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


        private DbQuery BuildGradeStatisicDbQuery(StringBuilder sql, GradingStatisticQuery query)
        {
            var conds = new Dictionary<string, object>();
            if (query.ClassId.HasValue)
            {
                conds.Add(ClassPerson.CLASS_REF_FIELD, query.ClassId);
                sql.AppendFormat(" and [Class].[{0}] = @{0}", Class.ID_FIELD);
            }
            if (query.TeacherId.HasValue)
            {
                conds.Add(Class.TEACHER_REF_FIELD, query.TeacherId);
                sql.AppendFormat(" and [Class].[{0}] = @{0}", Class.TEACHER_REF_FIELD);
            }
            if (query.SchoolYearId.HasValue)
            {
                conds.Add(Class.SCHOOL_YEAR_REF, query.SchoolYearId);
                sql.AppendFormat(" and [Class].[{0}] = @{0}", Class.SCHOOL_YEAR_REF);
            }
            if (query.MarkingPeriodIds != null && query.MarkingPeriodIds.Count > 0)
            {
                var mpidsStr = query.MarkingPeriodIds.Select(x => "'" + x.ToString() + "'").ToString();
                sql.AppendFormat(" and [MarkingPeriodClass].[{0}] in ({1})", MarkingPeriodClass.MARKING_PERIOD_REF_FIELD, mpidsStr);
            }
            return new DbQuery { Sql = sql.ToString(), Parameters = conds };
        }

        private DbQuery BuilStudentGradeStatisticQuery(StringBuilder sql, GradingStatisticQuery query)
        {
            var dbQuery = BuildGradeStatisicDbQuery(sql, query);
            sql.Append(dbQuery.Sql);
            var conds = dbQuery.Parameters;
            if (query.StudentId.HasValue)
            {
                conds.Add("@studentId", query.StudentId);
                sql.AppendFormat(" and [Person].[{0}] = @studentId", Person.ID_FIELD);
            }
            if (query.Role == CoreRoles.STUDENT_ROLE.Id)
            {
                conds.Add(Person.ID_FIELD, query.CallerId);
                sql.AppendFormat(" and [Person].[{0}] = @{0}", Person.ID_FIELD);
            }
            return new DbQuery { Sql = sql.ToString(), Parameters = conds };
        }
    
        private DbQuery BuildClassGradeStatisticQuery(StringBuilder sql, GradingStatisticQuery query)
        {
            var dbQuery = BuildGradeStatisicDbQuery(sql, query);
            sql.Append(dbQuery.Sql);
            var conds = dbQuery.Parameters;
            var personExistsingQueryTmp = string.Format("and exists(select * from [ClassPerson] where [{1}] = [Class].[{2}] [{0}] =@"
                              , ClassPerson.PERSON_REF_FIELD, ClassPerson.CLASS_REF_FIELD, Class.ID_FIELD);
            personExistsingQueryTmp += "{3})";
            if (query.StudentId.HasValue)
            {
                conds.Add("@studentId", query.StudentId);
                sql.AppendFormat(personExistsingQueryTmp, "@studentId");
            }
            if (query.Role == CoreRoles.STUDENT_ROLE.Id)
            {
                conds.Add(Person.ID_FIELD, query.CallerId);
                sql.AppendFormat(personExistsingQueryTmp, Person.ID_FIELD);
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
                             join Class on Class.Id = ClassPerson.ClassRef
                             join MarkingPeriodClass on MarkingPeriodClass.ClassRef = Class.Id"
                             , Orm.ComplexResultSetQuery(types));
            sql.Append(" where 1=1 and ");
            return BuilStudentGradeStatisticQuery(sql, query);
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

        public IList<MarkingPeriodClassGradeAvg> CalcClassGradingPerMp(GradingStatisticQuery query)
        {
            var types = new List<Type> {typeof (Class), typeof(MarkingPeriod)};
            var sql = new StringBuilder();
            sql.AppendFormat(@"select {0}, dbo.fnCalcClassGradeAvgPerMP(MarkingPeriodClass.Id) as [Avg] 
                               from Class 
                               join MarkingPeriodClass on MarkingPeriodClass.ClassRef = Class.Id
                               join MarkingPeriod on MarkingPeriod.Id = MarkingPeriodClass.MarkingPeriodRef"
                , Orm.ComplexResultSetQuery(types));
            sql.Append(" where 1=1 ");
            var dbQuery = BuildClassGradeStatisticQuery(sql, query);
            return ReadMany<MarkingPeriodClassGradeAvg>(dbQuery, true);
        }

        public IList<StudentGradeAvgPerDate> CalcStudentGradeStats(Guid studentId, Guid markingPeriodId, Guid? classId, int dayInterval)
        {
            var parameters = new Dictionary<string, object>()
                {
                    {"studentId", studentId},
                    {"markingPeriodId", markingPeriodId},
                    {"classId", classId},
                    {"dayInterval", dayInterval},
                };
            using (var reader = ExecuteStoredProcedureReader("spCalcStudentSummaryGradeStatsPerDate", parameters))
            {
                return reader.ReadList<StudentGradeAvgPerDate>();
            }
        }

        public IList<StudentClassGradeStats> CalcStudentClassGradeStats(Guid classId, Guid markingPeriodId, Guid? studentId, int dayInterval)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"studentId", studentId},
                    {"markingPeriodId", markingPeriodId},
                    {"classId", classId},
                    {"dayInterval", dayInterval},
                };
            using (var reader = ExecuteStoredProcedureReader("spCalcStudentClassGradeStatsPerDate", parameters))
            {
                var stDic = new Dictionary<Guid, IDictionary<DateTime, ClassGradeAvgPerDate>>();
                while (reader.Read())
                {
                    var stId = SqlTools.ReadGuid(reader, StudentClassGradeStats.STUDENT_ID_FEILD);
                    if(!stDic.ContainsKey(stId))
                        stDic.Add(stId, new Dictionary<DateTime, ClassGradeAvgPerDate>());
                    var avgPerDate = reader.Read<ClassGradeAvgPerDate>();
                    avgPerDate.ClassId = classId;
                    avgPerDate.AnnTypeGradeAvgs = new List<AnnTypeGradeAvg>();
                    stDic[stId].Add(avgPerDate.Date, avgPerDate);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    var stId = SqlTools.ReadGuid(reader, StudentClassGradeStats.STUDENT_ID_FEILD);
                    var date = SqlTools.ReadDateTime(reader, GradeAvgPerDate.DATE_FIELD);
                    stDic[stId][date].AnnTypeGradeAvgs.Add(reader.Read<AnnTypeGradeAvg>());
                }
                return stDic.Select(x=> new StudentClassGradeStats
                    {
                        StudentId = x.Key,
                        GradeAvgPerDates = x.Value.Select(y=>y.Value).ToList()
                    }).ToList();
            }
        }
    }

    public class GradingStatisticQuery
    {
        public IList<Guid> MarkingPeriodIds { get; set; }
        public Guid? SchoolYearId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid CallerId { get; set; }
        public int Role { get; set; }
    }
}
