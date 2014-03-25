using System.Collections.Generic;
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
            throw new System.NotImplementedException();
        }

        public ClassDetails Add(ClassPerson classPerson)
        {
            throw new System.NotImplementedException();
        }

        public void Add(IList<ClassPerson> classPersons)
        {
            throw new System.NotImplementedException();
        }

        public ClassPerson GetClassPerson(ClassPersonQuery classPersonQuery)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(ClassPersonQuery classPersonQuery)
        {
            throw new System.NotImplementedException();
        }
    }
}
