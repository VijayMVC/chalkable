﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassAttendanceDataAccess : DataAccessBase<ClassAttendance>
    {
        public ClassAttendanceDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        private const string SET_CLASS_ATTENDANCE_PROC = "spSetClassAttendance";
        private const string CLASS_PERIOD_ID_PARAM = "classPeriodId";
        private const string DATA_PARAM = "date";
        private const string TYPE_PARAM = "type";
        private const string ATTENDANCE_REASON_ID_PARAM = "attendanceReasonId";
        private const string LAST_MODIFIED_PARAM = "lastModified";
        private const string DESCRIPION_PARAM = "description";
        private const string SIS_ID_PARAM = "sisId";
        private const string CLASS_PERIODS_IDS_PARAM = "classPersonsIds";

        private const string GET_CLASS_ATTENDANCE_PROC = "spGetClassAttendance";
        private const string STUDENT_ID_PARAM = "studentId";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";
        private const string FROM_DATE_PARAM = "fromDate";
        private const string TO_DATE_PARAM = "toDate";
        private const string CLASS_ID_PARAM = "classId";
        private const string TEACHER_ID_PARAM = "teacherId";
        private const string FROM_TIME = "fromTime";
        private const string TO_TIME = "toTime";
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string ID_PARAM = "id";
        private const string NEED_ALL_DATA_PARAM = "needAllData";



        public IList<ClassAttendance> SetAttendance(ClassAttendance attendance, IList<Guid> classPersonsIds)
        {
            var parameters = new Dictionary<string, object>
                {
                    {CLASS_PERIOD_ID_PARAM, attendance.ClassPeriodRef},
                    {DATA_PARAM, attendance.Date},
                    {TYPE_PARAM, (int) attendance.Type},
                    {ATTENDANCE_REASON_ID_PARAM, attendance.AttendanceReasonRef},
                    {LAST_MODIFIED_PARAM, attendance.LastModified},
                    {DESCRIPION_PARAM, attendance.Description},
                    {SIS_ID_PARAM, attendance.SisId},
                    {CLASS_PERIODS_IDS_PARAM, classPersonsIds.Select(x=> x.ToString()).JoinString(",")}
                };
            using (var reader = ExecuteStoredProcedureReader(SET_CLASS_ATTENDANCE_PROC, parameters))
            {
               return reader.ReadList<ClassAttendance>();
            }
        }
        public IList<ClassAttendanceDetails> GetAttendance(ClassAttendanceQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {STUDENT_ID_PARAM, query.StudentId},
                    {MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId},
                    {FROM_DATE_PARAM, query.FromDate},
                    {TO_DATE_PARAM, query.ToDate},
                    {CLASS_ID_PARAM, query.ClassId},
                    {TEACHER_ID_PARAM, query.TeacherId},
                    {FROM_TIME, query.FromTime},
                    {TO_TIME, query.ToTime},
                    {SCHOOL_YEAR_ID_PARAM, query.SchoolYearId},
                    {ID_PARAM, query.Id},
                    {NEED_ALL_DATA_PARAM, query.NeedAllData},
                    {TYPE_PARAM, query.Type},
                    {CLASS_PERIOD_ID_PARAM, query.ClassPeriodId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_CLASS_ATTENDANCE_PROC, parameters))
            {
                var res = new List<ClassAttendanceDetails>();
                while (reader.Read())
                {
                    var attendance = reader.Read<ClassAttendanceDetails>(true);
                    attendance.Student = PersonDataAccess.ReadPersonData(reader);
                    attendance.Class.Id = attendance.ClassPerson.ClassRef;
                    attendance.ClassPeriodRef = attendance.ClassPeriod.Id;
                    attendance.ClassPeriod.PeriodRef = attendance.ClassPeriod.Period.Id;
                    attendance.ClassPersonRef = attendance.ClassPerson.Id;
                    attendance.ClassPeriod.ClassRef = attendance.Class.Id;
                    res.Add(attendance);
                }
                return res;
            }
        } 
        
        public IList<AttendanceTotalPerType> CalcAttendanceTypeTotal(Guid? markingPeriodId, Guid? schoolYearId,
               Guid? studentId, DateTime? fromDate, DateTime? toDate)
        {
            var parameters = new Dictionary<string, object>
                {
                    {MARKING_PERIOD_ID_PARAM, markingPeriodId},
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId},
                    {FROM_DATE_PARAM, fromDate},
                    {TO_DATE_PARAM, toDate},
                    {STUDENT_ID_PARAM, studentId}
                };
            using (var reader = ExecuteStoredProcedureReader("spCalcAttendanceTypeTotal", parameters))
            {
               return reader.ReadList<AttendanceTotalPerType>();
            }
        } 

        public bool Exists(Guid markingPeriodId, DateTime fromDate)
        {
            var sql = @"select ca.Id from ClassAttendance ca
                        join ClassPeriod cp on cp.Id = ca.ClassPeriodRef
                        join Period p on p.Id = cp.PeriodRef
                        where p.MarkingPeriodRef = @markingPeriodId and ca.Date >= @fromDate";
            var conds = new Dictionary<string, object>
                {
                    {"markingPeriodId", markingPeriodId},
                    {"fromDate", fromDate},
                };
            return Exists(new DbQuery(sql, conds));
        }

        public int PossibleAttendanceCount(Guid markingPeriodId, Guid classId, DateTime? tillDate)
        {
            var b = new StringBuilder();
            b.Append(@" select ClassPeriod.* from ClassPeriod 
                        join Period on Period.Id = ClassPeriod.PeriodRef
                        join [Date] on [Date].MarkingPeriodRef = Period.MarkingPeriodRef and [Date].ScheduleSectionRef = Period.SectionRef
                        join ClassPerson on ClassPerson.ClassRef = ClassPeriod.ClassRef
                        where ClassPeriod.ClassRef = @classId and Period.MarkingPeriodRef = @markingPeriodId and [Date].IsSchoolDay = 1 ");

            var conds = new Dictionary<string, object>
                {
                    {"classId", classId},
                    {"markingPeriodId", markingPeriodId}
                };
            if (tillDate.HasValue)
            {
                conds.Add("tillDate", tillDate.Value);
                b.Append(" and  [Date].[DateTime] <= @tillDate");
            }
            return Count(new DbQuery(b, conds));
        }

        public IDictionary<Guid, DateTime> GetStudentAbsentFromDay(DateTime fromDate, DateTime toDate, IList<Guid> gradeLevelIds)
        {
            return  Read(PrepareGetStudentAbsentFromDayQuery(fromDate, toDate, gradeLevelIds), ReadStudentAbsentFromDay);
        } 
        public IDictionary<DateTime, int> GetStudentCountAbsentFromDay(DateTime fromDate, DateTime toDate, IList<Guid> gradeLevelIds)
        {
            var dbQuery = PrepareGetStudentAbsentFromDayQuery(fromDate, toDate, gradeLevelIds);
            dbQuery = PrepareGetStudentCountPerDayQuery(dbQuery); 
            return Read(dbQuery, ReadStudentCountAbsentFromDay);
        }

        private const string GRADE_LEVELS_IDS_PARAM = "gradeLevelsIds";
        private const string FN_GET_STUDENT_ABSENT_FROM_DAY = "dbo.fnGetStudentsAbsentFromDay({0}, {1}, {2})";
        private DbQuery PrepareGetStudentAbsentFromDayQuery(DateTime fromDate, DateTime toDate, IEnumerable<Guid> gradeLevelIds)
        {
            var dbQuery = new DbQuery();
            var glStr = gradeLevelIds != null ? gradeLevelIds.Select(x => x.ToString()).JoinString(",") : null;
            dbQuery.Sql.Append("select * from ").AppendFormat(FN_GET_STUDENT_ABSENT_FROM_DAY, 
                "@" + FROM_DATE_PARAM, "@" + TO_DATE_PARAM, "@" + GRADE_LEVELS_IDS_PARAM);
            dbQuery.Parameters.Add(FROM_DATE_PARAM, fromDate);
            dbQuery.Parameters.Add(TO_DATE_PARAM, toDate);
            dbQuery.Parameters.Add(GRADE_LEVELS_IDS_PARAM, glStr);
            return dbQuery;
        }

        private const string STUDENT_COUNT_RES_FIELD = "StudentCount";
        private const string DATE_RES_FIELD = "Date";
        private const string PERSON_ID_RES_FIELD = "PersonId";
        private IDictionary<DateTime, int> ReadStudentCountAbsentFromDay(DbDataReader reader)
        {
            var res = new Dictionary<DateTime, int>();
            while (reader.Read())
            {
                var studentCount = SqlTools.ReadInt32(reader, STUDENT_COUNT_RES_FIELD);
                var date = SqlTools.ReadDateTime(reader, DATE_RES_FIELD);
                res.Add(date, studentCount);
            }
            return res;
        }
        private IDictionary<Guid, DateTime> ReadStudentAbsentFromDay(DbDataReader reader)
        {
                var res = new Dictionary<Guid, DateTime>();
                while (reader.Read())
                {
                    var personId = SqlTools.ReadGuid(reader, PERSON_ID_RES_FIELD);
                    var date = SqlTools.ReadDateTime(reader, DATE_RES_FIELD);
                    res.Add(personId,date);
                }
                return res;    
        }

        private const string FN_GET_STUDENT_ABSENT_FROM_PERIOD = "fnGetStudentAbentFromPeriod({0}, {1}, {2})";
        
        private DbQuery PrepareGetStudentCountPerDayQuery(DbQuery innerQuery)
        {
            var res = new DbQuery();
            var innerSql = innerQuery.Sql.ToString();
            res.Parameters = innerQuery.Parameters;
            res.Sql.AppendFormat(@"select d.[{5}] as [{1}],
	                                   case when x.[{1}] is null then 0 else Count(*) end as {2}
                                       from [Date] d
                                       left join ({0})x on d.[{5}] = x.[{1}]
                                       where d.[{6}] = 1 and (d.[{5}] between @{3} and @{4})
                                       group by d.[{5}], x.[{1}]", innerSql, DATE_RES_FIELD,
                                       STUDENT_COUNT_RES_FIELD, FROM_DATE_PARAM, TO_DATE_PARAM
                                      , Date.DATE_TIME_FIELD, Date.IS_SCHOOL_DAY_FIELD);
            return res;
        }
        private DbQuery PrepareGetStudentAbsentFromPeriodQuery(int periodOrder, string periodOrderParamName, DateTime fromDate, DateTime toDate, IList<Guid> gradeLevels)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select x.{0} as {0}, x.{1} as {1} 
                                       from StudentInfo si", PERSON_ID_RES_FIELD, DATE_RES_FIELD);
            dbQuery.Sql.Append(" join ").AppendFormat(FN_GET_STUDENT_ABSENT_FROM_PERIOD, "@" + periodOrderParamName, "@" + FROM_DATE_PARAM, "@" + TO_DATE_PARAM);
            dbQuery.Sql.AppendFormat(" x on x.{0} = si.{1} ", PERSON_ID_RES_FIELD, StudentInfo.ID_FIELD);
            dbQuery.Parameters.Add(periodOrderParamName, periodOrder);
            dbQuery.Parameters.Add(FROM_DATE_PARAM, fromDate);
            dbQuery.Parameters.Add(TO_DATE_PARAM, toDate);
            if (gradeLevels != null && gradeLevels.Count > 0)
            {
                var glStr = gradeLevels.Select(x => "'" + x.ToString() + "'").JoinString(",");
                dbQuery.Sql.AppendFormat(" where si.{0} in ({1})", StudentInfo.GRADE_LEVEL_REF_FIELD, glStr);
            }
            return dbQuery;
        }
       

        public IList<StudentCountAbsentFromPeriod> GetStudentCountAbsentFromPeriod(DateTime fromDate, DateTime toDate, int fromPeriodOrder, 
            int toPeriodOrder, IList<Guid> gradeLevelsIds)
        {
            var dbQuery = new DbQuery();
            for (int i = fromPeriodOrder; i <= toPeriodOrder; i++)
            {
                var periodOrderFieldName = i + "_periodOrder";
                var innerQuery = PrepareGetStudentAbsentFromPeriodQuery(i, periodOrderFieldName, fromDate, toDate, gradeLevelsIds);
                var query = PrepareGetStudentCountPerDayQuery(innerQuery);
                var sql = string.Format(@"select x.[{1}] as [{1}], x.[{2}] as [{2}], @{3} as [PeriodOrder]
                                         from ({0}) x ", query.Sql, DATE_RES_FIELD, STUDENT_COUNT_RES_FIELD, periodOrderFieldName);
                dbQuery.Sql.Append(sql);
                foreach (var parameter in query.Parameters)
                {
                    if(!dbQuery.Parameters.ContainsKey(parameter.Key))
                        dbQuery.Parameters.Add(parameter);
                }
                if (i < toPeriodOrder) dbQuery.Sql.Append(" union ");
            }
            return ReadMany<StudentCountAbsentFromPeriod>(dbQuery);
        }



        public IList<StudentAbsentFromPeriod> GetStudentAbsentFromPeriod(DateTime date, IList<Guid> gradeLevelsIds, int periodOrder)
        {
            var dbQuery = PrepareGetStudentAbsentFromPeriodQuery(periodOrder, "periodOrder", date, date, gradeLevelsIds);
            var res = ReadMany<StudentAbsentFromPeriod>(dbQuery);
            foreach (var studentAbsentFromPeriod in res)
            {
                studentAbsentFromPeriod.PeriodOrder = periodOrder;
            }
            return res;
        }  
    }

    public class AttendanceTotalPerType
    {
        public Guid PersonId { get; set; }
        public int Total { get; set; }
        public AttendanceTypeEnum AttendanceType { get; set; }
    }
    public class StudentAbsentFromPeriod
    {
        public DateTime Date { get; set; }
        public Guid PersonId { get; set; }
        public int PeriodOrder { get; set; }
    }
    public class StudentCountAbsentFromPeriod
    {
        public DateTime Date { get; set; }
        public int StudentCount { get; set; }
        public int PeriodOrder { get; set; }
    }

    public class ClassAttendanceQuery
    {
        public Guid? Id { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? ClassPeriodId { get; set; }
        public Guid? SchoolYearId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public AttendanceTypeEnum? Type { get; set; }
        public int? FromTime { get; set; }
        public int? ToTime { get; set; }
        public bool NeedAllData { get; set; }

        public ClassAttendanceQuery()
        {
            NeedAllData = false;
        }
    }
}
