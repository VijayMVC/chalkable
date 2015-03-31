using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassTeacherStorage : BaseDemoIntStorage<ClassTeacher>
    {
        public DemoClassTeacherStorage() : base( null, true)
        {
        }

        public IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId)
        {
            var res = data.Select(x => x.Value);
            if (classId.HasValue)
                res = res.Where(x => x.ClassRef == classId.Value);
            if (teacherId.HasValue)
                res = res.Where(x => x.PersonRef == teacherId.Value);
            return res.ToList();
        }

        public bool Exists(int? classId, int? teacherId)
        {
            return GetClassTeachers(classId, teacherId).Count > 0;
        }

    }
}
