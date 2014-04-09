using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassPersonStorage:BaseDemoStorage<int, ClassPerson>
    {
        public DemoClassPersonStorage(DemoStorage storage) : base(storage)
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

        public void Add(ClassPerson classPerson)
        {
            data.Add(GetNextFreeId(), classPerson);
        }

        public void Add(IList<ClassPerson> classPersons)
        {
            foreach (var classPerson in classPersons)
            {
                Add(classPerson);
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

        public void Setup()
        {
            Add(new ClassPerson
            {
                ClassRef = 1,
                MarkingPeriodRef = 1,
                PersonRef = 1196,
                SchoolRef = 1
            });

            Add(new ClassPerson
            {
                ClassRef = 1,
                MarkingPeriodRef = 2,
                PersonRef = 1196,
                SchoolRef = 1
            });

            Add(new ClassPerson
            {
                ClassRef = 2,
                MarkingPeriodRef = 1,
                PersonRef = 1196,
                SchoolRef = 1
            });

            Add(new ClassPerson
            {
                ClassRef = 2,
                MarkingPeriodRef = 2,
                PersonRef = 1196,
                SchoolRef = 1
            });
        }
    }
}
