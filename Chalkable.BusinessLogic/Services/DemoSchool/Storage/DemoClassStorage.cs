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

        public ClassQueryResult GetClassesComplex(ClassQuery query)
        {
            var classes = data.Select(x => x.Value);

            var classDetailsList = new List<ClassDetails>();

            if (query.ClassId.HasValue)
                classes = classes.Where(x => x.Id == query.ClassId);

            if (query.SchoolYearId.HasValue)
                classes = classes.Where(x => x.SchoolYearRef == query.SchoolYearId);
            if (query.MarkingPeriodId.HasValue)
                classes = classes.Where(x => Storage.MarkingPeriodClassStorage.Exists(x.Id, query.MarkingPeriodId));

            string filter1 = null;
            string filter2 = null;
            string filter3 = null;
            if (!string.IsNullOrEmpty(query.Filter))
            {
                string[] sl = query.Filter.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sl.Length > 0)
                    filter1 = sl[0];
                if (sl.Length > 1)
                    filter2 = sl[1];
                if (sl.Length > 2)
                    filter3 = sl[2];
            }

            if (query.CallerRoleId == 3)
            {
                classes = classes.Where(x => Storage.ClassPersonStorage.Exists(new ClassPersonQuery
                {
                    ClassId = x.Id,
                    PersonId = query.CallerId
                }));
            }

            if (query.PersonId.HasValue)
            {

                var roleId = query.PersonId.HasValue
                ? Storage.SchoolPersonStorage.GetRoleId(query.PersonId.Value, DemoSchoolConstants.SchoolId)
                : (int?)null;

                if (roleId == CoreRoles.TEACHER_ROLE.Id)
                    classes = classes.Where(x => x.PrimaryTeacherRef == query.PersonId);

                if (roleId == CoreRoles.STUDENT_ROLE.Id)
                    classes = classes.Where(x => Storage.ClassPersonStorage.Exists(new ClassPersonQuery
                    {
                        ClassId = x.Id,
                        PersonId = query.PersonId
                    }));
            }

            if (!string.IsNullOrEmpty(filter1))
                classes = classes.Where(x => x.Name.Contains(filter1));
            if (!string.IsNullOrEmpty(filter2))
                classes = classes.Where(x => x.Name.Contains(filter2));
            if (!string.IsNullOrEmpty(filter3))
                classes = classes.Where(x => x.Name.Contains(filter3));


            classes = classes.Skip(query.Start).Take(query.Count);



            var markingPeriodClasses = Storage.MarkingPeriodClassStorage.GetAll();

            foreach (var cls in classes)
            {
                var clsDetails = new ClassDetails
                {
                    ChalkableDepartmentRef = cls.ChalkableDepartmentRef,
                    CourseRef = cls.CourseRef,
                    ClassNumber = cls.ClassNumber,
                    Description = cls.Description,
                    Id = cls.Id,
                    Name = cls.Name,
                    RoomRef = cls.RoomRef,
                    SchoolYearRef = cls.SchoolYearRef,
                    PrimaryTeacher = Storage.PersonStorage.GetById(DemoSchoolConstants.TeacherId),
                    PrimaryTeacherRef = DemoSchoolConstants.TeacherId,
                    StudentsCount = 10
                };


                if (clsDetails.PrimaryTeacherRef.HasValue)
                    clsDetails.PrimaryTeacher = Storage.PersonStorage.GetById(clsDetails.PrimaryTeacherRef.Value);
                clsDetails.MarkingPeriodClasses = markingPeriodClasses.Where(x => x.ClassRef == clsDetails.Id).ToList();
                classDetailsList.Add(clsDetails);
            }


            return new ClassQueryResult
            {
                Classes = classDetailsList,
                Query = query,
                SourceCount = data.Count
            };
        }

        public List<Class> GetAll(int? teacherRef)
        {
            var classes = GetClassesComplex(new ClassQuery()).Classes;

            if (teacherRef.HasValue)
                classes = classes.Where(x => x.PrimaryTeacherRef == teacherRef).ToList();
            return classes.Where(x => x.PrimaryTeacherRef == teacherRef).Select(x => (Class)x).ToList();
        }

        public IList<ClassDetails> GetClassesSortedByPeriod()
        {
            var classes = Storage.SchoolLocator.ClassService.GetClasses(Storage.Context.SchoolYearId, null, Storage.Context.PersonId).ToList();
            int? teacherId = null;
            int? studentId = null;
            if (Storage.Context.RoleId == CoreRoles.TEACHER_ROLE.Id)
                teacherId = Storage.Context.PersonId;
            else if (Storage.Context.RoleId == CoreRoles.STUDENT_ROLE.Id)
                studentId = Storage.Context.PersonId;
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
