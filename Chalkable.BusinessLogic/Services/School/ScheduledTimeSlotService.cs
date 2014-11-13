using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IScheduledTimeSlotService
    {
        void Add(IList<ScheduledTimeSlot> scheduledTimeSlots);
        void Edit(IList<ScheduledTimeSlot> scheduledTimeSlots);
        void Delete(IList<ScheduledTimeSlot> scheduledTimeSlots);
        IList<ScheduledTimeSlot> GetAll();
    }

    public class ScheduledTimeSlotService : SchoolServiceBase, IScheduledTimeSlotService
    {
        public ScheduledTimeSlotService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<ScheduledTimeSlot> scheduledTimeSlots)
        {
            DoUpdate(u => new ScheduledTimeSlotDataAccess(u, null).Insert(scheduledTimeSlots));
        }

        public void Edit(IList<ScheduledTimeSlot> scheduledTimeSlots)
        {
            DoUpdate(u => new ScheduledTimeSlotDataAccess(u, null).Update(scheduledTimeSlots));
        }

        public void Delete(IList<ScheduledTimeSlot> scheduledTimeSlots)
        {
            DoUpdate(u => new ScheduledTimeSlotDataAccess(u, null).Delete(scheduledTimeSlots));
        }

        public IList<ScheduledTimeSlot> GetAll()
        {
            return DoRead(u => new ScheduledTimeSlotDataAccess(u, null).GetAll());
        }
    }
}