using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
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
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ScheduledTimeSlot>(u).Insert(scheduledTimeSlots));
        }

        public void Edit(IList<ScheduledTimeSlot> scheduledTimeSlots)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ScheduledTimeSlot>(u).Update(scheduledTimeSlots));
        }

        public void Delete(IList<ScheduledTimeSlot> scheduledTimeSlots)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ScheduledTimeSlot>(u).Delete(scheduledTimeSlots));
        }

        public IList<ScheduledTimeSlot> GetAll()
        {
            return DoRead(u => new DataAccessBase<ScheduledTimeSlot>(u).GetAll());
        }


        public void AddSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<SectionTimeSlotVariation>(u).Insert(sectionTimeSlotVariations));
        }

        public void EditSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<SectionTimeSlotVariation>(u).Update(sectionTimeSlotVariations));
        }

        public void DeleteSectionTimeSlotVariations(IList<SectionTimeSlotVariation> sectionTimeSlotVariations)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<SectionTimeSlotVariation>(u).Delete(sectionTimeSlotVariations));
        }

        public IList<SectionTimeSlotVariation> GetAllSectionTimeSlotVariations()
        {
            return DoRead(u => new DataAccessBase<SectionTimeSlotVariation>(u).GetAll());
        }


        public void AddScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations)
        {
            DoUpdate(u => new ScheduledTimeSlotVariationDataAccess(u).Insert(scheduledTimeSlotVariations));
        }

        public void EditScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations)
        {
            DoUpdate(u => new ScheduledTimeSlotVariationDataAccess(u).Update(scheduledTimeSlotVariations));
        }

        public void DeleteScheduledTimeSlotVariations(IList<ScheduledTimeSlotVariation> scheduledTimeSlotVariations)
        {
            DoUpdate(u => new ScheduledTimeSlotVariationDataAccess(u).Delete(scheduledTimeSlotVariations));
        }

        public IList<ScheduledTimeSlotVariation> GetAllScheduledTimeSlotVariations()
        {
            return DoRead(u => new ScheduledTimeSlotVariationDataAccess(u).GetAll());
        }
    }
}