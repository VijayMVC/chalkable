using System;
using System.Collections;
using System.Collections.Generic;
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
                    SchoolRef = 1
                },
                new GradingScale()
                {
                    Name = "Grades 4-5",
                    SchoolRef = 1
                },
                new GradingScale()
                {
                    Name = "Upper School",
                    SchoolRef = 1,
                    HomeGradeToDisplay = 0
                }
            });
        }
        
    }
}
