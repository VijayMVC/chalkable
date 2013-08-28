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

        private string FN_GET_STUDENT_ABSENT_FROM_DAY = "fnGetStudentsAbsentFromDay({0}, {1})";

        public IDictionary<Guid, DateTime> GetStudentAbsentFromDay(DateTime fromDate, DateTime toDate)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append("select * from ").AppendFormat(FN_GET_STUDENT_ABSENT_FROM_DAY, "@fromDate", "@toDate");
            dbQuery.Parameters.Add("@fromDate", fromDate);
            dbQuery.Parameters.Add("@toDate", toDate);
            return  Read(dbQuery, ReadStudentAbsentFromDay);
        } 

        private IDictionary<Guid, DateTime> ReadStudentAbsentFromDay(DbDataReader reader)
        {
                var res = new Dictionary<Guid, DateTime>();
                while (reader.Read())
                {
                    var personId = SqlTools.ReadGuid(reader, "PersonId");
                    var date = SqlTools.ReadDateTime(reader, "Date");
                    res.Add(personId,date);
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
