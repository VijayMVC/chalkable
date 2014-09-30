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
            using (var uow = Update())
            {
                var da = new ScheduledTimeSlotDataAccess(uow, null);
                da.Insert(scheduledTimeSlots);
                uow.Commit();
            }
        }

        public void Edit(IList<ScheduledTimeSlot> scheduledTimeSlots)
        {
            using (var uow = Update())
            {
                var da = new ScheduledTimeSlotDataAccess(uow, null);
                da.Update(scheduledTimeSlots);
                uow.Commit();
            }
        }

        public void Delete(IList<ScheduledTimeSlot> scheduledTimeSlots)
        {
            using (var uow = Update())
            {
                var da = new ScheduledTimeSlotDataAccess(uow, null);
                da.Delete(scheduledTimeSlots);
                uow.Commit();
            }
        }

        public IList<ScheduledTimeSlot> GetAll()
        {
            using (var uow = Read())
            {
                var da = new ScheduledTimeSlotDataAccess(uow, null);
                return da.GetAll();
            }
        }
    }
}