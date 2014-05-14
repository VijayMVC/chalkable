using System;
using System.Collections;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradingScaleRangeStorage: BaseDemoIntStorage<GradingScaleRange>
    {
        public DemoGradingScaleRangeStorage(DemoStorage storage)
            : base(storage, x => x.GradingScaleRef, true)
        {
        }

        public override void Setup()
        {

        }

        public void Edit(IList<GradingScaleRange> gradingScaleRanges)
        {
            Update(gradingScaleRanges);
        }
    }
}
