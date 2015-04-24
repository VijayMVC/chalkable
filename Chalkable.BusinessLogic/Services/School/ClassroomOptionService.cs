using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassroomOptionService
    {
        void Add(IList<ClassroomOption> classroomOptions);
        void Edit(IList<ClassroomOption> classroomOptions);
        void Delete(IList<ClassroomOption> classroomOptions);
        ClassroomOption GetClassOption(int classId);
    }

    public class ClassroomOptionService : SchoolServiceBase, IClassroomOptionService
    {
        public ClassroomOptionService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<ClassroomOption> classroomOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=>new DataAccessBase<ClassroomOption>(u).Insert(classroomOptions));
        }

        public void Edit(IList<ClassroomOption> classroomOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ClassroomOption>(u).Update(classroomOptions));
        }

        public void Delete(IList<ClassroomOption> classroomOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ClassroomOption>(u).Update(classroomOptions));
        }

        public ClassroomOption GetClassOption(int classId)
        {
            return DoRead(u => new DataAccessBase<ClassroomOption, int>(u).GetByIdOrNull(classId));
        }
    }
}
