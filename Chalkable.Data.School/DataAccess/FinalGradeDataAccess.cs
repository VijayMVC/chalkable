using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class FinalGradeDataAccess : DataAccessBase<FinalGrade>
    {
        public FinalGradeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public void FullDelete(Guid id)
        {
            var conds = new Dictionary<string, object> {{"finalGradeId", id}};
            var sql = @"delete from FinalGradeAnnouncementType
                        where FinalGradeRef = @finalGradeId
                        delete from StudentFinalGrade
                        where FinalGradeRef = @finalGradeId
                        delete from FinalGrade
                        where Id = @finalGradeId";
            ExecuteNonQueryParametrized(sql, conds);
        }

        public VwFinalGrade GetVwFinalGrade(Guid id)
        {
            var conds = new AndQueryCondition { { "FinalGrade_Id", id } };
            var command = Orm.SimpleSelect<VwFinalGrade>(conds);
            return ReadOne<VwFinalGrade>(command, true);
        }
        public VwFinalGrade GetFirstFinalGrade(Guid markingPeriodId, Guid teacherId)
        {
            var conds = new AndQueryCondition
                {
                    {"MarkingPeriodClass_MarkingPeriodRef", markingPeriodId},
                    {"Class_TeacherRef", teacherId}
                };
            var command = Orm.SimpleSelect<VwFinalGrade>(conds);
            return ReadOneOrNull<VwFinalGrade>(command, true);
        }

        public bool Exists(Guid id)
        {
            return Exists<FinalGrade>(new AndQueryCondition { { FinalGrade.ID_FIELD, id } });
        }
        


        public FinalGradeQueryResult GetFinalGradesDetails(FinalGradeQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"finalGradeId", query.Id},
                    {"markingPeriodId", query.MarkingPeriodId},
                    {"classId", query.ClassId},
                    {"callerId", query.CallerId},
                    {"callerRoleId", query.CallerRoleId},
                    {"status", query.Status},
                    {"start", query.Start},
                    {"count", query.Count}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetFinalGrades", parameters))
            {
                int sourceCount;
                var finalGrades = ReadFinalGradesResult(reader, out sourceCount);
                return new FinalGradeQueryResult { Query = query, FinalGrades = finalGrades, SourceCount = sourceCount };
            }
        }     
        public FinalGradeDetails BuildFinalGrade(Guid markingPeriodClassId, Guid callerId, int callerRoleId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"markingPeriodClassId", markingPeriodClassId},
                    {"callerId", callerId},
                    {"callerRoleId", callerRoleId}
                };
            using (var reader = ExecuteStoredProcedureReader("spBuildFinalGrade", parameters))
            {
                int sourceCount;
                var res = ReadFinalGradesResult(reader, out sourceCount);
                return res.FirstOrDefault();
            }
        }
        private IList<FinalGradeDetails> ReadFinalGradesResult(SqlDataReader reader, out int sourceCount)
        {
            sourceCount = reader.Read() ? SqlTools.ReadInt32(reader, "SourceCount") : 0;
            reader.NextResult();
            var finalGrades = reader.ReadList<FinalGradeDetails>(true);
            if (finalGrades.Count > 0)
            {
                reader.NextResult();
                var fgAnnTypes = reader.ReadList<FinalGradeAnnouncementType>(true);
                reader.NextResult();
                var stFinalGrades = StudentFinalGradeDataAccess.ReadStudentFinalGradeResult(reader);
                foreach (var finalGrade in finalGrades)
                {
                    finalGrade.StudentFinalGrades = stFinalGrades.Where(x => x.FinalGradeRef == finalGrade.Id).ToList();
                    finalGrade.FinalGradeAnnouncementTypes = fgAnnTypes.Where(x => x.FinalGradeRef == finalGrade.Id).ToList();
                }
            }
            return finalGrades;
        }  
    }

    public class FinalGradeQuery
    {
        public Guid? Id { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid CallerId { get; set; }
        public int CallerRoleId { get; set; }
        public FinalGradeStatus? Status { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }

        public FinalGradeQuery()
        {
            Start = 0;
            Count = int.MaxValue;
        }
    }

    public class FinalGradeQueryResult
    {
        public FinalGradeQuery Query { get; set; }
        public IList<FinalGradeDetails> FinalGrades { get; set; }
        public int SourceCount { get; set; }
    }
}
