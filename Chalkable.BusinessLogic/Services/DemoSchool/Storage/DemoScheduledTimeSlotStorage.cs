using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoScheduledTimeSlotStorage : BaseDemoIntStorage<ScheduledTimeSlot>
    {
        public DemoScheduledTimeSlotStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }
    }
}
