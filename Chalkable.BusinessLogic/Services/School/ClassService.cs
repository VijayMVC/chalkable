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
        Class Add(Guid schoolYearId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId);
        Class Edit(Guid classId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId);
        Class AddStudent(Guid classId, Guid personId);
        Class DeleteStudent(Guid classId, Guid personId);
        void Delete(Guid id);
    }

    public class ClassService : SchoolServiceBase, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test
        public Class Add(Guid schoolYearId, Guid courseInfoId, string name, string description, Guid teacherId,
                         Guid gradeLevelId, List<Guid> markingPeriodsId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow);
                var cClass = new Class
                    {
                        Id = Guid.NewGuid(),
                        CourseInfoRef = courseInfoId,
                        Description = description,
                        GradeLevelRef = gradeLevelId,
                        Name = name,
                        SchoolYearRef = schoolYearId,
                        TeacherRef = teacherId
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

        private void AddMarkingPeriod(Class cClass, Guid markingPeriodId, UnitOfWork unitOfWork)
        {
            var markingPeriodDa = new MarkingPeriodDataAccess(unitOfWork);
            var markingPeriod = markingPeriodDa.GetById(markingPeriodId);
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


        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow);
                var mpcDa = new MarkingPeriodClassDataAccess(uow);
                var classPersonDa = new ClassPersonDataAccess(uow);
                mpcDa.Delete(id);
                classPersonDa.Delete(id);
                da.Delete(id);
                uow.Commit();
            }
        }

        public Class Edit(Guid classId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classDa = new ClassDataAccess(uow);
                var cClass = classDa.GetById(classId);
                var mpcDa = new MarkingPeriodClassDataAccess(uow);
                var markingPeriodClasses = mpcDa.GetList(classId);
                var mpcForDelete = markingPeriodClasses.Where(x => !markingPeriodsId.Contains(x.MarkingPeriodRef));
                var mpIdsForAdd = markingPeriodsId.Where(x => mpcForDelete.Any(y => y.MarkingPeriodRef != x));

                foreach (var markingPeriodClass in mpcForDelete)
                {
                    mpcDa.Delete(markingPeriodClass);
                }
                foreach (var mpId in mpIdsForAdd)
                {
                    AddMarkingPeriod(cClass, mpId, uow);
                }
                cClass.Name = name;
                cClass.CourseInfoRef = courseInfoId;
                cClass.Description = description;
                cClass.TeacherRef = teacherId;
                cClass.GradeLevelRef = gradeLevelId;
                classDa.Update(cClass);
                uow.Commit();
                return cClass;
            }
        }

        public Class AddStudent(Guid classId, Guid personId)
        {
            throw new NotImplementedException();
        }

        public Class DeleteStudent(Guid classId, Guid personId)
        {
            throw new NotImplementedException();
        }
    }
}
