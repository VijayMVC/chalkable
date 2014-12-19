using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
//    public class StudentAnnouncementDataAccess : DataAccessBase<StudentAnnouncement, int>
//    {
//        public StudentAnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
//        {
//        }

//        public void Update(int announcementId, bool drop)
//        {
//            var conds = new AndQueryCondition { { StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME, announcementId } };
//            var updateParams = new Dictionary<string, object> { { StudentAnnouncement.DROPPED_FIELD, drop } };
//            SimpleUpdate<StudentAnnouncement>(updateParams, conds);
//        }

//        public IList<StudentAnnouncementDetails> GetStudentAnnouncementsDetails(int announcementId, int personId)
//        {
//            var parameters = new Dictionary<string, object>
//                {
//                    {"announcementId", announcementId},
//                    {"personId", personId},
//                };
//            using (var reader = ExecuteStoredProcedureReader("spGetStudentAnnouncementsForAnnouncement", parameters))
//            {
//                return ReadListStudentAnnouncement(reader);
//            }
//        }
        

//        public static IList<StudentAnnouncementDetails> ReadListStudentAnnouncement(DbDataReader reader)
//        {
//            var res = new List<StudentAnnouncementDetails>();
//            while (reader.Read())
//            {
//                res.Add(ReadStudentAnnouncement(reader));
//            }
//            return res;
//        }
//        public  static  StudentAnnouncementDetails ReadStudentAnnouncement(DbDataReader reader)
//        {
//            var res = reader.Read<StudentAnnouncementDetails>(true);
//            res.Person = PersonDataAccess.ReadPersonData(reader);
//            return res;
//        }

//        public IList<StudentAnnouncement> GetList(StudentAnnouncementShortQuery query)
//        {
//            return SelectMany<StudentAnnouncement>(BuildConditions(query));
//        } 
//        private QueryCondition BuildConditions(StudentAnnouncementShortQuery query)
//        {
//            var res = new AndQueryCondition();
//            if(query.AnnouncementId.HasValue)
//                res.Add(StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME, query.AnnouncementId);
//            if (query.State.HasValue)
//                res.Add(StudentAnnouncement.STATE_FIELD, query.State);
//            var q = query as StudentAnnouncementQuery;
//            if (q != null)
//            {
//                if(q.StudentId.HasValue)
//                    res.Add(StudentAnnouncement.PERSON_REF, q.StudentId);
//            }
//            return res;
//        }

//        //TODO: rewrite this methods ... it's probably not working now
//        public IList<StudentAnnouncementGrade> GetStudentAnnouncementGrades(StudentAnnouncementQuery query)
//        {
//            var conds = new AndQueryCondition {BuildConditions(query)};
//            var sql = @"select {0},vwAnnouncement.* from vwAnnouncement
//                        join StudentAnnouncement on StudentAnnouncement.AnnouncementRef = vwAnnouncement.Id";
//            var res = new DbQuery();
//            res.Sql.AppendFormat(sql, Orm.ComplexResultSetQuery(new List<Type>{typeof(StudentAnnouncement)}))
//                   .AppendFormat(" where  [StudentAnnouncement].[{0}] is not null ",StudentAnnouncement.GRADE_VALUE_FIELD);
           
//            conds.Add(StudentAnnouncement.GRADE_VALUE_FIELD, null, ConditionRelation.NotEqual);
            
//            conds.BuildSqlWhere(res, "StudentAnnouncement");
//            if (query.ClassId.HasValue)
//                new AndQueryCondition { { Announcement.CLASS_REF_FIELD, query.ClassId } }.BuildSqlWhere(res, "vwAnnouncement", false);

//            var orderBy = string.IsNullOrEmpty(query.OrderBy) ? Announcement.ID_FIELD : query.OrderBy;
//            res = Orm.PaginationSelect(res, orderBy, Orm.OrderType.Desc, 0, query.Count);
//            return ReadPaginatedResult(res, 0 , query.Count, ReadGetStudentAnnouncementGradesResult);
//        }


//        public IList<StudentAnnouncementDetails> GetStudentAnnouncementsDetails(StudentAnnouncementQuery query)
//        {
//            var conds = new AndQueryCondition { BuildConditions(query) };
//            var res = new DbQuery();
//            var comResSet = Orm.ComplexResultSetQuery(new List<Type> {typeof (StudentAnnouncement)});
//            res.Sql.AppendFormat(@"select Person.*, {0}, 
//                                          Announcement.[ClassRef] as StudentAnnouncement_ClassId,
//                                   from StudentAnnouncement 
//                                   join Person on Person.[{1}] = StudentAnnouncement.[{2}] 
//                                   join Announcement on Announcement.[{3}] = StudentAnnouncement.[{4}]"
//                                 , comResSet, Person.ID_FIELD, StudentAnnouncement.PERSON_REF
//                                 , Announcement.ID_FIELD, StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME);
//            conds.BuildSqlWhere(res, "StudentAnnouncement");
//            if (query.ClassId.HasValue)
//            {
//                new AndQueryCondition {{Announcement.CLASS_REF_FIELD, query.ClassId}}.BuildSqlWhere(res, "Announcement", false);
//            }
//            var orderBy = string.IsNullOrEmpty(query.OrderBy) ? Announcement.ID_FIELD : query.OrderBy;
//            res = Orm.PaginationSelect(res, orderBy, Orm.OrderType.Desc, 0, query.Count);
//            return ReadPaginatedResult(res, 0, query.Count, ReadListStudentAnnouncement);
//        } 

//        private IList<StudentAnnouncementGrade> ReadGetStudentAnnouncementGradesResult(DbDataReader reader)
//        {
//            var res = new List<StudentAnnouncementGrade>();
//            while (reader.Read())
//            {
//                var st = reader.Read<StudentAnnouncementGrade>(true);
//                st.Announcement = reader.Read<AnnouncementComplex>();
//                res.Add(st);
//            }
//            return res;
//        }
//    }




//    public class StudentAnnouncementShortQuery
//    {
//        public int? AnnouncementId { get; set; }
//        public int? MarkingPeriodClassId { get; set; }
//        public StudentAnnouncementStateEnum? State { get; set; }     
//    }

//    public class StudentAnnouncementQuery : StudentAnnouncementShortQuery
//    {
//        public int? StudentId { get; set; }
//        public int Count { get; set; }
//        public string OrderBy { get; set; }
//        public int? ClassId { get; set; }

//        public StudentAnnouncementQuery()
//        {
//            Count = int.MaxValue;
//        }
//    }
}
