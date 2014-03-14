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
        IList<DayType> Add(IList<DayType> dayTypes); 
        DayType Edit(int id, int number, string name);
        IList<DayType> Edit(IList<DayType> dayTypes);
        void Delete(int id);
        void Delete(IList<int> ids);
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
                return new DayTypeDataAccess(uow).GetDateTypes(schoolYearId);
            }
        }

        //TODO : filter by school 
        public DayType GetSectionById(int id)
        {
            using (var uow = Read())
            {
                return new DayTypeDataAccess(uow).GetById(id);
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
                        SchoolYearRef = schoolYearId
                    };
                var da = new DayTypeDataAccess(uow);
                var dayTypes = da.GetDateTypes(schoolYearId);
                foreach (var scheduleSection in dayTypes)
                {
                    if (scheduleSection.Number >= number)
                    {
                        scheduleSection.Number++;
                    }
                }
                da.Insert(ss);
                dayTypes.Add(ss);
                dayTypes = AdjustNumbering(dayTypes);
                da.Update(dayTypes);
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
                var da = new DayTypeDataAccess(uow);
                var section = da.GetById(id);
                var old = section.Number;
                var mn = Math.Min(old, number);
                var mx = Math.Max(old, number);
                var d = Math.Sign(old - number);

                IList<DayType> sections = da.GetDateTypes(section.SchoolYearRef, null, null)
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
                var da = new DayTypeDataAccess(uow);
                var dateType = da.GetById(id);
                if (!CanDeleteSections(dateType.SchoolYearRef))
                    throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_CANT_DELETE);
                
                da.Delete(dateType);
                var sections = AdjustNumbering(da.GetDateTypes(dateType.SchoolYearRef));
                da.Update(sections);
                uow.Commit();
            }
        }


        public IList<DayType> Add(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            //TODO: adjust numbering
            using (var uow = Update())
            {
                new DayTypeDataAccess(uow).Insert(dayTypes);
                uow.Commit();
                return dayTypes;
            }
             
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new DayTypeDataAccess(uow);
                var dayTypes = da.GetDateTypes(ids);
                var syIds = dayTypes.GroupBy(x => x.SchoolYearRef).Select(x=>x.Key).ToList();
                da.Delete(ids);
                var dayTypesForUpdate = new List<DayType>();
                foreach (var syId in syIds)
                {
                    dayTypesForUpdate.AddRange(AdjustNumbering(da.GetDateTypes(syId)));
                }
                da.Update(dayTypesForUpdate);
                uow.Commit();
            }
        }


        public IList<DayType> Edit(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            //TODO: adjust numbering
            using (var uow = Update())
            {
                new DayTypeDataAccess(uow).Update(dayTypes);
                uow.Commit();
                return dayTypes;
            }
        }
    }
}
