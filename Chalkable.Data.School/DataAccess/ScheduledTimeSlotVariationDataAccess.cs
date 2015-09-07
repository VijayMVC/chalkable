using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Data.School.DataAccess
{
    public class ScheduledTimeSlotVariationDataAccess : DataAccessBase<ScheduledTimeSlotVariation>
    {
        public ScheduledTimeSlotVariationDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}