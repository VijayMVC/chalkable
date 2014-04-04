using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoUserStorage:BaseDemoStorage<Guid, User>
    {
        public DemoUserStorage(DemoStorage storage) : base(storage)
        {
            //get users 1 teacher 1 admin 1 student
        }
    }
}
