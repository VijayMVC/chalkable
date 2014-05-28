using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassPersonStorage:BaseDemoIntStorage<ClassPerson>
    {
        public DemoClassPersonStorage(DemoStorage storage) : base(storage, null, true)
        {
        }

        public void Delete(ClassPersonQuery classPersonQuery)
        {
            var classPersons = GetClassPersons(classPersonQuery);
            
            foreach (var classPerson in classPersons)
            {
                var item = data.First(x => x.Value == classPerson);
                data.Remove(item.Key);
            }
        }

        public ClassPerson GetClassPerson(ClassPersonQuery classPersonQuery)
        {
            var classPersons = GetClassPersons(classPersonQuery);
            return classPersons.First();
        }

        public bool Exists(ClassPersonQuery classPersonQuery)
        {
            var classPersons = GetClassPersons(classPersonQuery);
            var classPersonsList = classPersons as IList<ClassPerson> ?? classPersons.ToList();

            return (classPersonQuery.ClassId.HasValue || classPersonQuery.MarkingPeriodId.HasValue ||
                    classPersonQuery.PersonId.HasValue) && classPersonsList.Count > 0;
        }

        public IEnumerable<ClassPerson> GetClassPersons(ClassPersonQuery classPersonQuery)
        {
            var classPersons = data.Select(x => x.Value);

            if (classPersonQuery.ClassId.HasValue)
                classPersons = classPersons.Where(x => x.ClassRef == classPersonQuery.ClassId);
            if (classPersonQuery.MarkingPeriodId.HasValue)
                classPersons = classPersons.Where(x => x.MarkingPeriodRef == classPersonQuery.MarkingPeriodId);
            if (classPersonQuery.PersonId.HasValue)
                classPersons = classPersons.Where(x => x.PersonRef == classPersonQuery.PersonId);
            return classPersons;
        }

        public override void Setup()
        {
            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.AlgebraClassId,
                MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                PersonRef = DemoSchoolConstants.FirstStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.AlgebraClassId,
                MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                PersonRef = DemoSchoolConstants.FirstStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.GeometryClassId,
                MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                PersonRef = DemoSchoolConstants.FirstStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.GeometryClassId,
                MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                PersonRef = DemoSchoolConstants.FirstStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.AlgebraClassId,
                MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                PersonRef = DemoSchoolConstants.SecondStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.AlgebraClassId,
                MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                PersonRef = DemoSchoolConstants.SecondStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.GeometryClassId,
                MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                PersonRef = DemoSchoolConstants.SecondStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.GeometryClassId,
                MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                PersonRef = DemoSchoolConstants.SecondStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.AlgebraClassId,
                MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                PersonRef = DemoSchoolConstants.ThirdStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.AlgebraClassId,
                MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                PersonRef = DemoSchoolConstants.ThirdStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.GeometryClassId,
                MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                PersonRef = DemoSchoolConstants.ThirdStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });

            Add(new ClassPerson
            {
                ClassRef = DemoSchoolConstants.GeometryClassId,
                MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                PersonRef = DemoSchoolConstants.ThirdStudentId,
                SchoolRef = DemoSchoolConstants.SchoolId
            });
        }
    }
}
