using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolUserStorage:BaseDemoStorage<Guid, SchoolUser>
    {
        public DemoSchoolUserStorage(DemoStorage storage) : base(storage)
        {

        }


        

    }
}
