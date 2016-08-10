using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class EthnicityAdapter : SyncModelAdapter<Ethnicity>
    {
        public EthnicityAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Ethnicity Selector(Ethnicity model) => new Data.School.Model.Ethnicity
        {
            Id = model.EthnicityID,
            Name = model.Name,
            Description = model.Description,
            Code = model.Code,
            IsActive = model.IsActive,
            IsSystem = model.IsSystem,
            NCESCode = model.NCESCode,
            SIFCode = model.SIFCode,
            StateCode = model.StateCode
        };

        protected override void InsertInternal(IList<Ethnicity> entities)
        {
            ServiceLocatorSchool.EthnicityService.Add(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<Ethnicity> entities)
        {
            ServiceLocatorSchool.EthnicityService.Edit(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<Ethnicity> entities)
        {
            ServiceLocatorSchool.EthnicityService.Delete(entities.Select(Selector).ToList());
        }
    }
}
