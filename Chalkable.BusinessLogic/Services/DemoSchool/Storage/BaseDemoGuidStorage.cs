using System;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{

    public abstract class BaseDemoGuidStorage<TValue>: BaseDemoStorage<Guid, TValue> where TValue: new()
    {
        protected BaseDemoGuidStorage(Func<TValue, Guid> keyField, bool autoIncrement = false)
            : base(keyField, autoIncrement)
        {
        }

        public override Guid GetNextFreeId()
        {
            return Guid.NewGuid();
        }
    }
}
