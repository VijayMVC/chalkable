using System;
using System.Collections.Generic;
using System.Linq;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{

    public abstract class BaseDemoIntStorage<TValue>: BaseDemoStorage<int, TValue> where TValue: new()
    {
        protected BaseDemoIntStorage(DemoStorage storage, Func<TValue, int> keyField, bool autoIncrement = false) : base(storage, keyField, autoIncrement)
        {
            Index = 1;
        }

        public override int GetNextFreeId()
        {
            return Index++;
        }

        
    }
}
