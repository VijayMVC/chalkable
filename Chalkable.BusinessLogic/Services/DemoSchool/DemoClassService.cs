using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
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

        public IList<ClassDetails> GetTeacherClasses(int schoolYearId, int teacherId, int? markingPeriodId = null)
        {
            return Storage.ClassStorage.GetTeacherClasses(schoolYearId, teacherId, markingPeriodId);
        }

        public IList<ClassDetails> GetStudentClasses(int schoolYearId, int studentId, int? markingPeriodId)
        {
            return Storage.ClassStorage.GetStudentClasses(schoolYearId, studentId, markingPeriodId);
        }

        public IList<ClassDetails> SearchClasses(string filter)
        {
            var res = new List<ClassDetails>();
            var classes = Storage.ClassStorage.GetAll();
            var words = filter.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var @class in classes)
            {
                if (words.Any(x => @class.Name.Contains(x) || @class.ClassNumber.Contains(x)))
                    res.Add(GetClassDetailsById(@class.Id));
            }
            return res;
        }

        public ClassDetails GetClassDetailsById(int id)
        {
            return Storage.ClassStorage.GetClassDetailsById(id);
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
        
        public IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled)
        {
            return Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
                {
                    PersonId = personId,
                    IsEnrolled = isEnrolled
                }).ToList();
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


        public IList<ClassDetails> GetClasses(int schoolYearId, int? studentId, int? teacherId, int? markingPeriodId = null)
        {
            IList<ClassDetails> classes = new List<ClassDetails>();
            if (studentId.HasValue)
                classes = GetStudentClasses(schoolYearId, studentId.Value, markingPeriodId);
            if (teacherId.HasValue)
                classes = GetTeacherClasses(schoolYearId, teacherId.Value, markingPeriodId);
            return classes;
        }
    }
}
