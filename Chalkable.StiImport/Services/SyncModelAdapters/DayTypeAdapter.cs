using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class DayTypeAdapter : SyncModelAdapter<DayType>
    {
        public DayTypeAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<DayType> entities)
        {
            var dayTypes = entities.Select(x => new Data.School.Model.DayType
            {
                Id = x.DayTypeID,
                Name = x.Name,
                Number = x.Sequence,
                SchoolYearRef = x.AcadSessionID
            }).ToList();
            ServiceLocatorSchool.DayTypeService.Add(dayTypes);
        }

        protected override void UpdateInternal(IList<DayType> entities)
        {
            var dts = entities.Select(x => new Data.School.Model.DayType
            {
                Id = x.DayTypeID,
                Name = x.Name,
                Number = x.Sequence,
                SchoolYearRef = x.AcadSessionID
            }).ToList();
            ServiceLocatorSchool.DayTypeService.Edit(dts);
        }

        protected override void DeleteInternal(IList<DayType> entities)
        {
            var dayTypes = entities.Select(x => new Data.School.Model.DayType
            {
                Id = x.DayTypeID
            }).ToList();
            ServiceLocatorSchool.DayTypeService.Delete(dayTypes);
        }
    }
}