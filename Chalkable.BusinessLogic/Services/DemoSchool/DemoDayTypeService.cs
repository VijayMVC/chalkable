using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoDayTypeService : DemoSchoolServiceBase, IDayTypeService
    {
        public DemoDayTypeService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }


        public IList<DayType> GetSections(int schoolYearId)
        {
            return Storage.DayTypeStorage.GetDateTypes(schoolYearId);
            
        }

        //TODO : filter by school 
        public DayType GetSectionById(int id)
        {
            return Storage.DayTypeStorage.GetById(id);
        }
        public bool CanDeleteSections(int schoolYearId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            return !Storage.ClassPeriodStorage.Exists(new ClassPeriodQuery { SchoolYearId = schoolYearId });
        }


        public DayType Add(int id, int number, string name, int schoolYearId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            var ss = new DayType
            {
                Id = id,
                Name = name,
                Number = number,
                SchoolYearRef = schoolYearId
            };
            var dayTypes = Storage.DayTypeStorage.GetDateTypes(schoolYearId);
            foreach (var scheduleSection in dayTypes)
            {
                if (scheduleSection.Number >= number)
                {
                    scheduleSection.Number++;
                }
            }
            Storage.DayTypeStorage.Add(ss);
            dayTypes = AdjustNumbering(dayTypes);
            Storage.DayTypeStorage.Update(dayTypes);
            return GetSectionById(ss.Id);
        }

        public IList<DayType> Add(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Storage.DayTypeStorage.Add(dayTypes);
        }

        public DayType Edit(int id, int number, string name)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            var section = Storage.DayTypeStorage.GetById(id);
            var old = section.Number;
            var mn = Math.Min(old, number);
            var mx = Math.Max(old, number);
            var d = Math.Sign(old - number);

            IList<DayType> sections = Storage.DayTypeStorage.GetDateTypes(section.SchoolYearRef, null, null)
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
            Storage.DayTypeStorage.Update(sections);
            return GetSectionById(section.Id);
        }

        public IList<DayType> Edit(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            return Storage.DayTypeStorage.Update(dayTypes);
        }

        private IList<DayType> AdjustNumbering(IList<DayType> sections)
        {
            sections = sections.OrderBy(x=>x.Number).ToList();
            var i = 0;
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
            var dateType = Storage.DayTypeStorage.GetById(id);
            if (!CanDeleteSections(dateType.SchoolYearRef))
                throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_CANT_DELETE);

            Storage.DayTypeStorage.Delete(dateType);
            var sections = AdjustNumbering(Storage.DayTypeStorage.GetDateTypes(dateType.SchoolYearRef));
            Storage.DayTypeStorage.Update(sections);
        }

        public void Delete(IList<int> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }
    }
}
