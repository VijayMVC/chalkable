﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.Master.Model
{
    public class BackgroundTask
    {
        public Guid? SchoolRef { get; set; }
        public Guid Id { get; set; }
        public BackgroundTaskTypeEnum Type { get; set; }
        public BackgroundTaskStateEnum State { get; set; }
        public DateTime Scheduled { get; set; }
        public DateTime Created { get; set; }
        public string Data { get; set; }
        public DateTime? Started { get; set; }
    }

    public enum BackgroundTaskTypeEnum
    {
        CreateEmptySchool = 0
    }

    public enum BackgroundTaskStateEnum
    {
        Created = 0,
        Processing = 1,
        Processed = 2,
        Canceled = 3,
        Failed = 4
    }
}
