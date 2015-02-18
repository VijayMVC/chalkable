using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        void Add(IList<Class> classes);
        void Edit(IList<Class> classes); 
        void Delete(IList<int> ids);

        void AddStudents(IList<ClassPerson> classPersons);
        void EditStudents(IList<ClassPerson> classPersons);
        void AddTeachers(IList<ClassTeacher> classTeachers);
        void EditTeachers(IList<ClassTeacher> classTeachers);
        void DeleteTeachers(IList<ClassTeacher> classTeachers);
        void DeleteStudent(IList<ClassPerson> classPersons);
        ClassDetails GetClassDetailsById(int id);
        PaginatedList<ClassDetails> GetClasses(int? schoolYearId, int? markingPeriodId, int? personId, int start = 0, int count = int.MaxValue);
        IList<ClassDetails> GetClasses(string filter);
        PaginatedList<ClassDetails> GetClasses(int? schoolYearId);
        ClassPerson GetClassPerson(int classId, int personId);
        IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId); 
        IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled); 
        IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId); 

        void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses);
        void UnassignClassFromMarkingPeriod(int classId, int markingPeriodId);
        void DeleteMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses);
        Class GetById(int id);
        IList<Class> GetAll();
    }

    public class ClassService : SchoolServiceBase, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<Class> classes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassDataAccess(uow);
                da.Insert(classes);
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

        public void Edit(IList<Class> classes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
         
            using (var uow = Update())
            {

                new ClassDataAccess(uow).Update(classes);
                uow.Commit();
            }
        }

        public void AddStudents(IList<ClassPerson> classPersons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var classPersonDa = new ClassPersonDataAccess(uow);
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
                var classPersonDa = new ClassPersonDataAccess(uow);
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
                new ClassPersonDataAccess(uow).Delete(classPersons);
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

        public Class GetById(int id)
        {
            using (var uow = Read())
            {
                return new ClassDataAccess(uow)
                    .GetById(id);
            }
        }

        public IList<Class> GetAll()
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                return new ClassDataAccess(uow)
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
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Read())
            {
                query.CallerId = Context.PersonId.Value;
                query.CallerRoleId = Context.Role.Id;
                return new ClassDataAccess(uow)
                    .GetClassesComplex(query);
            }
        }
        //TODO: add markingPeriodId param 
        public ClassPerson GetClassPerson(int classId, int personId)
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

        public PaginatedList<ClassDetails> GetClasses(int? schoolYearId)
        {
            return GetClasses(schoolYearId, null, null);
        }

        public IList<ClassDetails> GetClasses(string filter)
        {
            return GetClasses(new ClassQuery {Filter = filter});
        }
        
        public void Delete(IList<int> ids)
        {
            using (var uow = Update())
            {
                new ClassDataAccess(uow).Delete(ids);
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
                return new ClassPersonDataAccess(uow)
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
