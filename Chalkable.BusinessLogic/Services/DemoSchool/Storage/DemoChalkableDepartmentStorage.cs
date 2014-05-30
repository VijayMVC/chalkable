using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoChalkableDepartmentStorage:BaseDemoGuidStorage<ChalkableDepartment>
    {
        public DemoChalkableDepartmentStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }
     
        public ChalkableDepartment GetByIdOrNull(Guid id)
        {
            return data.ContainsKey(id) ? data[id] : null;
        }

        public override void Setup()
        {
        }
    }
}
