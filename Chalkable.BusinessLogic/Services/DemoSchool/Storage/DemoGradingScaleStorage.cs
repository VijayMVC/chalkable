using System;
using System.Collections;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradingScaleStorage: BaseDemoIntStorage<GradingScale>
    {
        public DemoGradingScaleStorage(DemoStorage storage) : base(storage, x => x.Id, true)
        {
        } 
    }
}
