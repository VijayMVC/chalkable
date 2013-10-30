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
        ClassDetails Add(int classId, int schoolYearId, Guid? chlkableDepartmentId, string name, string description, int teacherId, int gradeLevelId, List<int> markingPeriodsId);
        ClassDetails Edit(int classId, Guid? chlkableDepartmentId, string name, string description, int teacherId, int gradeLevelId, List<int> markingPeriodsId);
        ClassDetails AddStudent(int classId, int personId);
        ClassDetails DeleteStudent(int classId, int personId);
        ClassDetails GetClassById(int id);

        ClassDetails AddMarkingPeriod(int markingPeriodClassId, int classId, int markingPeriodId);
        ClassDetails DeleteClassFromMarkingPeriod(int classId, int markingPeriodId);

        IList<ClassDetails> GetClasses(int? schoolYearId, int? markingPeriodId, int? personId, int start = 0, int count = int.MaxValue);
        IList<ClassDetails> GetClasses(string filter);
        PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int start = 0, int count = int.MaxValue);
        ClassPerson GetClassPerson(int classId, int personId);
        void Delete(int id);
    }

    public class ClassService : SchoolServiceBase, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test
        public ClassDetails Add(int classId, int schoolYearId, Guid? chlkableDepartmentId, string name
            , string description, int teacherId, int gradeLevelId, List<int> markingPeriodsId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow, Context.SchoolLocalId);
                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetById(schoolYearId);
                var cClass = new Class
                    {
                        Id = classId,
                        ChalkableDepartmentRef = chlkableDepartmentId,
                        Description = description,
                        GradeLevelRef = gradeLevelId,
                        Name = name,
                        SchoolYearRef = schoolYearId,
                        TeacherRef = teacherId,
                        SchoolRef = sy.SchoolRef
                    };
                da.Insert(cClass);
                //CreateMarkingPeriodClasses(cClass, markingPeriodsId, uow);
                uow.Commit();
                return GetClassById(cClass.Id);
            }
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow, Context.SchoolLocalId);
                var mpcDa = new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId);
                var classPersonDa = new ClassPersonDataAccess(uow, Context.SchoolLocalId);
                mpcDa.Delete(new MarkingPeriodClassQuery {ClassId = id});
                classPersonDa.Delete(new ClassPersonQuery{ClassId = id});
                da.Delete(id);
                uow.Commit();
            }
        }

        public ClassDetails Edit(int classId, Guid? chlkableDepartmentId, string name
            , string description, int teacherId, int gradeLevelId, List<int> markingPeriodsId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classDa = new ClassDataAccess(uow, Context.SchoolLocalId);
                var cClass = classDa.GetById(classId);
                var mpcDa = new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId);
                var markingPeriodClasses = mpcDa.GetList(new MarkingPeriodClassQuery{ClassId = classId});
                var mpcForDelete = markingPeriodClasses.Where(x => !markingPeriodsId.Contains(x.MarkingPeriodRef)).ToList();
                var mpIdsForAdd = markingPeriodsId.Where(x => mpcForDelete.Any(y => y.MarkingPeriodRef != x)).ToList();

                mpcDa.Delete(mpcForDelete);
                //CreateMarkingPeriodClasses(cClass, mpIdsForAdd, uow);

                cClass.Name = name;
                cClass.ChalkableDepartmentRef = chlkableDepartmentId;
                cClass.Description = description;
                cClass.TeacherRef = teacherId;
                cClass.GradeLevelRef = gradeLevelId;
                classDa.Update(cClass);
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public ClassDetails AddStudent(int classId, int personId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classPersonDa = new ClassPersonDataAccess(uow, Context.SchoolLocalId);
                var classPeriodDa = new ClassPeriodDataAccess(uow, Context.SchoolLocalId);
                var person = new PersonDataAccess(uow, Context.SchoolLocalId).GetPerson(new PersonQuery
                    {
                        CallerId = Context.UserLocalId,
                        RoleId = Context.Role.Id,
                        PersonId = personId
                    });
                if (person.RoleRef != CoreRoles.STUDENT_ROLE.Id)
                    throw new ChalkableException("Only student can be added to class");

                if (classPeriodDa.IsStudentAlreadyAssignedToClassPeriod(personId, classId))
                    throw new ChalkableException(ChlkResources.ERR_STUDENT_BAD_CLASS_PERIOD);

                if (!classPersonDa.Exists(new ClassPersonQuery {ClassId = classId, PersonId = personId}))
                {
                    var cClass = new ClassDataAccess(uow, Context.SchoolLocalId).GetById(classId);
                    classPersonDa.Insert(new ClassPerson
                        {
                            PersonRef = personId,
                            ClassRef = classId,
                            SchoolRef = cClass.SchoolRef
                        });    
                }
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public ClassDetails DeleteStudent(int classId, int personId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new ClassPersonDataAccess(uow, Context.SchoolLocalId).Delete(new ClassPersonQuery
                    {
                        ClassId = classId,
                        PersonId = personId
                    });
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public ClassDetails GetClassById(int id)
        {
            return GetClasses(new ClassQuery {ClassId = id, Count = 1}).First();
        }

        private void CreateMarkingPeriodClasses(Class cClass, IDictionary<int, int> markingPeriodClassIdsDic, UnitOfWork unitOfWork)
        {
            var mpClassDa = new MarkingPeriodClassDataAccess(unitOfWork, Context.SchoolLocalId);
            var mpClasses = new List<MarkingPeriodClass>();
            foreach (var mpcIdDic in markingPeriodClassIdsDic)
            {
                var mpClass = CreateMarkingPeriodClass(mpcIdDic.Key, cClass, mpcIdDic.Value, unitOfWork);
                if (mpClass != null)
                    mpClasses.Add(mpClass);         
            } 
            mpClassDa.Insert(mpClasses);
        }

        private MarkingPeriodClass CreateMarkingPeriodClass(int markingPeriodClassId, Class cClass, int markingPeriodId, UnitOfWork unitOfWork)
        {
            var markingPeriodDa = new MarkingPeriodDataAccess(unitOfWork, Context.SchoolLocalId);
            var markingPeriod = markingPeriodDa.GetById(markingPeriodId);
            if (markingPeriod.SchoolYearRef != cClass.SchoolYearRef)
                throw new ChalkableException(ChlkResources.ERR_CLASS_YEAR_DIFFERS_FROM_MP_YEAR);
            var mpClassDa = new MarkingPeriodClassDataAccess(unitOfWork, Context.SchoolLocalId);
            if (!mpClassDa.Exists(new MarkingPeriodClassQuery { ClassId = cClass.Id, MarkingPeriodId = markingPeriodId }))
            {
                return new MarkingPeriodClass
                {
                    Id = markingPeriodClassId,
                    ClassRef = cClass.Id,
                    MarkingPeriodRef = markingPeriod.Id,
                    SchoolRef = cClass.SchoolRef
                };
            }
            return null;
        }


        public ClassDetails AddMarkingPeriod(int markingPeriodClassId, int classId, int markingPeriodId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var cDa = new ClassDataAccess(uow, Context.SchoolLocalId);
                var c = cDa.GetById(classId);
                CreateMarkingPeriodClasses(c, new Dictionary<int, int>{{markingPeriodClassId,  markingPeriodId}}, uow);
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public ClassDetails DeleteClassFromMarkingPeriod(int classId, int markingPeriodId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId)
                    .Delete(new MarkingPeriodClassQuery
                    {
                        ClassId = classId,
                        MarkingPeriodId = markingPeriodId
                    });
                uow.Commit();
            }
            return GetClassById(classId);
        }

        public IList<ClassDetails> GetClasses(int? schoolYearId, int? markingPeriodId, int? personId, int start = 0, int count = int.MaxValue)
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
                query.CallerId = Context.UserLocalId.HasValue ? Context.UserLocalId.Value : 0;
                query.CallerRoleId = Context.Role.Id;
                return new ClassDataAccess(uow, Context.SchoolLocalId)
                    .GetClassesComplex(query);
            }
        }

        public ClassPerson GetClassPerson(int classId, int personId)
        {
            using (var uow = Read())
            {
                return new ClassPersonDataAccess(uow, Context.SchoolLocalId)
                    .GetClassPerson(new ClassPersonQuery
                        {
                            ClassId = classId,
                            PersonId = personId
                        });
            }
        }

        public PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int start = 0, int count = int.MaxValue)
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
