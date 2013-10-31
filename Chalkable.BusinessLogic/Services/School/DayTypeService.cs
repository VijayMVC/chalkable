using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDayTypeService
    {
        IList<DayType> GetSections(int schoolYearId);
        DayType GetSectionById(int id);
        bool CanDeleteSections(int schoolYearId);
        DayType Add(int id, int number, string name, int schoolYearId);
        DayType Edit(int id, int number, string name);
        void Delete(int id);
        
        //TODO: remove those methods 
        IList<DayType> GetSections(List<int> markingPeriodIds);
        bool CanGetSection(int schoolYearId);
        void ReBuildSections(List<string> sections, List<int> markingPeriodIds);
        void GenerateDefaultSections(int markingPeriodId);
        void GenerateScheduleSectionsWithDefaultPeriods(int markingPeriodId, string[] names);
    }

    public class DayTypeService : SchoolServiceBase, IDayTypeService
    {
        public DayTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public IList<DayType> GetSections(int schoolYearId)
        {
            using (var uow = Read())
            {
                return new DateTypeDataAccess(uow).GetDateTypes(schoolYearId);
            }
        }

        //TODO: remove this method
        public IList<DayType> GetSections(List<int> markingPeriodIds)
        {
            throw new NotImplementedException();
            //if(!CanGetSection(markingPeriodIds))
            //    throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_SECTIONS_NOT_EQUIVALENT);
            //using (var uow = Read())
            //{
            //    return new DateTypeDataAccess(uow).GetSections(markingPeriodIds);
            //}
        }

        public DayType GetSectionById(int id)
        {
            using (var uow = Read())
            {
                return new DateTypeDataAccess(uow).GetById(id);
            }
        }
        public bool CanDeleteSections(int schoolYearId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Read())
            {
                var cpDa = new ClassPeriodDataAccess(uow, Context.SchoolLocalId);
                return !cpDa.Exists(new ClassPeriodQuery{SchoolYearId = schoolYearId});
            }
        }

        //TODO: remove this methods
        public bool CanGetSection(int schoolYearId)
        {
            throw new NotImplementedException();
            //if (markingPeriodIds.Count > 1)
            //{ 
            //  var firstSections = GetSections(markingPeriodIds[0]);
            //  var firstMarkingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodIds[0]);
            //    for (var i = 1; i < markingPeriodIds.Count; i++)
            //    {
            //        var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodIds[i]);
            //        var sections = GetSections(markingPeriod.Id);
            //        if (markingPeriod.SchoolYearRef != firstMarkingPeriod.SchoolYearRef
            //            || markingPeriod.WeekDays != firstMarkingPeriod.WeekDays || sections.Count != firstSections.Count)
            //        {
            //            return false;
            //        }
            //        for (var j = 0; j < sections.Count; j++)
            //        {
            //            if (sections[j].Name != firstSections[j].Name
            //                || sections[j].Number != firstSections[j].Number)
            //                return false;
            //        }
            //    }
            //}
            //return true;
        }

        public DayType Add(int id, int number, string name, int schoolYearId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var ss = new DayType
                    {
                        Id = id,
                        Name = name,
                        Number = number,               
                    };
                var da = new DateTypeDataAccess(uow);
                var sections = da.GetDateTypes(schoolYearId);
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
                uow.Commit();
                return GetSectionById(ss.Id);
            }
        }

        public DayType Edit(int id, int number, string name)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new DateTypeDataAccess(uow);
                var section = da.GetById(id);
                var old = section.Number;
                var mn = Math.Min(old, number);
                var mx = Math.Max(old, number);
                var d = Math.Sign(old - number);

                IList<DayType> sections = da.GetDateTypes(section.SchoolYearRef)
                                             .Where(x => x.Id != section.Id).ToList();
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

        private IList<DayType> AdjustNumbering(IList<DayType> sections)
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


        public void Delete(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new DateTypeDataAccess(uow);
                var DayType = da.GetById(id);
                if (!CanDeleteSections(DayType.SchoolYearRef))
                    throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_CANT_DELETE);
                
                da.Delete(DayType);
                var sections = AdjustNumbering(da.GetDateTypes(DayType.SchoolYearRef));
                da.Update(sections);
                uow.Commit();
            }
        }

        //TODO: remove those methods later
       
        public void ReBuildSections(List<string> sections, List<int> markingPeriodIds)
        {
            throw new NotImplementedException();
            //if(!CanDeleteSections(markingPeriodIds))
            //    throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_CANT_DELETE);
            //if (!Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //if (markingPeriodIds.Count > 0)
            //{
            //    using (var uow = Update())
            //    {
            //        new DateTypeDataAccess(uow).ReBuildSection(markingPeriodIds, sections);
            //        uow.Commit();
            //    }

            //}
        }
        public void GenerateDefaultSections(int markingPeriodId)
        {

            throw new NotImplementedException();
            //var names = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            //var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            //var number = 0;
            //for (int i = 0; i < names.Length; i++)
            //{
            //    if ((mp.WeekDays & (1 << i)) != 0)
            //    {
            //        Add(number, names[i], markingPeriodId);
            //        number++;
            //    }
            //}
        }
        public void GenerateScheduleSectionsWithDefaultPeriods(int markingPeriodId, string[] names)
        {
            throw new NotImplementedException();
            //var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            //for (int i = 0; i < names.Length; i++)
            //{
            //    var section = Add(i, names[i], markingPeriod.Id);
            //    for (int start = 8 * 60; start < 18 * 60; start += 60)
            //        ServiceLocator.PeriodService.Add(markingPeriod.Id, start, start + 45, section.Id, i+1);
            //}
        }
    }
}
