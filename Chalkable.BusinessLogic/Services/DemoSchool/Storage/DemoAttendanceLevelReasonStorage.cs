﻿using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAttendanceLevelReasonStorage:BaseDemoStorage<int , AttendanceLevelReason>
    {
        public DemoAttendanceLevelReasonStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            throw new NotImplementedException();
        }

        public void Update(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            throw new NotImplementedException();
        }
    }
}
