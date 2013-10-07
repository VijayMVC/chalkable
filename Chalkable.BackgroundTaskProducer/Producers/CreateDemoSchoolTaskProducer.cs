﻿using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class CreateDemoSchoolTaskProducer : BaseProducer
    {
        private const string CONFIG_SECTION_NAME = "CreateDemoSchoolTaskProducer";
        public CreateDemoSchoolTaskProducer() : base(CONFIG_SECTION_NAME)
        {
        }

        protected  override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var existing = sl.BackgroundTaskService.Find(null, BackgroundTaskStateEnum.Created, BackgroundTaskTypeEnum.CreateDemoSchool);
            if (existing == null)
            {
                var have = sl.SchoolService.GetSchools(false, true, false).Count;
                int need = Settings.Configuration.DemoSchoolsReserved;
                int cnt = Math.Max(0, need - have);
                if (cnt > 0)
                    sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.CreateDemoSchool, currentTimeUtc, null, string.Empty);
            }
        }
    }
}