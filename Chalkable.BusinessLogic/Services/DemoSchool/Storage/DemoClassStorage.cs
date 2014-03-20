using System.Collections.Generic;
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
            throw new System.NotImplementedException();
        }

        public void Add(IList<Class> cClass)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Class cClass)
        {
            throw new System.NotImplementedException();
        }

        public void Update(IList<Class> cClass)
        {
            throw new System.NotImplementedException();
        }
    }
}
