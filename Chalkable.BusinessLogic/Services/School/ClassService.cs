using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassService
    {
        void Add(IList<Class> classes);
        void Edit(IList<Class> classes); 
        void Delete(IList<Class> classes);

        void AddStudents(IList<ClassPerson> classPersons);
        void EditStudents(IList<ClassPerson> classPersons);
        void AddTeachers(IList<ClassTeacher> classTeachers);
        void EditTeachers(IList<ClassTeacher> classTeachers);
        void DeleteTeachers(IList<ClassTeacher> classTeachers);
        void DeleteStudent(IList<ClassPerson> classPersons);

        IList<ClassDetails> GetTeacherClasses(int schoolYearId, int teacherId, int? markingPeriodId = null);
        IList<ClassDetails> GetStudentClasses(int schoolYearId, int studentId, int? markingPeriodId = null);
        IList<ClassDetails> GetClasses(int schoolYearId, int? studentId, int? teacherId, int? markingPeriodId = null); 
        IList<ClassDetails> SearchClasses(string filter);
        
        ClassDetails GetClassDetailsById(int id);
        
        IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId); 
        IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled); 
        IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId); 

        void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses);
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

        public IList<ClassDetails> GetTeacherClasses(int schoolYearId, int teacherId, int? markingPeriodId = null)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetTeacherClasses(schoolYearId, teacherId, markingPeriodId));
        }

        public IList<ClassDetails> GetStudentClasses(int schoolYearId, int studentId, int? markingPeriodId)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetStudentClasses(schoolYearId, studentId, markingPeriodId));
        }

        public IList<ClassDetails> SearchClasses(string filter)
        {
            var sl = filter.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string f1 = null, f2 = null, f3 = null;
            if (sl.Length == 0)
                return new List<ClassDetails>();
            if (sl.Length > 0)
                f1 = sl[0];
            if (sl.Length > 1)
                f2 = sl[1];
            if (sl.Length > 2)
                f3 = sl[2];
            return DoRead(uow => new ClassDataAccess(uow).SearchClasses(f1, f2, f3));
        }

        public ClassDetails GetClassDetailsById(int id)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetClassDetailsById(id));
        }
        
        public Class GetById(int id)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetById(id));
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
        
        public void Delete(IList<Class> classes)
        {
            DoUpdate(uow => new ClassDataAccess(uow).Delete(classes));
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
