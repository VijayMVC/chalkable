using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Logic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IScheduleSectionService
    {
        IList<ScheduleSection> GetSections(Guid markingPeriodId);
        IList<ScheduleSection> GetSections(List<Guid> markingPeriodIds);
        ScheduleSection GetSectionById(Guid id);

        bool CanDeleteSections(IList<Guid> markingPeriodIds);
        bool CanGetSection(IList<Guid> markingPeriodIds);

        ScheduleSection Add(int number, string name, Guid markingPeriodId, int? sisId = null);
        ScheduleSection Edit(Guid id, int number, string name);
        void Delete(Guid id);
        void ReBuildSections(List<string> sections, List<Guid> markingPeriodIds);
        void GenerateDefaultSections(Guid markingPeriodId);
        void GenerateScheduleSectionsWithDefaultPeriods(Guid markingPeriodId, string[] names);
    }

    public class ScheduleSectionService : SchoolServiceBase, IScheduleSectionService
    {
        public ScheduleSectionService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public IList<ScheduleSection> GetSections(Guid markingPeriodId)
        {
            using (var uow = Read())
            {
                return new ScheduleSectionDataAccess(uow).GetSections(markingPeriodId, null, null);
            }
        }

        public IList<ScheduleSection> GetSections(List<Guid> markingPeriodIds)
        {
            if(!CanGetSection(markingPeriodIds))
                throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_SECTIONS_NOT_EQUIVALENT);
            using (var uow = Read())
            {
                return new ScheduleSectionDataAccess(uow).GetSections(markingPeriodIds);
            }
        }

        public ScheduleSection GetSectionById(Guid id)
        {
            using (var uow = Read())
            {
                return new ScheduleSectionDataAccess(uow).GetById(id);
            }
        }
        public bool CanDeleteSections(IList<Guid> markingPeriodIds)
        {
            if (markingPeriodIds != null && markingPeriodIds.Count > 0)
            {
                if (!BaseSecurity.IsAdminEditor(Context))
                    throw new ChalkableSecurityException();

                using (var uow = Read())
                {
                    var cpDa = new ClassPeriodDataAccess(uow);
                    return CanGetSection(markingPeriodIds) && !cpDa.Exists(markingPeriodIds);
                }
            }
            return false;
        }

        //TODO: move it to stored procedure
        public bool CanGetSection(IList<Guid> markingPeriodIds)
        {
            if (markingPeriodIds == null || markingPeriodIds.Count == 0)
                return false;

            if (markingPeriodIds.Count > 1)
            { 
              var firstSections = GetSections(markingPeriodIds[0]);
              var firstMarkingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodIds[0]);
                for (var i = 1; i < markingPeriodIds.Count; i++)
                {
                    var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodIds[i]);
                    var sections = GetSections(markingPeriod.Id);
                    if (markingPeriod.SchoolYearRef != firstMarkingPeriod.SchoolYearRef
                        || markingPeriod.WeekDays != firstMarkingPeriod.WeekDays || sections.Count != firstSections.Count)
                    {
                        return false;
                    }
                    for (var j = 0; j < sections.Count; j++)
                    {
                        if (sections[j].Name != firstSections[j].Name
                            || sections[j].Number != firstSections[j].Number)
                            return false;
                    }
                }
            }
            return true;
        }

        public ScheduleSection Add(int number, string name, Guid markingPeriodId, int? sisId = null)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                IStateMachine machine = new SchoolStateMachine(Context.SchoolId.Value, ServiceLocator.ServiceLocatorMaster);
                if (!machine.CanApply(StateActionEnum.SectionsAdd))
                    throw new InvalidSchoolStatusException(ChlkResources.ERR_SCHEDULE_SECTION_INVALID_STATUS);

                var ss = new ScheduleSection
                    {
                        Id = Guid.NewGuid(),
                        MarkingPeriodRef = markingPeriodId,
                        Name = name,
                        Number = number,
                        SisId = sisId
                    };
                var da = new ScheduleSectionDataAccess(uow);
                var sections = da.GetSections(markingPeriodId, null, null);
                foreach (var scheduleSection in sections)
                {
                    if (scheduleSection.Number >= number)
                    {
                        scheduleSection.Number++;
                    }
                }
                da.Insert(ss);
                sections.Add(ss);
                sections = AdjustNumbering(sections);
                da.Update(sections);
                machine.Apply(StateActionEnum.SectionsAdd);
                uow.Commit();
                return GetSectionById(ss.Id);
            }
        }

        public ScheduleSection Edit(Guid id, int number, string name)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ScheduleSectionDataAccess(uow);
                var section = da.GetById(id);
                var old = section.Number;
                var mn = Math.Min(old, number);
                var mx = Math.Max(old, number);
                var d = Math.Sign(old - number);

                IList<ScheduleSection> sections = da.GetSections(section.MarkingPeriodRef, null, null).Where(x => x.Id != section.Id).ToList();
                foreach (var scheduleSection in sections)
                {
                    if (scheduleSection.Number >= mn && scheduleSection.Number <= mx)
                    {
                        scheduleSection.Number += d;   
                    }
                }
                section.Name = name;
                section.Number = number;
                sections.Add(section);
                sections = AdjustNumbering(sections);
                da.Update(sections);
                uow.Commit();
                return GetSectionById(section.Id);
            }
        }

        private IList<ScheduleSection> AdjustNumbering(IList<ScheduleSection> sections)
        {
            sections = sections.OrderBy(x=>x.Number).ToList();
            int i = 0;
            foreach (var scheduleSection in sections)
            {
                scheduleSection.Number = i;
                i++;
            }
            return sections;
        }  

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new ScheduleSectionDataAccess(uow);
                var ss = da.GetById(id);
                if (!CanDeleteSections(new List<Guid> { ss.MarkingPeriodRef }))
                    throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_CANT_DELETE);
                
                da.Delete(ss);
                var sections = AdjustNumbering(da.GetSections(ss.MarkingPeriodRef, null, null));
                da.Update(sections);
                uow.Commit();
            }
        }

        public void ReBuildSections(List<string> sections, List<Guid> markingPeriodIds)
        {
            if(!CanDeleteSections(markingPeriodIds))
                throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_CANT_DELETE);
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            if (markingPeriodIds.Count > 0)
            {
                using (var uow = Update())
                {
                    var machine = new SchoolStateMachine(Context.SchoolId.Value, ServiceLocator.ServiceLocatorMaster);
                    if (!machine.CanApply(StateActionEnum.SectionsAdd))
                        throw new InvalidSchoolStatusException(ChlkResources.ERR_SCHEDULE_SECTION_INVALID_STATUS);

                    new ScheduleSectionDataAccess(uow).ReBuildSection(markingPeriodIds, sections);
                    machine.Apply(StateActionEnum.SectionsAdd);
                    uow.Commit();
                }

            }
        }

        
        public void GenerateDefaultSections(Guid markingPeriodId)
        {
            var names = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            var number = 0;
            for (int i = 0; i < names.Length; i++)
            {
                if ((mp.WeekDays & (1 << i)) != 0)
                {
                    Add(number, names[i], markingPeriodId);
                    number++;
                }
            }
        }
        public void GenerateScheduleSectionsWithDefaultPeriods(Guid markingPeriodId, string[] names)
        {
            var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            for (int i = 0; i < names.Length; i++)
            {
                var section = Add(i, names[i], markingPeriod.Id);
                for (int start = 8 * 60; start < 18 * 60; start += 60)
                    ServiceLocator.PeriodService.Add(markingPeriod.Id, start, start + 45, section.Id, i+1);
            }
        }


    }
}
