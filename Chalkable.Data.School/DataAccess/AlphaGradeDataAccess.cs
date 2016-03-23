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

        private const string SP_GET_ALPHA_GRADES_FOR_SCHOOL_STANDARDS = "spGetAlphaGradesForSchoolStandards";
        public IDictionary<int, IList<AlphaGrade>> GetAlphaGradesForSchoolStandarts(IList<int> schoolIds)
        {
            var ps = new Dictionary<string, object> { { "schoolIds", schoolIds } };
            using (var reader = ExecuteStoredProcedureReader(SP_GET_ALPHA_GRADES_FOR_SCHOOL_STANDARDS, ps))
            {
                return ReadAlphaGradesForSchool(reader, schoolIds);
            }
        }

        private class ClassAlphaGrade
        {
            [DataEntityAttr]
            public AlphaGrade AlphaGrade { get; set; }
            public int ClassId { get; set; }
        }

        public class SchoolAlphaGrade
        {
            [DataEntityAttr]
            public AlphaGrade AlphaGrade { get; set; }
            public int SchoolId { get; set; }
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

        public static IDictionary<int, IList<AlphaGrade>> ReadAlphaGradesForSchool(DbDataReader reader, IEnumerable<int> schoolIds)
        {
            var schoolAlphaGrades = reader.ReadList<SchoolAlphaGrade>();
            var res = new Dictionary<int, IList<AlphaGrade>>();
            foreach (var schoolId in schoolIds.Distinct())
            {
                var alphaGrades = schoolAlphaGrades.Where(x => x.SchoolId == schoolId).Select(x => x.AlphaGrade).ToList();
                res.Add(schoolId, alphaGrades);
            }
            return res;
        }
    }
}
