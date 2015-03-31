using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoClassStorage : BaseDemoIntStorage<Class>
    {
        public DemoClassStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoClassService : DemoSchoolServiceBase, IClassService
    {
        private DemoClassStorage ClassStorage { get; set; }
        
        private DemoClassTeacherStorage ClassTeacherStorage { get; set; }
        private DemoClassPersonStorage ClassPersonStorage { get; set; }
        public DemoClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            ClassStorage = new DemoClassStorage();
            ClassPersonStorage = new DemoClassPersonStorage();
            ClassTeacherStorage = new DemoClassTeacherStorage();
        }
        
        public void Add(IList<Class> classes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            ClassStorage.Add(classes);
        }

        public void Edit(IList<Class> classes)
        {

            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            ClassStorage.Update(classes);
        }
        
        public void Delete(IList<Class> classes)
        {
            ClassStorage.Delete(classes);
        }

        public void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            ServiceLocator.MarkingPeriodService.Add(markingPeriodClasses);
        }
        
        public void AddStudents(IList<ClassPerson> classPersons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            ClassPersonStorage.Add(classPersons);
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
            var res = new List<ClassDetails>();
            var classes = ClassStorage.GetAll();
            var classteachers = ClassTeacherStorage.GetAll();
            foreach (var @class in classes)
            {
                if (@class.SchoolYearRef == schoolYearId)
                {
                    if (classteachers.Any(x => x.ClassRef == @class.Id && x.PersonRef == teacherId))
                    {
                        var cd = GetClassDetailsById(@class.Id);
                        if (!markingPeriodId.HasValue || cd.MarkingPeriodClasses.Any(x => x.MarkingPeriodRef == markingPeriodId.Value))
                            res.Add(cd);
                    }
                }
            }
            return res;
        }

        public IList<ClassDetails> GetStudentClasses(int schoolYearId, int studentId, int? markingPeriodId)
        {
            var res = new List<ClassDetails>();
            var classes = ClassStorage.GetAll();
            var classPersons = ClassPersonStorage.GetAll();
            foreach (var @class in classes)
            {
                if (@class.SchoolYearRef == schoolYearId)
                {
                    if (classPersons.Any(x => x.ClassRef == @class.Id && x.PersonRef == studentId))
                    {
                        var cd = GetClassDetailsById(@class.Id);
                        if (!markingPeriodId.HasValue || cd.MarkingPeriodClasses.Any(x => x.MarkingPeriodRef == markingPeriodId.Value))
                            res.Add(cd);
                    }
                }
            }
            return res;
        }

        public IList<ClassDetails> SearchClasses(string filter)
        {
            var res = new List<ClassDetails>();
            var classes = ClassStorage.GetAll();
            var words = filter.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var @class in classes)
            {
                if (words.Any(x => @class.Name.Contains(x) || @class.ClassNumber.Contains(x)))
                    res.Add(GetClassDetailsById(@class.Id));
            }
            return res;
        }

        public ClassDetails GetClassDetailsById(int id)
        {
            var clazz = GetById(id);
            var clsDetails = new ClassDetails
            {
                ChalkableDepartmentRef = clazz.ChalkableDepartmentRef,
                CourseRef = clazz.CourseRef,
                ClassNumber = clazz.ClassNumber,
                Description = clazz.Description,
                Id = clazz.Id,
                Name = clazz.Name,
                RoomRef = clazz.RoomRef,
                SchoolYearRef = clazz.SchoolYearRef,
                PrimaryTeacher = ServiceLocator.PersonService.GetPerson(DemoSchoolConstants.TeacherId),
                PrimaryTeacherRef = DemoSchoolConstants.TeacherId,
                StudentsCount = 10
            };

            var markingPeriodClasses = MarkingPeriodClassStorage.GetAll();
            if (clsDetails.PrimaryTeacherRef.HasValue)
                clsDetails.PrimaryTeacher = ServiceLocator.PersonService.GetPersonDetails(clsDetails.PrimaryTeacherRef.Value);
            clsDetails.MarkingPeriodClasses = markingPeriodClasses.Where(x => x.ClassRef == clsDetails.Id).ToList();
            var classTeachers = GetClassTeachers(clsDetails.Id, null);
            clsDetails.ClassTeachers = classTeachers;
            return clsDetails;
        }

        public void DeleteMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            throw new NotImplementedException();
        }

        public Class GetById(int id)
        {
            return ClassStorage.GetById(id);
        }

        public IList<Class> GetAll()
        {
            return ClassStorage.GetAll();
        }

        public IList<ClassDetails> GetClassesSortedByPeriod()
        {
            IList<ClassDetails> classes;
            int? teacherId = null;
            int? studentId = null;
            if (Context.RoleId == CoreRoles.TEACHER_ROLE.Id)
            {
                teacherId = Context.PersonId;
                classes = GetTeacherClasses(Context.SchoolYearId.Value, Context.PersonId.Value);
            }
            else if (Context.RoleId == CoreRoles.STUDENT_ROLE.Id)
            {
                studentId = Context.PersonId;
                classes = GetStudentClasses(Context.SchoolYearId.Value, Context.PersonId.Value);
            }
            else
                throw new NotImplementedException();

            var schedule = StorageLocator.ClassPeriodStorage.GetSchedule(teacherId, studentId, null,
                Context.NowSchoolYearTime.Date, Context.NowSchoolYearTime.Date).OrderBy(x => x.PeriodOrder);
            var res = new List<ClassDetails>();
            foreach (var classPeriod in schedule)
            {
                var c = classes.FirstOrDefault(x => x.Id == classPeriod.ClassId);
                if (c != null && res.All(x => x.Id != c.Id))
                    res.Add(c);
            }
            classes = classes.Where(x => res.All(y => y.Id != x.Id)).OrderBy(x => x.Name).ToList();


            var l = res.Concat(classes);
            var classDetailses = l as IList<ClassDetails> ?? l.ToList();
            foreach (var cls in classDetailses)
            {
                cls.ClassTeachers = ClassTeacherStorage.GetClassTeachers(cls.Id, null);
            }
            return classDetailses.ToList();
        }

        public IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled)
        {
            return ClassPersonStorage.GetClassPersons(new ClassPersonQuery
            {
                PersonId = personId,
                IsEnrolled = isEnrolled
            }).ToList();
        }
        
        public void AddTeachers(IList<ClassTeacher> classTeachers)
        {
            ClassTeacherStorage.Add(classTeachers);
        }

        public void EditTeachers(IList<ClassTeacher> classTeachers)
        {
            ClassTeacherStorage.Update(classTeachers);
        }

        public void DeleteTeachers(IList<ClassTeacher> classTeachers)
        {
            ClassTeacherStorage.Delete(classTeachers);
        }

        public IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId)
        {
            return ClassTeacherStorage.GetClassTeachers(classId, teacherId);
        }
        
        public IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId)
        {
            return ClassPersonStorage.GetClassPersons(new ClassPersonQuery
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

        public bool ClassTeacherExists(int classRef, int userId)
        {
            return ClassTeacherStorage.Exists(classRef, userId);
        }

        public bool ClassPersonExists(int classRef, int? userId)
        {
            return ClassPersonStorage.Exists(classRef, userId);
        }
    }
}
