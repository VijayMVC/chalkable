using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassStorage:BaseDemoStorage<int , Class>
    {
        public DemoClassStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(Class cClass)
        {
            if (!data.ContainsKey(cClass.Id))
                data[cClass.Id] = cClass;
        }

        public void Add(IList<Class> classes)
        {
            foreach (var cls in classes)
            {
                Add(cls);
            }
        }

        public void Update(Class cClass)
        {
            if (data.ContainsKey(cClass.Id))
                data[cClass.Id] = cClass;
        }

        public void Update(IList<Class> classes)
        {
            foreach (var cls in classes)
            {
                Update(cls);
            }
        }

        public override void Setup()
        {
            var classes = new List<Class>
            {
                new Class
                {
                    Id = 1,
                    Name = "Geometry",
                    Description = "Geometry",
                    GradeLevelRef = 12,
                    CourseRef = 43,
                    ChalkableDepartmentRef = null,
                    TeacherRef = 1195,
                    SchoolRef = 1,
                    SchoolYearRef = 12
                },
                new Class
                {
                    Id = 2,
                    Name = "Algebra",
                    Description = "Algebra",
                    GradeLevelRef = 12,
                    CourseRef = 43,
                    ChalkableDepartmentRef = null,
                    TeacherRef = 1195,
                    SchoolRef = 1,
                    SchoolYearRef = 12
                }
            };


            Add(classes);
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
                classes =
                    classes.Where(
                        x =>
                            x.Id == query.ClassId &&
                            Storage.MarkingPeriodClassStorage.Exists(query.ClassId, query.MarkingPeriodId));

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

            //todo: ask about caller role id

            //var roleId = Storage.SchoolPersonStorage.GetRoleId(personId, SchoolRef);

/*

              public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CourseRef { get; set; }
        public int? SchoolYearRef { get; set; }
        public Guid? ChalkableDepartmentRef { get; set; }
        public int? TeacherRef { get; set; }
        public int GradeLevelRef { get; set; }
        public int? RoomRef { get; set; }
        public int? SchoolRef { get; set; }

      */
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


                if (clsDetails.TeacherRef.HasValue)
                    clsDetails.Teacher = Storage.PersonStorage.GetById(clsDetails.TeacherRef.Value);
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

        public List<Class> GetAll(int teacherRef)
        {
            return GetClassesComplex(new ClassQuery()).Classes.Where(x => x.TeacherRef == teacherRef).Select(x => (Class) x).ToList();
        }

        public bool Exists(List<int> gradeLevelIds, int teacherId)
        {
            return GetAll(teacherId).Count(x => gradeLevelIds.Contains(x.GradeLevelRef)) > 0;
        }
    }
}
