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
        ClassDetails Add(Guid schoolYearId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId);
        ClassDetails Edit(Guid classId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId);
        ClassDetails AddStudent(Guid classId, Guid personId);
        ClassDetails DeleteStudent(Guid classId, Guid personId);
        ClassDetails GetClassById(Guid id);

        ClassDetails AddMarkingPeriod(Guid classId, Guid markingPeriodId);
        ClassDetails DeleteClassFromMarkingPeriod(Guid classId, Guid markingPeriodId);

        IList<ClassDetails> GetClasses(Guid? schoolYearId, Guid? markingPeriodId, Guid? personId, int start = 0, int count = int.MaxValue);
        IList<ClassDetails> GetClasses(string filter);
        PaginatedList<ClassDetails> GetClasses(Guid? schoolYearId, int start = 0, int count = int.MaxValue);
        ClassPerson GetClassPerson(Guid classId, Guid personId);
        void Delete(Guid id);
    }

    public class ClassService : SchoolServiceBase, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test
        public ClassDetails Add(Guid schoolYearId, Guid courseInfoId, string name, string description, Guid teacherId,
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

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow);
                var mpcDa = new MarkingPeriodClassDataAccess(uow);
                var classPersonDa = new ClassPersonDataAccess(uow);
                mpcDa.Delete(new MarkingPeriodClassQuery {ClassId = id});
                classPersonDa.Delete(new ClassPersonQuery{ClassId = id});
                da.Delete(id);
                uow.Commit();
            }
        }

        public ClassDetails Edit(Guid classId, Guid courseInfoId, string name, string description, Guid teacherId, Guid gradeLevelId, List<Guid> markingPeriodsId)
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

        public ClassDetails AddStudent(Guid classId, Guid personId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classPersonDa = new ClassPersonDataAccess(uow);
                var classPeriodDa = new ClassPeriodDataAccess(uow);
                var person = new PersonDataAccess(uow).GetById(personId);
                if(CoreRoles.GetById(person.RoleRef) != CoreRoles.STUDENT_ROLE)
                    throw new ChalkableException("Only student can be added to class");

                if(classPeriodDa.IsStudentAlreadyAssignedToClassPeriod(personId, classId))
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

        public ClassDetails DeleteStudent(Guid classId, Guid personId)
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

        public ClassDetails GetClassById(Guid id)
        {
            return GetClasses(new ClassQuery {ClassId = id, Count = 1}).First();
        }

        private void CreateMarkingPeriodClasses(Class cClass, IEnumerable<Guid> markingPeriodIds, UnitOfWork unitOfWork)
        {
            var mpClassDa = new MarkingPeriodClassDataAccess(unitOfWork);
            var mpClasses = new List<MarkingPeriodClass>();
            foreach (var markingPeriodId in markingPeriodIds)
            {
                var mpClass = CreateMarkingPeriodClass(cClass, markingPeriodId, unitOfWork);
                if (mpClass != null)
                {
                    mpClasses.Add(mpClass);         
                }
            } 
            mpClassDa.Insert(mpClasses);
        }

        private MarkingPeriodClass CreateMarkingPeriodClass(Class cClass, Guid markingPeriodId, UnitOfWork unitOfWork)
        {
            var markingPeriodDa = new MarkingPeriodDataAccess(unitOfWork);
            var markingPeriod = markingPeriodDa.GetById(markingPeriodId);
            if (markingPeriod.SchoolYearRef != cClass.SchoolYearRef)
                throw new ChalkableException(ChlkResources.ERR_CLASS_YEAR_DIFFERS_FROM_MP_YEAR);
            var mpClassDa = new MarkingPeriodClassDataAccess(unitOfWork);
            if (!mpClassDa.Exists(new MarkingPeriodClassQuery { ClassId = cClass.Id, MarkingPeriodId = markingPeriodId }))
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

        
        public ClassDetails AddMarkingPeriod(Guid classId, Guid markingPeriodId)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var cDa = new ClassDataAccess(uow);
                var c = cDa.GetById(classId);
                CreateMarkingPeriodClasses(c, new List<Guid> {markingPeriodId}, uow);
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public ClassDetails DeleteClassFromMarkingPeriod(Guid classId, Guid markingPeriodId)
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
        
        public IList<ClassDetails> GetClasses(Guid? schoolYearId, Guid? markingPeriodId, Guid? personId, int start = 0, int count = int.MaxValue)
        {
            return GetClasses(new ClassQuery
                {
                    SchoolYearId = schoolYearId,
                    MarkingPeriodId = markingPeriodId,
                    PersonId = personId,
                    Start = start,
                    Count = count
                });
        }

        private IList<ClassDetails> GetClasses(ClassQuery query)
        {
            return GetClassesQueryResult(query).Classes;
        } 

        private  ClassQueryResult GetClassesQueryResult(ClassQuery query)
        {
            using (var uow = Read())
            {
                query.CallerId = Context.UserId;
                return new ClassDataAccess(uow)
                    .GetClassesComplex(query);
            }
        }

        public ClassPerson GetClassPerson(Guid classId, Guid personId)
        {
            using (var uow = Read())
            {
                return new ClassPersonDataAccess(uow)
                    .GetClassPerson(new ClassPersonQuery
                        {
                            ClassId = classId,
                            PersonId = personId
                        });
            }
        }

        public PaginatedList<ClassDetails> GetClasses(Guid? schoolYearId, int start = 0, int count = int.MaxValue)
        {
            var res = GetClassesQueryResult(new ClassQuery
                {
                    SchoolYearId = schoolYearId,
                    Start = start,
                    Count = count
                });
            return new PaginatedList<ClassDetails>(res.Classes, start / count, count, res.SourceCount);
        }

        public IList<ClassDetails> GetClasses(string filter)
        {
            return GetClasses(new ClassQuery {Filter = filter});
        }
    }
}
