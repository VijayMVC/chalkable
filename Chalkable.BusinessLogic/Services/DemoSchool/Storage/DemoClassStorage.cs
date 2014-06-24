using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Microsoft.WindowsAzure.ServiceRuntime;

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
                    Description = cls.Description,
                    GradeLevelRef = cls.GradeLevelRef,
                    Id = cls.Id,
                    Name = cls.Name,
                    RoomRef = cls.RoomRef,
                    SchoolRef = cls.SchoolRef,
                    SchoolYearRef = cls.SchoolYearRef
                };


                if (clsDetails.PrimaryTeacherRef.HasValue)
                    clsDetails.PrimaryTeacher = Storage.PersonStorage.GetById(clsDetails.PrimaryTeacherRef.Value);
                clsDetails.GradeLevel = Storage.GradeLevelStorage.GetById(clsDetails.GradeLevelRef);
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

        public bool Exists(List<int> gradeLevelIds, int teacherId)
        {
            return GetAll(teacherId).Count(x => gradeLevelIds.Contains(x.GradeLevelRef)) > 0;
        }
    }
}
