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
        ClassDetails Add(int classId, int? schoolYearId, Guid? chlkableDepartmentId, string name, string description, int? teacherId, int gradeLevelId, int? roomId = null);
        void Add(IList<Class> classes);
        ClassDetails Edit(int classId, Guid? chlkableDepartmentId, string name, string description, int teacherId, int gradeLevelId);
        void Edit(IList<Class> classes); 
        void Delete(int id);
        void Delete(IList<int> ids);

        ClassDetails AddStudent(int classId, int personId, int markingPeriodId);
        void AddStudents(IList<ClassPerson> classPersons);
        void EditStudents(IList<ClassPerson> classPersons);
        void AddTeachers(IList<ClassTeacher> classTeachers);
        void EditTeachers(IList<ClassTeacher> classTeachers);
        void DeleteTeachers(IList<ClassTeacher> classTeachers);
        void DeleteStudent(IList<ClassPerson> classPersons);
        ClassDetails GetClassDetailsById(int id);
        PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int? markingPeriodId, int? personId, int start = 0, int count = int.MaxValue);
        IList<ClassDetails> GetClasses(string filter);
        PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int start = 0, int count = int.MaxValue);
        ClassPerson GetClassPerson(int classId, int personId);
        IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId); 
        IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled); 
        IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId); 

        void AssignClassToMarkingPeriod(int classId, int markingPeriodId);
        void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses);
        void UnassignClassFromMarkingPeriod(int classId, int markingPeriodId);
        void DeleteMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses);
        IList<Person> GetStudents(int classId, bool? isEnrolled = null, int? markingPeriodId = null);
        Class GetById(int id);
        IList<Class> GetAll();
    }

    public class ClassService : SchoolServiceBase, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test
        public ClassDetails Add(int classId, int? schoolYearId, Guid? chlkableDepartmentId, string name
            , string description, int? teacherId, int gradeLevelId, int? roomId = null)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            if (!CanAssignDepartment(chlkableDepartmentId))
                   throw new ChalkableException("There are no department with such id");
            
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
                        PrimaryTeacherRef = teacherId,
                        RoomRef = roomId,
                        SchoolRef = sy != null ? sy.SchoolRef : (int?)null
                    };
                da.Insert(cClass);
                uow.Commit();
                return GetClassDetailsById(classId);
            }
        }

        public void Add(IList<Class> classes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow, Context.SchoolLocalId);
                da.Insert(classes);
                uow.Commit();
            }
        }

        private bool CanAssignDepartment(Guid? departmentId)
        {
            return !departmentId.HasValue
                   || ServiceLocator.ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartmentById(
                       departmentId.Value) != null;
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new ClassDataAccess(uow, Context.SchoolLocalId).Delete(id);
                uow.Commit();
            }
        }

        public void AssignClassToMarkingPeriod(int classId, int markingPeriodId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            var c = GetClassDetailsById(classId);
            if(!c.SchoolYearRef.HasValue)
                throw new ChalkableException("school year is not assigned for this class");
            if (!c.SchoolRef.HasValue)
                throw new ChalkableException(string.Format("Class {0} is not assigned to any school. Looks like it's a general course", c.Name));
            using (var uow = Update())
            {
                var mpClassDa = new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId);
                var mp = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).GetById(markingPeriodId);
                if(mp.SchoolYearRef != c.SchoolYearRef)
                    throw new ChalkableException();
                var mpc = new MarkingPeriodClass
                {
                    ClassRef = classId,
                    MarkingPeriodRef = markingPeriodId,
                    SchoolRef = c.SchoolRef.Value
                };
                mpClassDa.Insert(mpc);
                uow.Commit();
            }
        }

        public void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var mpClassDa = new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId);
                mpClassDa.Insert(markingPeriodClasses);
                uow.Commit();
            }
        }

        public ClassDetails Edit(int classId, Guid? chlkableDepartmentId, string name
            , string description, int teacherId, int gradeLevelId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            if (!CanAssignDepartment(chlkableDepartmentId))
                throw new ChalkableException("There are no department with such id");
            
            using (var uow = Update())
            {
                var classDa = new ClassDataAccess(uow, Context.SchoolLocalId);
                var cClass = classDa.GetById(classId);
                if (!(new SchoolPersonDataAccess(uow).Exists(teacherId, CoreRoles.TEACHER_ROLE.Id, cClass.SchoolRef)))
                    throw new ChalkableException("Teacher is not assigned to current school");
                
                cClass.Name = name;
                cClass.ChalkableDepartmentRef = chlkableDepartmentId;
                cClass.Description = description;
                cClass.PrimaryTeacherRef = teacherId;
                cClass.GradeLevelRef = gradeLevelId;
                classDa.Update(cClass);
                uow.Commit();
            }
            return GetClassDetailsById(classId);
        }


        public void Edit(IList<Class> classes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
         
            using (var uow = Update())
            {

                new ClassDataAccess(uow, Context.SchoolLocalId).Update(classes);
                uow.Commit();
            }
        }

        public ClassDetails AddStudent(int classId, int personId, int markingPeriodId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                var classPersonDa = new ClassPersonDataAccess(uow, Context.SchoolLocalId);
                var cClass = new ClassDataAccess(uow, Context.SchoolLocalId).GetById(classId);
                if (!cClass.SchoolRef.HasValue)
                    throw new ChalkableException(string.Format("Class {0} is not assigned to any school. Looks like it's a general course", cClass.Name));
                classPersonDa.Insert(new ClassPerson
                {
                    PersonRef = personId,
                    ClassRef = classId,
                    MarkingPeriodRef = markingPeriodId,
                    SchoolRef = cClass.SchoolRef.Value
                });    
                uow.Commit();
            }
            return GetClassDetailsById(classId);
        }

        public void AddStudents(IList<ClassPerson> classPersons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classPersonDa = new ClassPersonDataAccess(uow, Context.SchoolLocalId);
                classPersonDa.Insert(classPersons);
                uow.Commit();
            }
        }

        public void EditStudents(IList<ClassPerson> classPersons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classPersonDa = new ClassPersonDataAccess(uow, Context.SchoolLocalId);
                classPersonDa.Update(classPersons);
                uow.Commit();
            }
        }

        public void DeleteStudent(IList<ClassPerson> classPersons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new ClassPersonDataAccess(uow, Context.SchoolLocalId).Delete(classPersons);
                uow.Commit();
            }
        }

        public ClassDetails GetClassDetailsById(int id)
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

        public IList<Person> GetStudents(int classId, bool? isEnrolled = null, int? markingPeriodId = null)
        {
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            IList<Person> res = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
                {
                    ClassId = classId,
                    CallerId = Context.PersonId,
                    CallerRoleId = Context.Role.Id,
                    Count = int.MaxValue,
                    RoleId = CoreRoles.STUDENT_ROLE.Id,
                    MarkingPeriodId = markingPeriodId,
                    IsEnrolled = isEnrolled
                });
             using (var uow = Read())
             {
                 //todo : maybe move this to Getpersons procedure
                 var enrollentStatus = isEnrolled.HasValue && isEnrolled.Value 
                                           ? StudentEnrollmentStatusEnum.CurrentlyEnrolled
                                           : (StudentEnrollmentStatusEnum?) null;
                 var studentSys = new StudentSchoolYearDataAccess(uow).GetList(Context.SchoolYearId.Value, enrollentStatus);
                 res = res.Where(x => studentSys.Any(y => y.StudentRef == x.Id)).ToList();

                 var da = new ClassPersonDataAccess(uow, Context.SchoolLocalId);
                 var classPersons = da.GetClassPersons(new ClassPersonQuery { ClassId = classId, MarkingPeriodId = markingPeriodId });
                 foreach (var person in res)
                 {
                     person.IsWithdrawn = classPersons.Any(x => x.PersonRef == person.Id && !x.IsEnrolled);
                 }
                 //if (isEnrolled.HasValue)
                 //{
                 //    var da = new ClassPersonDataAccess(uow, Context.SchoolLocalId);
                 //    var classPersons = da.GetClassPersons(new ClassPersonQuery { ClassId = classId, IsEnrolled = isEnrolled, MarkingPeriodId = markingPeriodId });
                 //    res = res.Where(x => classPersons.Any(y => y.PersonRef == x.Id)).ToList();
                 //}
             }                 
            return res;
        }

        public Class GetById(int id)
        {
            using (var uow = Read())
            {
                return new ClassDataAccess(uow, Context.SchoolLocalId)
                    .GetById(id);
            }
        }

        public IList<Class> GetAll()
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                return new ClassDataAccess(uow, Context.SchoolLocalId)
                    .GetAll();
            } 
        }

        public PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int? markingPeriodId, int? personId, int start = 0, int count = int.MaxValue)
        {
            var  res = GetClassesQueryResult(new ClassQuery
                {
                    SchoolYearId = schoolYearId,
                    MarkingPeriodId = markingPeriodId,
                    PersonId = personId,
                    Start = start,
                    Count = count
                });
            return new PaginatedList<ClassDetails>(res.Classes, start / count, count, res.SourceCount);
        }

        private IList<ClassDetails> GetClasses(ClassQuery query)
        {
            return GetClassesQueryResult(query).Classes;
        } 

        private  ClassQueryResult GetClassesQueryResult(ClassQuery query)
        {
            using (var uow = Read())
            {
                query.CallerId = Context.PersonId.HasValue ? Context.PersonId.Value : 0;
                query.CallerRoleId = Context.Role.Id;
                return new ClassDataAccess(uow, Context.SchoolLocalId)
                    .GetClassesComplex(query);
            }
        }
        //TODO: add markingPeriodId param 
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
            return GetClasses(schoolYearId, null, null, start, count);
        }

        public IList<ClassDetails> GetClasses(string filter)
        {
            return GetClasses(new ClassQuery {Filter = filter});
        }
        
        public void Delete(IList<int> ids)
        {
            using (var uow = Update())
            {
                new ClassDataAccess(uow, Context.SchoolLocalId).Delete(ids);
                uow.Commit();
            }
        }

        public void DeleteMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId).Delete(markingPeriodClasses);
                uow.Commit();
            }
        }

        public void AddTeachers(IList<ClassTeacher> classTeachers)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new ClassTeacherDataAccess(uow).Insert(classTeachers);
                uow.Commit();
            }
        }

        public void EditTeachers(IList<ClassTeacher> classTeachers)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new ClassTeacherDataAccess(uow).Update(classTeachers);
                uow.Commit();
            }
        }

        public void DeleteTeachers(IList<ClassTeacher> classTeachers)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new ClassTeacherDataAccess(uow).Delete(classTeachers);
                uow.Commit();
            }
        }


        public IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId)
        {
            using (var uow = Read())
            {
                return new ClassTeacherDataAccess(uow).GetClassTeachers(classId, teacherId);
            }
        }


        public IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled)
        {
            return GetClassPersons(personId, null, isEnrolled, null);
        }

        public IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId)
        {
            using (var uow = Read())
            {
                return new ClassPersonDataAccess(uow, Context.SchoolLocalId)
                    .GetClassPersons(new ClassPersonQuery
                        {
                            ClassId = classId,
                            IsEnrolled = isEnrolled,
                            PersonId = personId,
                            MarkingPeriodId = markingPeriodId
                        });
            }
        }
    }
}
