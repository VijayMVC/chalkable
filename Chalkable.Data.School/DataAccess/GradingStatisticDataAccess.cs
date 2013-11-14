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

        private DbQuery BuildStudentGradeAvgPerMPCDbQuey(GradingStatisticQuery query, string avgFieldName = "[Avg]")
        {
            var dbQuery = new DbQuery();
            var types = new List<Type> { typeof(Person), typeof(MarkingPeriodClass) };
            dbQuery.Sql.AppendFormat(@"select {0}, 
                                       dbo.fnCalcStudentGradeAvgForFinalGrade(Person.Id, MarkingPeriodClass.Id, MarkingPeriod.EndDate) as {1} 
                             from Person 
                             join ClassPerson on ClassPerson.PersonRef = Person.Id
                             join Class on Class.Id = ClassPerson.ClassRef
                             join MarkingPeriodClass on MarkingPeriodClass.ClassRef = Class.Id
                             join MarkingPeriod on MarkingPeriod.Id = MarkingPeriodClass.MarkingPeriodRef "
                             , Orm.ComplexResultSetQuery(types), avgFieldName);
            dbQuery.Sql.Append(" where 1=1 ");
            return BuilStudentGradeStatisticQuery(dbQuery, query);
        }

        //TODO: add method to Orm for function building
        public IList<StudentGradeAvgPerMPC> CalcStudentGradeAvgPerMPC(GradingStatisticQuery query)
        {
            var dbQuery = BuildStudentGradeAvgPerMPCDbQuey(query, "StudentGradeAvg_Avg");
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
                    string.Format(fullFieldNameFormat, personType.Name, Person.ROLE_REF_FIELD),
                    string.Format(fullFieldNameFormat, mpcType.Name, MarkingPeriodClass.CLASS_REF_FIELD),
                };
            var resultSetStr = fields.Select(x => string.Format("x.{0} as {0}", x)).JoinString(",");
            
            //fields.Add(string.Format(fullFieldNameFormat, mpcType.Name, MarkingPeriodClass.ID_FIELD)); TODO: fix it
            
            
            var groupBySetStr = fields.Select(x => string.Format("x.{0}", x)).JoinString(",");
            var sql = @"select {1}, AVG(x.[Avg]) as StudentGradeAvg_Avg                                
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

        public IList<StudentClassGradeStats> CalcStudentClassGradeStats(int classId, int markingPeriodId, int? studentId, int dayInterval)
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
                var stDic = new Dictionary<int, StudentClassGradeStats>();
                while (reader.Read())
                {
                    var stId = SqlTools.ReadInt32(reader, StudentClassGradeStats.STUDENT_ID_FEILD);
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
                var anntypeDic = new Dictionary<int, IDictionary<int, AnnTypeGradeStats>>();
                while (reader.Read())
                {
                    var stId = SqlTools.ReadInt32(reader, StudentClassGradeStats.STUDENT_ID_FEILD);
                    var annType = SqlTools.ReadInt32(reader, AnnTypeGradeStats.ANNOUNCEMENT_TYPE_ID_FIELD);
                    if(!anntypeDic.ContainsKey(stId))
                        anntypeDic.Add(stId, new Dictionary<int, AnnTypeGradeStats>());
                    if(!anntypeDic[stId].ContainsKey(annType))
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

        public IList<ClassPersonGradingStats> CalcGradingStats(int callerId, int role, int studentId, int markingPeriodId)
        {
            throw new NotImplementedException();
            //var parameters = new Dictionary<string, object>
            //    {
            //        {"studentId", studentId},
            //        {"markingPeriodId", markingPeriodId},
            //        {"callerId", callerId},
            //        {"roleId", role},
            //    };
            //using (var reader = ExecuteStoredProcedureReader("spCalcGradingStats", parameters))
            //{
            //    var res = reader.ReadList<ClassPersonGradingStats>();
            //    reader.NextResult();

            //    while (reader.Read())
            //    {
            //        var attTypeGrading = reader.Read<AnnouncementTypeGrading>();
            //        var cp = res.First(x => x.Id == attTypeGrading.ClassPersonId);
            //        if(cp.GradingsByAnnType == null)
            //            cp.GradingsByAnnType = new List<AnnouncementTypeGrading>();
            //        cp.GradingsByAnnType.Add(attTypeGrading);
            //    }
            //    return res;
            //}
        }
    
        
        public IList<StudentGradingRank> GetStudentGradingRank(Guid callerId, int roleId, Guid schoolYearId, Guid? gradeLevelId,
                                                               Guid? studentId, Guid? classId)
        {
            throw new NotImplementedException();
//            var dbQuery = new DbQuery();
//            dbQuery.Sql.AppendFormat(@"
//                                select x.* from 
//                                    (select	
//	                                    Person.Id as StudentId,
//	                                    mp.Id as MarkingPeriodId,
//	                                    mp.Name as MarkingPeriodName,
//	                                    si.{1} as GradeLevelId,
//	                                    Avg(sa.GradeValue) as [Avg],
//	                                    Rank() over (partition by mp.Id, si.{1} order by avg(sa.gradeValue) desc) as [Rank]
//                                    from Person   
//                                    join ClassPerson on ClassPerson.PersonRef = Person.Id
//                                    join StudentInfo si on si.Id = Person.Id
//                                    join StudentAnnouncement sa on sa.ClassPersonRef = ClassPerson.Id
//                                    join Announcement a on a.Id = sa.AnnouncementRef
//                                    join MarkingPeriodClass mpc on mpc.Id = a.MarkingPeriodClassRef
//                                    join MarkingPeriod mp on mp.Id = mpc.MarkingPeriodRef 
//                                    where {0} = mp.SchoolYearRef and (@{1} is null or si.{1} = @{1}) 
//		                                    and sa.GradeValue is not null and sa.[State] = 2
//                                    group by si.{1}, mp.Id, mp.Name, Person.Id) x
//                                ", MarkingPeriod.SCHOOL_YEAR_REF, StudentInfo.GRADE_LEVEL_REF_FIELD);
//            dbQuery.Parameters.Add(MarkingPeriod.SCHOOL_YEAR_REF, schoolYearId);
//            dbQuery.Parameters.Add(StudentInfo.GRADE_LEVEL_REF_FIELD, gradeLevelId);
//            dbQuery.Sql.Append(" where ")
//                       .Append(@"(@roleId = 1 or @roleId = 2 or @roleId = 5 or @roleId = 7 or @roleId = 8 
//	                              or (@roleId = 3 and x.[StudentId] = @callerId))");
//            dbQuery.Parameters.Add("@roleId", roleId);
//            dbQuery.Parameters.Add("@callerId", callerId);
//            if (studentId.HasValue)
//            {
//                dbQuery.Sql.Append(" and x.[StudentId] = @studentId ");
//                dbQuery.Parameters.Add("@studentId", studentId);
//            }
//            if (classId.HasValue)
//            {
//                dbQuery.Sql.Append(" and (x.[StudentId] in (select csp.SchoolPersonRef from ClassSchoolPerson csp where csp.ClassRef = @classId))");
//                dbQuery.Parameters.Add("@classId", classId);
//            }
//            if (gradeLevelId.HasValue)
//            {
//                dbQuery.Sql.AppendFormat(" and x.[GradeLevelId] = @{0}", StudentInfo.GRADE_LEVEL_REF_FIELD);
//            }
//            return ReadMany<StudentGradingRank>(dbQuery);
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
