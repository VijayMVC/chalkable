using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class LimitedEnglishAdapter : SyncModelAdapter<LimitedEnglish>
    {
        public LimitedEnglishAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.LimitedEnglish Selector(LimitedEnglish model)
        {
            return new Data.School.Model.LimitedEnglish
            {
                Id = model.LimitedEnglishID,
                Description = model.Description,
                Code = model.Code,
                Name = model.Name,
                IsActive = model.IsActive,
                StateCode = model.StateCode,
                IsSystem = model.IsSystem,
                NcesCode = model.NCESCode,
                SifCode = model.SIFCode
            };
        }

        protected override void InsertInternal(IList<LimitedEnglish> entities)
        {
            ServiceLocatorSchool.LimitedEnglishService.Add(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<LimitedEnglish> entities)
        {
            ServiceLocatorSchool.LimitedEnglishService.Edit(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<LimitedEnglish> entities)
        {
            ServiceLocatorSchool.LimitedEnglishService.Delete(entities.Select(Selector).ToList());
        }

        protected override void PrepareToDeleteInternal(IList<LimitedEnglish> entities)
        {
            throw new NotImplementedException();
        }
    }
}
