﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolGradeLevelStorage:BaseDemoIntStorage<SchoolGradeLevel>
    {
        public DemoSchoolGradeLevelStorage() : base(null, true)
        {
        }
        
        public IList<SchoolGradeLevel> GetAll(int? schoolId)
        {
            return data.Where(x => x.Value.SchoolRef == schoolId).Select(x => x.Value).ToList();
        }
    }
}
