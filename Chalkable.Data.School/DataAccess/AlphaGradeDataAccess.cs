using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AlphaGradeDataAccess : DataAccessBase<AlphaGrade, int>
    {
        public AlphaGradeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public IList<AlphaGrade> GetAlphaGradesBySchoolId(int schoolId)
        {
            var ps = new Dictionary<string, object> {{"schoolId", schoolId}};
            return ExecuteStoredProcedureList<AlphaGrade>("spGetAlphaGradesBySchool", ps);
        }

        public IDictionary<int, IList<AlphaGrade>> GetAlphaGradeForClasses(IList<int> classIds)
        {
            var ps = new Dictionary<string, object> {{"classIds", classIds}};
            using (var reader = ExecuteStoredProcedureReader("spGetAlphaGradesForClasses", ps))
            {
                return ReadAlphaGradesForClasses(reader, classIds);
            }
        }

        public IDictionary<int, IList<AlphaGrade>> GetAlphaGradesForClassStandards(IList<int> classIds)
        {
            var ps = new Dictionary<string, object> {{"classIds", classIds}};
            using (var reader = ExecuteStoredProcedureReader("spGetAlphaGradesForClassStandards", ps))
            {
                return ReadAlphaGradesForClasses(reader, classIds);
            }
        }

        private class ClassAlphaGrade
        {
            [DataEntityAttr]
            public AlphaGrade AlphaGrade { get; set; }
            public int ClassId { get; set; }
        }

        public static IDictionary<int, IList<AlphaGrade>> ReadAlphaGradesForClasses(DbDataReader reader, IEnumerable<int> classIds)
        {
            var classAlphaGrades = reader.ReadList<ClassAlphaGrade>();
            var res = new Dictionary<int, IList<AlphaGrade>>();
            foreach (var classId in classIds.Distinct())
            {
                var alphaGrades = classAlphaGrades.Where(x=>x.ClassId == classId).Select(x => x.AlphaGrade).ToList();
                res.Add(classId, alphaGrades);
            }
            return res;
        }
    }
}
