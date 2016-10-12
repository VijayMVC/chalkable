using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class InfractionAdapter : SyncModelAdapter<Infraction>
    {
        public InfractionAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Infraction Selector(Infraction x)
        {
            return new Data.School.Model.Infraction
            {
                Code = x.Code,
                Demerits = x.Demerits,
                Description = x.Description,
                Id = x.InfractionID,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name,
                NCESCode = x.NCESCode,
                SIFCode = x.SIFCode,
                StateCode = x.StateCode,
                //TODO:
                // The VisibleInClassroom property was added to Inow after the Infraction table was synced. 
                // At the time that this release of Chalkable goes out, some Inow apis will have this new 
                // column and some won't.  We need to take this into account and make the sync model property
                // a nullable bool.  When null, we should assume the value is true so that the infraction will
                // be displayed in the UI
                VisibleInClassroom = x.VisibleInClassroom ?? true
            };
        }

        protected override void InsertInternal(IList<Infraction> entities)
        {
            var infractions =entities.Select(Selector).ToList();
            ServiceLocatorSchool.InfractionService.Add(infractions);
        }

        protected override void UpdateInternal(IList<Infraction> entities)
        {
            var infractions = entities.Select(Selector).ToList();
            ServiceLocatorSchool.InfractionService.Edit(infractions);
        }

        protected override void DeleteInternal(IList<Infraction> entities)
        {
            var infractions = entities.Select(x => new Data.School.Model.Infraction { Id = x.InfractionID }).ToList();
            ServiceLocatorSchool.InfractionService.Delete(infractions);
        }

        protected override void PrepareToDeleteInternal(IList<Infraction> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}