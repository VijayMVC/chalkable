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

        void AddSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations);
        void EditSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations);
        void DeleteSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations);
        IList<SectionTimeSlotVariation> GetAllSectionTimeSlotVariations();

        void AddScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations);
        void EditScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations);
        void DeleteScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations);
        IList<ScheduledTimeSlotVariation> GetAllScheduledTimeSlotVariations();
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


        public void AddSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations)
        {
            DoUpdate(u => new SectionTimeSlotVariationDataAccess(u, null).Insert(sectionTimeSlotVariations));
        }

        public void EditSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations)
        {
            DoUpdate(u => new SectionTimeSlotVariationDataAccess(u, null).Update(sectionTimeSlotVariations));
        }

        public void DeleteSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations)
        {
            DoUpdate(u => new SectionTimeSlotVariationDataAccess(u, null).Delete(sectionTimeSlotVariations));
        }

        public IList<SectionTimeSlotVariation> GetAllSectionTimeSlotVariations()
        {
            return DoRead(u => new SectionTimeSlotVariationDataAccess(u, null).GetAll());
        }


        public void AddScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations)
        {
            DoUpdate(u => new ScheduledTimeSlotVariationDataAccess(u, null).Insert(scheduledTimeSlotVariations));
        }

        public void EditScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations)
        {
            DoUpdate(u => new ScheduledTimeSlotVariationDataAccess(u, null).Update(scheduledTimeSlotVariations));
        }

        public void DeleteScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations)
        {
            DoUpdate(u => new ScheduledTimeSlotVariationDataAccess(u, null).Delete(scheduledTimeSlotVariations));
        }

        public IList<ScheduledTimeSlotVariation> GetAllScheduledTimeSlotVariations()
        {
            return DoRead(u => new ScheduledTimeSlotVariationDataAccess(u, null).GetAll());
        }
    }
}