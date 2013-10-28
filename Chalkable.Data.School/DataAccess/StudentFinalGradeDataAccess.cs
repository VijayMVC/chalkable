using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentFinalGradeDataAccess : DataAccessBase<StudentFinalGrade, int>
    {
        public StudentFinalGradeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<StudentFinalGradeDetails> ReCalculateGradeByAnnouncement(Guid finalGradeId)
        {
            var sql = @"declare @markingPeriodId uniqueidentifier = (select top 1 MarkingPeriodRef 
                                                                     from MarkingPeriodClass where Id = @finalGradeId)
                       
                        update StudentFinalGrade 
                        set GradeByAnnouncement = dbo.fnGetStudentGradeAvgForClass(ClassPerson.PersonRef, ClassPerson.ClassRef, @markingPeriodId)
                        from ClassPerson 
                        where StudentFinalGrade.FinalGradeRef = @finalGradeId 
	                           and ClassPerson.Id = StudentFinalGrade.ClassPersonRef
                        
                        select * from vwStudentFinalGrade where StudentFinalGrade_FinalGradeRef = @finalGradeId ";

            var conds = new Dictionary<string, object> { { "@finalGradeId", finalGradeId } };
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                return ReadStudentFinalGradeResult(reader);
            }
        }

        public static IList<StudentFinalGradeDetails> ReadStudentFinalGradeResult(DbDataReader reader)
        {
            var res = new List<StudentFinalGradeDetails>();
            while (reader.Read())
            {
                var stFinalGrade = reader.Read<StudentFinalGradeDetails>(true);
                stFinalGrade.Student = PersonDataAccess.ReadPersonData(reader);
                res.Add(stFinalGrade);
            }
            return res;
        } 
    }
}
