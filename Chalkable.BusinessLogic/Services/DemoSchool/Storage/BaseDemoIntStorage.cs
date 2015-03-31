using System;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{

    public abstract class BaseDemoIntStorage<TValue>: BaseDemoStorage<int, TValue> where TValue: new()
    {
        protected BaseDemoIntStorage(Func<TValue, int> keyField, bool autoIncrement = false) : base(keyField, autoIncrement)
        {
            Index = 1;
        }

        public override int GetNextFreeId()
        {
            return Index++;
        }

        
    }
}
