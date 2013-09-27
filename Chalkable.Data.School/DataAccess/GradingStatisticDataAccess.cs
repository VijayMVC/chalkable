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
                conds.Add(Class.ID_FIELD, query.ClassId);
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
                var mpidsStr = query.MarkingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
                sql.AppendFormat(" and [MarkingPeriodClass].[{0}] in ({1})", MarkingPeriodClass.MARKING_PERIOD_REF_FIELD, mpidsStr);
            }
            return new DbQuery (sql, conds);
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
            return new DbQuery(sql, conds);
        }
    
        private DbQuery BuildClassGradeStatisticQuery(StringBuilder sql, GradingStatisticQuery query)
        {
            var dbQuery = BuildGradeStatisicDbQuery(sql, query);
            var conds = dbQuery.Parameters;
            var personExistsingQueryTmp = string.Format(" and exists(select * from [ClassPerson] where [{1}] = [Class].[{2}] [{0}] =@"
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
            return new DbQuery (sql, conds);
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

            return ReadMany<StudentGradeAvgPerClass>(new DbQuery (sql, innerQuery.Parameters), true);
        }

        public IList<MarkingPeriodClassGradeAvg> CalcClassGradingPerMp(GradingStatisticQuery query)
        {
            var types = new List<Type> {typeof (Class), typeof(MarkingPeriod), typeof(MarkingPeriodClass)};
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

        public IList<DepartmentGradeAvg> CalcDepartmentGradeAvgPerMp(Guid markingPeriodId, Guid callerId, int role, IList<Guid> gradeLevels)
        {
            var query = new GradingStatisticQuery
                {
                    CallerId = callerId,
                    Role = role,
                    MarkingPeriodIds = new List<Guid> {markingPeriodId}
                };
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select Course.ChalkableDepartmentRef,
                                       AVG(dbo.fnCalcClassGradeAvgPerMP(MarkingPeriodClass.Id)) as [Avg]
                                 from Course 
                                 join Class on Class.CourseRef = Course.Id
                                 join MarkingPeriodClass on MarkingPeriodClass.ClassRef = Class.Id ");
            dbQuery.Sql.Append(" where Course.ChalkableDepartmentRef is not null ");
            if (gradeLevels != null && gradeLevels.Count > 0)
            {
                var gradeLevelsStr = gradeLevels.Select(x => "'" + x.ToString() + "'").JoinString(",");
                dbQuery.Sql.AppendFormat(" and Class.GradeLevelRef in ({0})", gradeLevelsStr);
            }
            dbQuery = BuildClassGradeStatisticQuery(dbQuery.Sql, query);
            dbQuery.Sql.Append(" group by Course.ChalkableDepartmentRef");
            return ReadMany<DepartmentGradeAvg>(dbQuery);
        } 

        public IList<StudentGradeAvgPerDate> CalcStudentGradeStatsPerDate(Guid studentId, Guid markingPeriodId, Guid? classId, int dayInterval)
        {
            var parameters = new Dictionary<string, object>
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
                var stDic = new Dictionary<Guid, StudentClassGradeStats>();
                while (reader.Read())
                {
                    var stId = SqlTools.ReadGuid(reader, StudentClassGradeStats.STUDENT_ID_FEILD);
                    if(!stDic.ContainsKey(stId))
                        stDic.Add(stId, new StudentClassGradeStats
                            {
                                ClassId = classId,
                                StudentId = stId,
                                GradeAvgPerDates = new List<GradeAvgPerDate>(),
                                AnnTypesGradeStats = new List<AnnTypeGradeStats>()
                            });
                    stDic[stId].GradeAvgPerDates.Add(reader.Read<GradeAvgPerDate>()); 
                }
                reader.NextResult();
                var anntypeDic = new Dictionary<Guid, IDictionary<int, AnnTypeGradeStats>>();
                while (reader.Read())
                {
                    var stId = SqlTools.ReadGuid(reader, StudentClassGradeStats.STUDENT_ID_FEILD);
                    var annType = SqlTools.ReadInt32(reader, AnnTypeGradeStats.ANNOUNCEMENT_TYPE_ID_FIELD);
                    if(!anntypeDic.ContainsKey(stId))
                        anntypeDic.Add(stId, new Dictionary<int, AnnTypeGradeStats>());
                    if(anntypeDic[stId].ContainsKey(annType))
                        anntypeDic[stId].Add(annType, new AnnTypeGradeStats
                            {
                                AnnouncementTypeId = annType,
                                GradeAvgPerDates = new List<GradeAvgPerDate>()
                            });
                     anntypeDic[stId][annType].GradeAvgPerDates.Add(reader.Read<GradeAvgPerDate>());
                }
                foreach (var keyValue in stDic.Where(keyValue => anntypeDic.ContainsKey(keyValue.Key)))
                {
                    keyValue.Value.AnnTypesGradeStats = anntypeDic[keyValue.Key].Values.ToList();
                }
                return stDic.Values.ToList();
            }
        }
   
        public IList<ClassPersonGradingStats> CalcGradingStats(Guid callerId, int role, Guid studentId, Guid markingPeriodId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"studentId", studentId},
                    {"markingPeriodId", markingPeriodId},
                    {"callerId", callerId},
                    {"role", role},
                };
            using (var reader = ExecuteStoredProcedureReader("spCalcGradingStats", parameters))
            {
                return reader.ReadList<ClassPersonGradingStats>();
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
