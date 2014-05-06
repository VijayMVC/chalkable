using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassroomOptionService
    {
        void Add(IList<ClassroomOption> classroomOptions);
        void Edit(IList<ClassroomOption> classroomOptions);
        void Delete(IList<int> classroomOptionsIds);
        ClassroomOption GetById(int classId);
    }

    public class ClassroomOptionService : SchoolServiceBase, IClassroomOptionService
    {
        public ClassroomOptionService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<ClassroomOption> classroomOptions)
        {
            Modify(da=>da.Insert(classroomOptions));
        }

        public void Edit(IList<ClassroomOption> classroomOptions)
        {
            Modify(da => da.Update(classroomOptions));
        }

        public void Delete(IList<int> classroomOptionsIds)
        {
            Modify(da => da.Delete(classroomOptionsIds));
        }

        public ClassroomOption GetById(int classId)
        {
            using (var uow = Read())
            {
                return new ClassroomOptionDataAccess(uow).GetById(classId);
            }
        }

        private void Modify(Action<ClassroomOptionDataAccess> modifyMethods)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassroomOptionDataAccess(uow);
                modifyMethods(da);
                uow.Commit();
            }
        }

    }
}
