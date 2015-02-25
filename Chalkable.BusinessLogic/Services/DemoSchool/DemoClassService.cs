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
        
        public void Add(IList<Class> classes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassStorage.Add(classes);
        }

        public void Edit(IList<Class> classes)
        {

            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassStorage.Update(classes);
        }
        
        public void Delete(IList<int> ids)
        {
            foreach (var id in ids)
            {
                Storage.MarkingPeriodClassStorage.Delete(new MarkingPeriodClassQuery { ClassId = id });
                Storage.ClassPersonStorage.Delete(new ClassPersonQuery { ClassId = id });
                Storage.ClassStorage.Delete(id);
            }
        }

        public void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.MarkingPeriodClassStorage.Add(markingPeriodClasses);
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

        public Class GetById(int id)
        {
            return Storage.ClassStorage.GetById(id);
        }

        public IList<Class> GetAll()
        {
            return Storage.ClassStorage.GetAll();
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
            query.CallerId = Context.PersonId.HasValue ? Context.PersonId.Value : 0;
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

        public PaginatedList<ClassDetails> GetClasses(int? schoolYearId)
        {
            return GetClasses(schoolYearId, null, null);
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
