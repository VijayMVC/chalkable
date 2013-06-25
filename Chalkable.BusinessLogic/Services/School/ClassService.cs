using System;
using System.Collections.Generic;
using System.Linq;
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
        ClassComplex Add(Guid schoolYearId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId);
        ClassComplex Edit(Guid classId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId);
        ClassComplex AddStudent(Guid classId, Guid personId);
        ClassComplex DeleteStudent(Guid classId, Guid personId);
        ClassComplex GetClassById(Guid id);

        ClassComplex AddMarkingPeriod(Guid classId, Guid markingPeriodId);
        ClassComplex DeleteClassFromMarkingPeriod(Guid classId, Guid markingPeriodId);

        IList<ClassComplex> GetClasses(Guid? schoolYearId, Guid? markingPeriodId, Guid? personId, int start = 0, int count = int.MaxValue);

        void Delete(Guid id);
    }

    public class ClassService : SchoolServiceBase, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test
        public ClassComplex Add(Guid schoolYearId, Guid courseInfoId, string name, string description, Guid teacherId,
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
                        CourseRef = courseInfoId,
                        Description = description,
                        GradeLevelRef = gradeLevelId,
                        Name = name,
                        SchoolYearRef = schoolYearId,
                        TeacherRef = teacherId
                    };
                da.Insert(cClass);
                CreateMarkingPeriodClasses(cClass, markingPeriodsId, uow);
                uow.Commit();
                return GetClassById(cClass.Id);
            }
        }

        private MarkingPeriodClass CreateMarkingPeriodClass(Class cClass, Guid markingPeriodId, UnitOfWork unitOfWork)
        {
            var markingPeriodDa = new MarkingPeriodDataAccess(unitOfWork);
            var markingPeriod = markingPeriodDa.GetById(markingPeriodId);
            if(markingPeriod.SchoolYearRef.ToString() != cClass.SchoolYearRef.ToString())
                throw new ChalkableException(ChlkResources.ERR_CLASS_YEAR_DIFFERS_FROM_MP_YEAR);
            var mpClassDa = new MarkingPeriodClassDataAccess(unitOfWork);
            if (!mpClassDa.Exists(new MarkingPeriodClassQuery{ClassId = cClass.Id, MarkingPeriodId = markingPeriodId}))
            {
                return new MarkingPeriodClass
                    {
                        Id = Guid.NewGuid(),
                        ClassRef = cClass.Id,
                        MarkingPeriodRef = markingPeriod.Id
                    };
            }
            return null;
        }

        private void CreateMarkingPeriodClasses(Class cClass, IEnumerable<Guid> markingPeriodIds, UnitOfWork unitOfWork)
        {
            var mpClassDa = new MarkingPeriodClassDataAccess(unitOfWork);
            var mpClasses = markingPeriodIds.Select(mpId => CreateMarkingPeriodClass(cClass, mpId, unitOfWork)).ToList();
            mpClassDa.Insert(mpClasses);
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
                mpcDa.Delete(new MarkingPeriodClassQuery {Id = id});
                classPersonDa.Delete(new ClassPersonQuery{Id = id});
                da.Delete(id);
                uow.Commit();
            }
        }

        public ClassComplex Edit(Guid classId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classDa = new ClassDataAccess(uow);
                var cClass = classDa.GetById(classId);
                var mpcDa = new MarkingPeriodClassDataAccess(uow);
                var markingPeriodClasses = mpcDa.GetList(new MarkingPeriodClassQuery{ClassId = classId});
                var mpcForDelete = markingPeriodClasses.Where(x => !markingPeriodsId.Contains(x.MarkingPeriodRef)).ToList();
                var mpIdsForAdd = markingPeriodsId.Where(x => mpcForDelete.Any(y => y.MarkingPeriodRef != x)).ToList();

                mpcDa.Delete(mpcForDelete);
                CreateMarkingPeriodClasses(cClass, mpIdsForAdd, uow);

                cClass.Name = name;
                cClass.CourseRef = courseInfoId;
                cClass.Description = description;
                cClass.TeacherRef = teacherId;
                cClass.GradeLevelRef = gradeLevelId;
                classDa.Update(cClass);
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public ClassComplex AddStudent(Guid classId, Guid personId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classPersonDa = new ClassPersonDataAccess(uow);
                var classPeriodDa = new ClassPeriodDataAccess(uow);
                if(!classPeriodDa.IsStudentAlreadyAssignedToClassPeriod(personId, classId))
                    throw new ChalkableException(ChlkResources.ERR_STUDENT_BAD_CLASS_PERIOD);

                if (!classPersonDa.Exists(new ClassPersonQuery {ClassId = classId, PersonId = personId}))
                {
                    classPersonDa.Insert(new ClassPerson
                        {
                            Id = Guid.NewGuid(),
                            PersonRef = personId,
                            ClassRef = classId
                        });    
                }
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public ClassComplex DeleteStudent(Guid classId, Guid personId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new ClassPersonDataAccess(uow).Delete(new ClassPersonQuery{ClassId = classId, PersonId = personId});
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public ClassComplex GetClassById(Guid id)
        {
            using (var uow = Read())
            {
                return new ClassDataAccess(uow)
                            .GetClassesComplex(new ClassQuery {ClassId = id}, Context.UserId)
                            .Classes.First();
            }
        }
        
        public ClassComplex AddMarkingPeriod(Guid classId, Guid markingPeriodId)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
                var cDa = new ClassDataAccess(uow);
                var mpcDa = new MarkingPeriodClassDataAccess(uow); 
                var c = cDa.GetById(classId);
                if (mp.SchoolYearRef != c.SchoolYearRef)
                    throw new ChalkableException(ChlkResources.ERR_CLASS_YEAR_DIFFERS_FROM_MP_YEAR);

                if (!mpcDa.Exists(new MarkingPeriodClassQuery{ClassId = classId, MarkingPeriodId = markingPeriodId}))
                {
                    mpcDa.Insert(new MarkingPeriodClass
                        {
                            Id = Guid.NewGuid(),
                            ClassRef = classId,
                            MarkingPeriodRef = markingPeriodId
                        });
                }
                uow.Commit();
            }
            
            return GetClassById(classId);
        }

        public ClassComplex DeleteClassFromMarkingPeriod(Guid classId, Guid markingPeriodId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new MarkingPeriodClassDataAccess(uow)
                    .Delete(new MarkingPeriodClassQuery
                    {
                        ClassId = classId,
                        MarkingPeriodId = markingPeriodId
                    });
                uow.Commit();
            }
            return GetClassById(classId);
        }
        
        public IList<ClassComplex> GetClasses(Guid? schoolYearId, Guid? markingPeriodId, Guid? personId, int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                return new ClassDataAccess(uow)
                    .GetClassesComplex(new ClassQuery
                        {
                            SchoolYearId = schoolYearId,
                            MarkingPeriodId = markingPeriodId,
                            PersonId = personId,
                            Start = start,
                            Count = count
                        }, Context.UserId).Classes;
            }
        }
    }
}
