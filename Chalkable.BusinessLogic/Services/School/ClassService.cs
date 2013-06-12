using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassService
    {
        Class Add(string schoolYearId, string courseInfoId, string name, string description, string teacherId, string gradeLevelId, List<string> markingPeriodsId);
        void Delete(string id);
    }

    public class ClassService : SchoolServiceBase, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test
        public Class Add(string schoolYearId, string courseInfoId, string name, string description, string teacherId,
                         string gradeLevelId, List<string> markingPeriodsId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow);
                var cClass = new Class
                    {
                        Id = Guid.NewGuid(),
                        CourseInfoRef = Guid.Parse(courseInfoId),
                        Description = description,
                        GradeLevelRef = Guid.Parse(gradeLevelId),
                        Name = name,
                        SchoolYearRef = Guid.Parse(schoolYearId),
                        TeacherRef = Guid.Parse(teacherId)
                    };
                da.Create(cClass);
                foreach (var markingPeriodId in markingPeriodsId)
                {
                    AddMarkingPeriod(cClass, markingPeriodId, uow);   
                }
                uow.Commit();
                return cClass;
            }
        }

        private void AddMarkingPeriod(Class cClass, string markingPeriodId, UnitOfWork unitOfWork)
        {
            var markingPeriodDa = new MarkingPeriodDataAccess(unitOfWork);
            var markingPeriod = markingPeriodDa.GetById(Guid.Parse(markingPeriodId));
            if(markingPeriod.SchoolYearRef.ToString() != cClass.SchoolYearRef.ToString())
                throw new ChalkableException(ChlkResources.ERR_CLASS_YEAR_DIFFERS_FROM_MP_YEAR);
            var mpClassDa = new MarkingPeriodClassDataAccess(unitOfWork);
            if (!mpClassDa.Exists(cClass.Id, markingPeriod.Id))
            {
                mpClassDa.Create(new MarkingPeriodClass
                    {
                        Id = Guid.NewGuid(),
                        ClassRef = cClass.Id,
                        MarkingPeriodRef = markingPeriod.Id
                    });
            }
        }


        public void Delete(string id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var guidId = Guid.Parse(id);
                var da = new ClassDataAccess(uow);
                var mpcDa = new MarkingPeriodClassDataAccess(uow);
                var classPersonDa = new ClassPersonDataAccess(uow);
                mpcDa.Delete(guidId);
                classPersonDa.Delete(guidId);
                da.Delete(guidId);
                uow.Commit();
            }
        }
    }
}
