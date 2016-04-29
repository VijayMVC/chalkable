using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class DistrictAdapter : SyncModelAdapter<District>
    {
        public DistrictAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<District> entities)
        {
            //Not support insert 
        }

        protected override void UpdateInternal(IList<District> entities)
        {
            var districtId = ServiceLocatorMaster.Context.DistrictId;
            var entity = entities.FirstOrDefault(x => x.DistrictGUID == districtId);
            if(entity == null || !districtId.HasValue)
                return;
            var d = ServiceLocatorMaster.DistrictService.GetByIdOrNull(districtId.Value);
            d.Name = entity.Name;
            ServiceLocatorMaster.DistrictService.Update(d);
        }

        protected override void DeleteInternal(IList<District> entities)
        {
            //Not support delete
        }
    }
}
