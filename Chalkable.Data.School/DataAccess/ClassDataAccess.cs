using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
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

        private const string SP_GET_CLASSES_BY_TEACHERS = "spGetClassesByTeachers";

        public IList<ClassDetails> GetClassesByTeachers(int schoolYearId, IList<int> teacherIds, int start, int count)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>()
            {
                ["schoolYearId"] = schoolYearId,
                ["teacherIds"] = teacherIds ?? new List<int>(),
                ["start"] = start,
                ["count"] = count
            };
            using (var reader = ExecuteStoredProcedureReader(SP_GET_CLASSES_BY_TEACHERS, ps))
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
                {"@filter1", !string.IsNullOrWhiteSpace(filter1) ? string.Format(FILTER_FORMAT,filter1) : null},
                {"@filter2", !string.IsNullOrWhiteSpace(filter2) ? string.Format(FILTER_FORMAT,filter2) : null},
                {"@filter3", !string.IsNullOrWhiteSpace(filter3) ? string.Format(FILTER_FORMAT,filter3) : null},
            };
            using (var reader = ExecuteStoredProcedureReader("spSearchClasses", ps))
            {
                return ReadClasses(reader);
            }
        }

        public static IList<ClassDetails> ReadClasses(DbDataReader reader, bool withPeriods = true)
        {
            var classes = new List<ClassDetails>();
            while (reader.Read())
            {
                var c = reader.Read<ClassDetails>(true);
                if (c.PrimaryTeacherRef.HasValue)
                    c.PrimaryTeacher = reader.Read<Person>(true);
                if (c.SchoolYearRef.HasValue)
                    c.SchoolYear = reader.Read<SchoolYear>(true);
                c.ClassPeriods = new List<ClassPeriod>();
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
            if (withPeriods)
            {
                reader.NextResult();
                var classPeriods = reader.ReadList<ClassPeriod>();
                foreach (var classDetailse in classes)
                {
                    classDetailse.ClassPeriods = classPeriods.Where(cPeriod => cPeriod.ClassRef == classDetailse.Id).ToList();
                }
            }
            return SortClasses(classes);
        }

        private static IList<ClassDetails> SortClasses(IList<ClassDetails> classDetailses)
        {
            var res = classDetailses.Where(x => x.ClassPeriods != null && x.ClassPeriods.Count > 0).ToList();
            res = res.OrderBy(x => x.ClassPeriods.Min(y => y.Period.Order)).ThenBy(x=>x.Name).ToList();
            res.AddRange(classDetailses.Where(x=> x.ClassPeriods == null || x.ClassPeriods.Count == 0).OrderBy(x=>x.Name).ToList());
            return res;
        } 


        public IList<ClassDetails> GetAllSchoolsActiveClasses()
        {
            IDictionary<string, object> ps = new Dictionary<string, object>();
            using (var reader = ExecuteStoredProcedureReader("spGetAllSchoolsActiveClasses", ps))
            {
                return ReadClasses(reader);
            }
        }

        public void Delete(IList<int> ids)
        {
            var ps = new Dictionary<string, object>
            {
                {"@classIds", ids}
            };
            ExecuteStoredProcedure("spDeleteClasses", ps);
        }

        private const string SP_GET_CLASSES_BY_SCHOOL_YEAR = "spGetClassesBySchoolYear";

        public PaginatedList<ClassDetails> GetClassesBySchoolYear(int schoolYearId, int start, int count, string filter, int? teacherId)
        {
            var param = new Dictionary<string, object>()
            {
                ["schoolYearId"] = schoolYearId,
                ["filter"] = string.IsNullOrWhiteSpace(filter) ? null : '%'+ filter+ '%',
                ["start"] = start,
                ["count"] = count,
                ["teacherId"] = teacherId
            };

            return ExecuteStoredProcedurePaginated(SP_GET_CLASSES_BY_SCHOOL_YEAR, param, x=>ReadClasses(x), start, count);
        } 
    }
}
