using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassTeacherDataAccess : DataAccessBase<ClassTeacher, int>
    {
        public ClassTeacherDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<ClassTeacher> GetClassTeachers(IList<int> classIds)
        {
            string innerSelect = "select * from @classIds";
            string sqlQuery = $"Select * from {nameof(ClassTeacher)} where {ClassTeacher.CLASS_REF_FIELD} in({innerSelect})";
            var @params = new Dictionary<string, object>
            {
                ["classIds"] = classIds
            };

            using (var reader = ExecuteReaderParametrized(sqlQuery, @params))
            {
                return reader.ReadList<ClassTeacher>();
            }
        } 

        public IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId)
        {
            return GetAll(BuildConditioins(classId, teacherId));
        } 

        public bool Exists(int? classId, int? teacherId)
        {
            return Exists<ClassTeacher>(BuildConditioins(classId, teacherId));
        }

        private QueryCondition BuildConditioins(int? classId, int? teacherId)
        {
            var conds = new AndQueryCondition();
            if (classId.HasValue)
                conds.Add(ClassTeacher.CLASS_REF_FIELD, classId);
            if (teacherId.HasValue)
                conds.Add(ClassTeacher.PERSON_REF_FIELD, teacherId);
            return conds;
        }
    }
}
