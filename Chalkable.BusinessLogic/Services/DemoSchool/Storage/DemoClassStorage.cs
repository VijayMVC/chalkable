using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassStorage:BaseDemoIntStorage<Class>
    {
        public DemoClassStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<ClassDetails> GetTeacherClasses(int schoolYearId, int teacherId, int? markingPeriodId = null)
        {
            var res = new List<ClassDetails>();
            var classes = Storage.ClassStorage.GetAll();
            var classteachers = Storage.ClassTeacherStorage.GetAll();
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
            var classes = Storage.ClassStorage.GetAll();
            var classPersons = Storage.ClassPersonStorage.GetAll();
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
                PrimaryTeacher = Storage.PersonStorage.GetById(DemoSchoolConstants.TeacherId),
                PrimaryTeacherRef = DemoSchoolConstants.TeacherId,
                StudentsCount = 10
            };

            var markingPeriodClasses = Storage.MarkingPeriodClassStorage.GetAll();
            if (clsDetails.PrimaryTeacherRef.HasValue)
                clsDetails.PrimaryTeacher = Storage.PersonStorage.GetById(clsDetails.PrimaryTeacherRef.Value);
            clsDetails.MarkingPeriodClasses = markingPeriodClasses.Where(x => x.ClassRef == clsDetails.Id).ToList();
            var classTeachers = Storage.ClassTeacherStorage.GetClassTeachers(clsDetails.Id, null);
            clsDetails.ClassTeachers = classTeachers;
            return clsDetails;
        }


        public IList<ClassDetails> GetClassesSortedByPeriod()
        {
            IList<ClassDetails> classes;
            int? teacherId = null;
            int? studentId = null;
            if (Storage.Context.RoleId == CoreRoles.TEACHER_ROLE.Id)
            {
                teacherId = Storage.Context.PersonId;
                classes = Storage.SchoolLocator.ClassService.GetTeacherClasses(Storage.Context.SchoolYearId.Value, Storage.Context.PersonId.Value);
            }
            else if (Storage.Context.RoleId == CoreRoles.STUDENT_ROLE.Id)
            {
                studentId = Storage.Context.PersonId;
                classes = Storage.SchoolLocator.ClassService.GetStudentClasses(Storage.Context.SchoolYearId.Value, Storage.Context.PersonId.Value);
            }
            else
                throw new NotImplementedException();

             


            var schedule = Storage.SchoolLocator.ClassPeriodService.GetSchedule(teacherId, studentId, null,
                Storage.Context.NowSchoolYearTime.Date, Storage.Context.NowSchoolYearTime.Date).OrderBy(x => x.PeriodOrder);
            var res = new List<ClassDetails>();
            foreach (var classPeriod in schedule)
            {
                var c = classes.FirstOrDefault(x => x.Id == classPeriod.ClassId);
                if (c != null && res.All(x => x.Id != c.Id))
                    res.Add(c);
            }
            classes = classes.Where(x => res.All(y => y.Id != x.Id)).OrderBy(x => x.Name).ToList();

            return res.Concat(classes).ToList();
        }
    }
}
