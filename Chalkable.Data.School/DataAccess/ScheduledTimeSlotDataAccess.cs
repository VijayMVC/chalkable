using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ScheduledTimeSlotDataAccess : BaseSchoolDataAccess<ScheduledTimeSlot>
    {
        public ScheduledTimeSlotDataAccess(UnitOfWork unitOfWork, int? localSchoolId) : base(unitOfWork, localSchoolId)
        {
        }

        public void Delete(IList<ScheduledTimeSlot> scheduledTimeSlots)
        {
            SimpleDelete(scheduledTimeSlots);
        }
    }
}