using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityDate
    {
        /// <summary>
        /// Id of the activity
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// Date of the activity
        /// </summary>
        public DateTime Date { get; set; }

        public static IList<ActivityDate> Create(IList<Pair<int, DateTime>> activityDates)
        {
            return activityDates.Select(x => new ActivityDate
            {
                ActivityId = x.First,
                Date = x.Second
            }).ToList();
        } 
    }
}
