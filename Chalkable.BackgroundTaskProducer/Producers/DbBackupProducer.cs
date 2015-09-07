﻿using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class DbBackupProducer : BaseProducer
    {
        public DbBackupProducer(string configSectionName)
            : base(configSectionName)
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = new DatabaseBackupRestoreTaskData(DateTime.UtcNow.Ticks, true);
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.BackupDatabases, DateTime.UtcNow, null, data.ToString(), BackgroundTask.GLOBAL_DOMAIN);
        }
    }
}