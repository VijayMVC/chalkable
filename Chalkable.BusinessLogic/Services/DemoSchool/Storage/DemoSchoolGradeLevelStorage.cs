﻿using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolGradeLevelStorage:BaseDemoStorage<int, SchoolGradeLevel>
    {
        private int index = 0;
        public DemoSchoolGradeLevelStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(SchoolGradeLevel schoolGradeLevel)
        {
            data.Add(index++, schoolGradeLevel);
        }
        
    }
}
