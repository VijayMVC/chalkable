using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    class MealTypeAdapter : SyncModelAdapter<MealType>
    {
        public MealTypeAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.MealType Selector(MealType x)
        {
            return new Data.School.Model.MealType
            {
                Id = x.MealTypeID,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                NCESCode = x.NCESCode,
                SIFCode = x.SIFCode,
                StateCode = x.StateCode
            };
        }

        protected override void InsertInternal(IList<MealType> entities)
        {
            ServiceLocatorSchool.MealTypeService.Add(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<MealType> entities)
        {
            ServiceLocatorSchool.MealTypeService.Edit(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<MealType> entities)
        {
            ServiceLocatorSchool.MealTypeService.Delete(entities.Select(Selector).ToList());
        }

        protected override void PrepareToDeleteInternal(IList<MealType> entities)
        {
            throw new NotImplementedException();
        }
    }
}
