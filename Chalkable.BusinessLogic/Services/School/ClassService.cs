using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassService
    {
        ClassDetails Add(int classId, int? schoolYearId, Guid? chlkableDepartmentId, string name, string description, int? teacherId, int gradeLevelId);
        ClassDetails Edit(int classId, Guid? chlkableDepartmentId, string name, string description, int teacherId, int gradeLevelId);
        void Delete(int id);

        ClassDetails AddStudent(int classId, int personId);
        ClassDetails DeleteStudent(int classId, int personId);
        ClassDetails GetClassById(int id);
        IList<ClassDetails> GetClasses(int? schoolYearId, int? markingPeriodId, int? personId, int start = 0, int count = int.MaxValue);
        IList<ClassDetails> GetClasses(string filter);
        PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int start = 0, int count = int.MaxValue);
        ClassPerson GetClassPerson(int classId, int personId);
        
        void AssignClassToMarkingPeriod(int classId, int markingPeriodId);
        void UnassignClassFromMarkingPeriod(int classId, int markingPeriodId);
    }

    public class ClassService : SchoolServiceBase, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test
        public ClassDetails Add(int classId, int? schoolYearId, Guid? chlkableDepartmentId, string name
            , string description, int? teacherId, int gradeLevelId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow, Context.SchoolLocalId);
                SchoolYear sy = null;
                if (schoolYearId.HasValue)
                    sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetById(schoolYearId.Value);
                var cClass = new Class
                    {
                        Id = classId,
                        ChalkableDepartmentRef = chlkableDepartmentId,
                        Description = description,
                        GradeLevelRef = gradeLevelId,
                        Name = name,
                        SchoolYearRef = schoolYearId,
                        TeacherRef = teacherId,
                        SchoolRef = sy != null ? sy.SchoolRef : (int?)null
                    };
                da.Insert(cClass);
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

        public void AssignClassToMarkingPeriod(int classId, int markingPeriodId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            var schoolId = GetClassById(classId).SchoolRef;
            using (var uow = Read())
            {
                var mpClassDa = new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId);    
                var mpc = new MarkingPeriodClass
                {
                    ClassRef = classId,
                    MarkingPeriodRef = markingPeriodId,
                    SchoolRef = schoolId.Value
                };
                mpClassDa.Insert(mpc);
            }
        }

        public ClassDetails Edit(int classId, Guid? chlkableDepartmentId, string name
            , string description, int teacherId, int gradeLevelId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classDa = new ClassDataAccess(uow, Context.SchoolLocalId);
                var cClass = classDa.GetById(classId);
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
                            SchoolRef = cClass.SchoolRef.Value
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
        
        public void UnassignClassFromMarkingPeriod(int classId, int markingPeriodId)
        {
            if(!BaseSecurity.IsDistrict(Context))
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
