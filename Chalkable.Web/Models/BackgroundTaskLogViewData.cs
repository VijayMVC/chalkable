using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;

namespace Chalkable.Web.Models
{
    public class BackgroundTaskLogViewData
    {
        public Guid Id { get; set; }
        public int Level { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }

        public static BackgroundTaskLogViewData Create(BackgroundTaskService.BackgroundTaskLogItem log)
        {
            return new BackgroundTaskLogViewData
                       {
                           Id = log.Id,
                           Level = log.Level,
                           Message = log.Message,
                           TimeStamp = log.Time
                       };
        }
        public static IList<BackgroundTaskLogViewData> Create(IList<BackgroundTaskService.BackgroundTaskLogItem> logs)
        {
            return logs.Select(Create).ToList();
        } 
    }
}