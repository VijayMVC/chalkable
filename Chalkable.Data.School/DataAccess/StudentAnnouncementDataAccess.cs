using System;
using System.Collections.Generic;
using System.Data.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentAnnouncementDataAccess : DataAccessBase<StudentAnnouncement, int>
    {
        public StudentAnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Update(int announcementId, bool drop)
        {
            var conds = new AndQueryCondition { { StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME, announcementId } };
            var updateParams = new Dictionary<string, object> { { StudentAnnouncement.DROPPED_FIELD, drop } };
            SimpleUpdate<StudentAnnouncement>(updateParams, conds);
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncementsDetails(int announcementId, int personId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"announcementId", announcementId},
                    {"personId", personId},
                };
            using (var reader = ExecuteStoredProcedureReader("spGetStudentAnnouncementsForAnnouncement", parameters))
            {
                return ReadListStudentAnnouncement(reader);
            }
        }
        

        public static IList<StudentAnnouncementDetails> ReadListStudentAnnouncement(DbDataReader reader)
        {
            var res = new List<StudentAnnouncementDetails>();
            while (reader.Read())
            {
                res.Add(ReadStudentAnnouncement(reader));
            }
            return res;
        }
        public  static  StudentAnnouncementDetails ReadStudentAnnouncement(DbDataReader reader)
        {
            var res = reader.Read<StudentAnnouncementDetails>(true);
            res.Person = PersonDataAccess.ReadPersonData(reader);
            return res;
        }

        public IList<StudentAnnouncement> GetList(StudentAnnouncementShortQuery query)
        {
            return SelectMany<StudentAnnouncement>(BuildConditions(query));
        } 
        private QueryCondition BuildConditions(StudentAnnouncementShortQuery query)
        {
            var res = new AndQueryCondition();
            if(query.AnnouncementId.HasValue)
                res.Add(StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME, query.AnnouncementId);
            if (query.State.HasValue)
                res.Add(StudentAnnouncement.STATE_FIELD, query.State);
            return res;
        }

        public IList<StudentAnnouncementGrade> GetStudentAnnouncementGrades(StudentAnnouncementQuery query)
        {
            var conds = BuildConditions(query);
            var sql = @"select {0},vwAnnouncement.* from vwAnnouncement
                        join StudentAnnouncement on StudentAnnouncement.AnnouncementRef = vwAnnouncement.Id";
            var res = new DbQuery();
            res.Sql.AppendFormat(sql, Orm.ComplexResultSetQuery(new List<Type>{typeof(StudentAnnouncement)}))
                   .AppendFormat(" where  [StudentAnnouncement].[{0}] is not null ",StudentAnnouncement.GRADE_VALUE_FIELD);
            if (query.StudentId.HasValue || query.ClassId.HasValue)
            {
                res.Sql.Append(@" and StudentAnnouncement.ClassPersonRef 
                                        in (select [ClassPerson].Id from ClassPerson where 1=1");

                if (query.StudentId.HasValue)
                {
                    res.Parameters.Add("studentId", query.StudentId);
                    res.Sql.Append(" and [ClassPerson].PersonRef=@studentId");
                }
                if (query.ClassId.HasValue)
                {
                    res.Parameters.Add("classId", query.ClassId);
                    res.Sql.Append(" and [ClassPerson].ClassRef=@classId");
                }
                res.Sql.Append(")");
            }
            conds.BuildSqlWhere(res, "StudentAnnouncement", false);
            var orderBy = string.IsNullOrEmpty(query.OrderBy) ? Announcement.ID_FIELD : query.OrderBy;
            res = Orm.PaginationSelect(res, orderBy, Orm.OrderType.Desc, 0, query.Count);
            return ReadPaginatedResult(res, 0 , query.Count, ReadGetStudentAnnouncementGradesResult);
        }

        private IList<StudentAnnouncementGrade> ReadGetStudentAnnouncementGradesResult(DbDataReader reader)
        {
            var res = new List<StudentAnnouncementGrade>();
            while (reader.Read())
            {
                var st = reader.Read<StudentAnnouncementGrade>(true);
                st.Announcement = reader.Read<AnnouncementComplex>();
                res.Add(st);
            }
            return res;
        }
    }




    public class StudentAnnouncementShortQuery
    {
        public int? AnnouncementId { get; set; }
        public int? MarkingPeriodClassId { get; set; }
        public StudentAnnouncementStateEnum? State { get; set; }     
    }

    public class StudentAnnouncementQuery : StudentAnnouncementShortQuery
    {
        public int? StudentId { get; set; }
        public int Count { get; set; }
        public string OrderBy { get; set; }
        public int? ClassId { get; set; }

        public StudentAnnouncementQuery()
        {
            Count = int.MaxValue;
        }
    }
}
