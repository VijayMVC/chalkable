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


        private DbQuery BuildGradeStatisicDbQuery(DbQuery dbQuery, GradingStatisticQuery query)
        {
            if (query.ClassId.HasValue && !dbQuery.Parameters.ContainsKey(Class.ID_FIELD))
            {
                dbQuery.Parameters.Add(Class.ID_FIELD, query.ClassId);
                dbQuery.Sql.AppendFormat(" and [Class].[{0}] = @{0}", Class.ID_FIELD);
            }
            if (query.TeacherId.HasValue && !dbQuery.Parameters.ContainsKey(Class.TEACHER_REF_FIELD))
            {
                dbQuery.Parameters.Add(Class.TEACHER_REF_FIELD, query.TeacherId);
                dbQuery.Sql.AppendFormat(" and [Class].[{0}] = @{0}", Class.TEACHER_REF_FIELD);
            }
            if (query.SchoolYearId.HasValue && !dbQuery.Parameters.ContainsKey(Class.SCHOOL_YEAR_REF))
            {
                dbQuery.Parameters.Add(Class.SCHOOL_YEAR_REF, query.SchoolYearId);
                dbQuery.Sql.AppendFormat(" and [Class].[{0}] = @{0}", Class.SCHOOL_YEAR_REF);
            }
            if (query.MarkingPeriodIds != null && query.MarkingPeriodIds.Count > 0)
            {
                var mpidsStr = query.MarkingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
                dbQuery.Sql.AppendFormat(" and [MarkingPeriodClass].[{0}] in ({1})", MarkingPeriodClass.MARKING_PERIOD_REF_FIELD, mpidsStr);
            }
            return dbQuery;
        }

        private DbQuery BuilStudentGradeStatisticQuery(DbQuery dbQuery, GradingStatisticQuery query)
        {
            dbQuery = BuildGradeStatisicDbQuery(dbQuery, query);
            if (query.StudentId.HasValue && !dbQuery.Parameters.ContainsKey("@studentId"))
            {
                dbQuery.Parameters.Add("@studentId", query.StudentId);
                dbQuery.Sql.AppendFormat(" and [Person].[{0}] = @studentId", Person.ID_FIELD);
            }
            if (query.Role == CoreRoles.STUDENT_ROLE.Id && !dbQuery.Parameters.ContainsKey(Person.ID_FIELD))
            {
                dbQuery.Parameters.Add(Person.ID_FIELD, query.CallerId);
                dbQuery.Sql.AppendFormat(" and [Person].[{0}] = @{0}", Person.ID_FIELD);
            }
            return dbQuery;
        }
    
        private DbQuery BuildClassGradeStatisticQuery(DbQuery dbQuery, GradingStatisticQuery query)
        {
            dbQuery = BuildGradeStatisicDbQuery(dbQuery, query);
            var personExistsingQueryTmp = string.Format(" and exists(select * from [ClassPerson] where [{1}] = [Class].[{2}] [{0}] =@"
                              , ClassPerson.PERSON_REF_FIELD, ClassPerson.CLASS_REF_FIELD, Class.ID_FIELD);
            personExistsingQueryTmp += "{3})";
            if (query.StudentId.HasValue && !dbQuery.Parameters.ContainsKey("@studentId"))
            {
                dbQuery.Parameters.Add("@studentId", query.StudentId);
                dbQuery.Sql.AppendFormat(personExistsingQueryTmp, "@studentId");
            }
            if (query.Role == CoreRoles.STUDENT_ROLE.Id && !dbQuery.Parameters.ContainsKey(Person.ID_FIELD))
            {
                dbQuery.Parameters.Add(Person.ID_FIELD, query.CallerId);
                dbQuery.Sql.AppendFormat(personExistsingQueryTmp, Person.ID_FIELD);
            }
            return dbQuery;
        }

        private DbQuery BuildStudentGradeAvgPerMPCDbQuey(GradingStatisticQuery query)
        {
            var dbQuery = new DbQuery();
            var types = new List<Type> { typeof(Person), typeof(MarkingPeriodClass) };
            dbQuery.Sql.AppendFormat(@"select {0}, dbo.fnCalcStudentGradeAvgForFinalGrade(Person.Id, MarkingPeriodClass.Id, MarkingPeriod.EndDate) as [Avg] 
                             from Person 
                             join ClassPerson on ClassPerson.PersonRef = Person.Id
                             join Class on Class.Id = ClassPerson.ClassRef
                             join MarkingPeriodClass on MarkingPeriodClass.ClassRef = Class.Id
                             join MarkingPeriod on MarkingPeriod.Id = MarkingPeriodClass.MarkingPeriodRef "
                             , Orm.ComplexResultSetQuery(types));
            dbQuery.Sql.Append(" where 1=1 ");
            return BuilStudentGradeStatisticQuery(dbQuery, query);
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
            var fullFieldNameFormat = "{0}_{1}";
            var fields = new List<string>
                {
                    string.Format(fullFieldNameFormat, personType.Name, Person.ID_FIELD),
                    string.Format(fullFieldNameFormat, personType.Name, Person.FIRST_NAME_FIELD),
                    string.Format(fullFieldNameFormat, personType.Name, Person.LAST_NAME_FIELD),
                    string.Format(fullFieldNameFormat, personType.Name, Person.GENDER_FIELD),
                    string.Format(fullFieldNameFormat, mpcType.Name, MarkingPeriodClass.CLASS_REF_FIELD),
                };
            var resultSetStr = fields.Select(x => string.Format("x.{0} as {0}", x)).JoinString(",");
            fields.Add(string.Format(fullFieldNameFormat, mpcType.Name, MarkingPeriodClass.ID_FIELD));
            var groupBySetStr = fields.Select(x => string.Format("x.{0}", x)).JoinString(",");
            var sql = @"select {1}, AVG(x.[Avg]) as [Avg]                                
                        from ({0})x  group by {2}";
            sql = string.Format(sql, innerQuery.Sql, resultSetStr, groupBySetStr);

            return ReadMany<StudentGradeAvgPerClass>(new DbQuery (sql, innerQuery.Parameters), true);
        }

        public IList<MarkingPeriodClassGradeAvg> CalcClassGradingPerMp(GradingStatisticQuery query)
        {
            var types = new List<Type> {typeof (Class), typeof(MarkingPeriod), typeof(MarkingPeriodClass)};
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select {0}, dbo.fnCalcClassGradeAvgPerMP(MarkingPeriodClass.Id) as [Avg] 
                               from Class 
                               join MarkingPeriodClass on MarkingPeriodClass.ClassRef = Class.Id
                               join MarkingPeriod on MarkingPeriod.Id = MarkingPeriodClass.MarkingPeriodRef"
                , Orm.ComplexResultSetQuery(types));
            dbQuery.Sql.Append(" where 1=1 ");
            dbQuery = BuildClassGradeStatisticQuery(dbQuery, query);
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
            dbQuery = BuildClassGradeStatisticQuery(dbQuery, query);
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
                    {"roleId", role},
                };
            using (var reader = ExecuteStoredProcedureReader("spCalcGradingStats", parameters))
            {
                var res = reader.ReadList<ClassPersonGradingStats>();
                reader.NextResult();

                while (reader.Read())
                {
                    var attTypeGrading = reader.Read<AnnouncementTypeGrading>();
                    var cp = res.First(x => x.Id == attTypeGrading.ClassPersonId);
                    if(cp.GradingsByAnnType == null)
                        cp.GradingsByAnnType = new List<AnnouncementTypeGrading>();
                    cp.GradingsByAnnType.Add(attTypeGrading);
                }
                return res;
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

        public DateTime Date { get; set; }

        public GradingStatisticQuery()
        {
            Date = DateTime.UtcNow;
        }
    }
}
