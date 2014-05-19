using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
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

        public void Delete(IList<Pair<int, int>> classTeachersIds)
        {
            SimpleDelete(classTeachersIds.Select(x=> new ClassTeacher{PersonRef = x.First, ClassRef = x.Second}).ToList());
        }

        public bool Exists(int? classId, int? teacherId)
        {
            var conds = new AndQueryCondition();
            if(classId.HasValue)
                conds.Add(ClassTeacher.CLASS_REF_FIELD, classId);
            if(teacherId.HasValue)
                conds.Add(ClassTeacher.PERSON_REF_FIELD, teacherId);
            return Exists<ClassTeacher>(conds);
        }
    }
}
