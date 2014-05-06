using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoChalkableDepartmentStorage:BaseDemoStorage<Guid, ChalkableDepartment>
    {
        public DemoChalkableDepartmentStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(ChalkableDepartment res)
        {
            if (!data.ContainsKey(res.Id))
                data[res.Id] = res;
        }

        public void Update(ChalkableDepartment res)
        {
            if (data.ContainsKey(res.Id))
                data[res.Id] = res;
        }

        public ChalkableDepartment GetByIdOrNull(Guid id)
        {
            return data.ContainsKey(id) ? data[id] : null;
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
