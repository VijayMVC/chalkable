using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ScheduledTimeSlotVariationDataAccess : BaseSchoolDataAccess<ScheduledTimeSlotVariation>
    {
        public ScheduledTimeSlotVariationDataAccess(UnitOfWork unitOfWork, int? localSchoolId) : base(unitOfWork, localSchoolId)
        {
        }

        public void Delete(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations)
        {
            SimpleDelete(scheduledTimeSlotVariations);
        }
    }
}