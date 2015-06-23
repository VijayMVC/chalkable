using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassDataAccess : DataAccessBase<Class, int>
    {
        public ClassDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IList<ClassDetails> GetTeacherClasses(int schoolYearId, int personId, int? markingPeriodId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@schoolYearId", schoolYearId},
                {"@personId", personId},
                {"@markingPeriodId", markingPeriodId},
            };
            using (var reader = ExecuteStoredProcedureReader("spGetTeacherClasses", ps))
            {
                return ReadClasses(reader);
            }
        }

        public IList<ClassDetails> GetStudentClasses(int schoolYearId, int personId, int? markingPeriodId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@schoolYearId", schoolYearId},
                {"@personId", personId},
                {"@markingPeriodId", markingPeriodId},
            };
            using (var reader = ExecuteStoredProcedureReader("spGetStudentClasses", ps))
            {
                return ReadClasses(reader);
            }
        }

        public IList<CourseDetails> GetAdminCourses(int schoolYearId, int gradeLevelId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@schoolYearId", schoolYearId},
                {"@gradeLevelId", gradeLevelId}
            };
            using (var reader = ExecuteStoredProcedureReader("spGetAdminCourses", ps))
            {
                var courses = reader.ReadList<CourseDetails>();
                reader.NextResult();
                var classes = ReadClasses(reader);
                foreach (var course in courses)
                {
                    course.Classes = classes.Where(c => c.CourseRef == course.Id).ToList();
                }
                return courses;
            }
        } 

        public ClassDetails GetClassDetailsById(int id)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@id", id},
            };
            using (var reader = ExecuteStoredProcedureReader("spGetClassById", ps))
            {
                return ReadClasses(reader).First();
            }
        }

        public IList<ClassDetails> SearchClasses(string filter1, string filter2, string filter3)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@filter1", filter1},
                {"@filter2", filter2},
                {"@filter3", filter3},
            };
            using (var reader = ExecuteStoredProcedureReader("spSearchClasses", ps))
            {
                return ReadClasses(reader);
            }
        }

        public static IList<ClassDetails> ReadClasses(SqlDataReader reader)
        {
            var classes = new List<ClassDetails>();
            while (reader.Read())
            {
                var c = reader.Read<ClassDetails>(true);
                if (c.PrimaryTeacherRef.HasValue)
                    c.PrimaryTeacher = reader.Read<Person>(true);
                classes.Add(c);
            }
            reader.NextResult();
            var markingPeriodClasses = reader.ReadList<MarkingPeriodClass>();
            foreach (var classComplex in classes)
            {
                classComplex.MarkingPeriodClasses = markingPeriodClasses.Where(x => x.ClassRef == classComplex.Id).ToList();
            }
            reader.NextResult();
            var classTeachers = reader.ReadList<ClassTeacher>();
            foreach (var classDetailse in classes)
            {
                classDetailse.ClassTeachers = classTeachers.Where(cTeacher => cTeacher.ClassRef == classDetailse.Id).ToList();
            }
            return classes;
        }
    }
}
