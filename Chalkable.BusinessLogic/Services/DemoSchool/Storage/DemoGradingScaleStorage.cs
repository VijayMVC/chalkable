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

        public override void Setup()
        {
            Add(new List<GradingScale>
            {
                new GradingScale()
                {
                    Name = "K-3 Grade Scale",
                    SchoolRef = DemoSchoolConstants.SchoolId
                },
                new GradingScale()
                {
                    Name = "Grades 4-5",
                    SchoolRef = DemoSchoolConstants.SchoolId
                },
                new GradingScale()
                {
                    Name = "Upper School",
                    SchoolRef = DemoSchoolConstants.SchoolId,
                    HomeGradeToDisplay = 0
                }
            });
        }
        
    }
}
