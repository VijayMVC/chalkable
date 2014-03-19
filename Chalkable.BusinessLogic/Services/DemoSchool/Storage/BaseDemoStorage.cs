using System.Diagnostics;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class BaseDemoStorage
    {
        protected DemoStorage Storage { get; private set; }
        public BaseDemoStorage(DemoStorage storage)
        {
            Storage = storage;
        }

        public void Init()
        {
            //some init data
        }


    }
}
