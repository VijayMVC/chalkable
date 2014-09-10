using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoClassService : DemoSchoolServiceBase, IClassService
    {
        public DemoClassService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
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

            SchoolYear sy = null;
            if (schoolYearId.HasValue)
                sy = Storage.SchoolYearStorage.GetById(schoolYearId.Value);
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
            Storage.ClassStorage.Add(cClass);
            return GetClassDetailsById(classId);
        }

        public void Add(IList<Class> classes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassStorage.Add(classes);
        }

        private bool CanAssignDepartment(Guid? departmentId)
        {
            return !departmentId.HasValue
                   || ServiceLocator.ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartmentById(
                       departmentId.Value) != null;
        }

        public void Edit(IList<Class> classes)
        {

            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassStorage.Update(classes);
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();


            Storage.MarkingPeriodClassStorage.Delete(new MarkingPeriodClassQuery { ClassId = id });
            Storage.ClassPersonStorage.Delete(new ClassPersonQuery { ClassId = id });
            Storage.ClassStorage.Delete(id);

        }

        public void Delete(IList<int> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        public void AssignClassToMarkingPeriod(int classId, int markingPeriodId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            var c = GetClassDetailsById(classId);
            if(!c.SchoolYearRef.HasValue)
                throw new ChalkableException("school year is not assigned for this class");

            var mp = Storage.MarkingPeriodStorage.GetById(markingPeriodId);
            if (mp.SchoolYearRef != c.SchoolYearRef)
                throw new ChalkableException();

            var mpc = new MarkingPeriodClass
            {
                ClassRef = classId,
                MarkingPeriodRef = markingPeriodId,
                SchoolRef = c.SchoolRef.Value
            };

            Storage.MarkingPeriodClassStorage.Add(mpc);

        }

        public void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.MarkingPeriodClassStorage.Add(markingPeriodClasses);
        }

        public ClassDetails Edit(int classId, Guid? chlkableDepartmentId, string name
            , string description, int teacherId, int gradeLevelId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            if (!CanAssignDepartment(chlkableDepartmentId))
                throw new ChalkableException("There are no department with such id");
            
            var cClass = Storage.ClassStorage.GetById(classId);
            if (!(Storage.SchoolPersonStorage.Exists(teacherId, CoreRoles.TEACHER_ROLE.Id, cClass.SchoolRef)))
                throw new ChalkableException("Teacher is not assigned to current school");
                
            cClass.Name = name;
            cClass.ChalkableDepartmentRef = chlkableDepartmentId;
            cClass.Description = description;
            cClass.PrimaryTeacherRef = teacherId;
            cClass.GradeLevelRef = gradeLevelId;
            Storage.ClassStorage.Update(cClass);
            return GetClassDetailsById(classId);
        }

        public ClassDetails AddStudent(int classId, int personId, int markingPeriodId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            
            var cClass = Storage.ClassStorage.GetById(classId);

            var classPerson = new ClassPerson
            {
                PersonRef = personId,
                ClassRef = classId,
                MarkingPeriodRef = markingPeriodId,
                SchoolRef = cClass.SchoolRef.Value
            };
            Storage.ClassPersonStorage.Add(classPerson);
            return GetClassDetailsById(classId);

        }

        public void AddStudents(IList<ClassPerson> classPersons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassPersonStorage.Add(classPersons);
        }

        public void EditStudents(IList<ClassPerson> classPersons)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudent(IList<ClassPerson> classPersons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            throw new NotImplementedException();
        }

        public ClassDetails GetClassDetailsById(int id)
        {
            return GetClasses(new ClassQuery {ClassId = id, Count = 1}).First();
        }
        
        public void UnassignClassFromMarkingPeriod(int classId, int markingPeriodId)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.MarkingPeriodClassStorage.Delete(new MarkingPeriodClassQuery
            {
                ClassId = classId,
                MarkingPeriodId = markingPeriodId
            });
            
        }

        public void DeleteMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.MarkingPeriodClassStorage.Delete(markingPeriodClasses);
        }

        public IList<Person> GetStudents(int classId, bool? isEnrolled = null, int? markingPeriodId = null)
        {
            IList<Person> res = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
            {
                ClassId = classId,
                CallerId = Context.UserLocalId,
                CallerRoleId = Context.Role.Id,
                Count = int.MaxValue,
                RoleId = CoreRoles.STUDENT_ROLE.Id
            });


            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            var enrollmentStatus = isEnrolled.HasValue && isEnrolled.Value
                                      ? StudentEnrollmentStatusEnum.CurrentlyEnrolled
                                      : (StudentEnrollmentStatusEnum?)null;
            var studentSys = Storage.StudentSchoolYearStorage.GetList(sy.Id, enrollmentStatus);
            res = res.Where(x => studentSys.Any(y => y.StudentRef == x.Id)).ToList();
            if (isEnrolled.HasValue)
            {
                var classPersons = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery { ClassId = classId, IsEnrolled = isEnrolled, MarkingPeriodId = markingPeriodId });
                res = res.Where(x => classPersons.Any(y => y.PersonRef == x.Id)).ToList();
            }
         
            return res;
        }

        public Class GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Class> GetAll()
        {
            throw new NotImplementedException();
        }

        public PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int? markingPeriodId, int? personId, int start = 0, int count = int.MaxValue)
        {
            var res =  GetClassesQueryResult(new ClassQuery
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
            query.CallerId = Context.UserLocalId.HasValue ? Context.UserLocalId.Value : 0;
            query.CallerRoleId = Context.Role.Id;
            return Storage.ClassStorage.GetClassesComplex(query);
            
        }
        //TODO: add markingPeriodId param 
        public ClassPerson GetClassPerson(int classId, int personId)
        {
            return Storage.ClassPersonStorage.GetClassPerson(new ClassPersonQuery
            {
                ClassId = classId,
                PersonId = personId
            });
            
        }

        public IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled)
        {
            return Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
                {
                    PersonId = personId,
                    IsEnrolled = isEnrolled
                }).ToList();
        }

        public PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int start = 0, int count = int.MaxValue)
        {
            return GetClasses(schoolYearId, null, null, start, count);
        }

        public IList<ClassDetails> GetClasses(string filter)
        {
            return GetClasses(new ClassQuery {Filter = filter});
        }


        public void AddTeachers(IList<ClassTeacher> classTeachers)
        {
            Storage.ClassTeacherStorage.Add(classTeachers);
        }

        public void EditTeachers(IList<ClassTeacher> classTeachers)
        {
            throw new NotImplementedException();
        }

        public void DeleteTeachers(IList<ClassTeacher> classTeachers)
        {
            throw new NotImplementedException();
        }

        public IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId)
        {
            return Storage.ClassTeacherStorage.GetClassTeachers(classId, teacherId);
        }
        
        public IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId)
        {
            return Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
                {
                    ClassId = classId,
                    IsEnrolled = isEnrolled,
                    MarkingPeriodId = markingPeriodId,
                    PersonId = personId
                }).ToList();
        }
    }
}
