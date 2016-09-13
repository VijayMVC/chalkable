using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class SchoolProgramAdapter : SyncModelAdapter<SchoolProgram>
    {
        public SchoolProgramAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.SchoolProgram Selector(SchoolProgram x)
        {
            return new Data.School.Model.SchoolProgram
            {
                Id = x.SchoolProgramID,
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

        protected override void InsertInternal(IList<SchoolProgram> entities)
        {
            ServiceLocatorSchool.SchoolProgramService.Add(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<SchoolProgram> entities)
        {
            ServiceLocatorSchool.SchoolProgramService.Edit(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<SchoolProgram> entities)
        {
            ServiceLocatorSchool.SchoolProgramService.Delete(entities.Select(Selector).ToList());
        }

        protected override void PrepareToDeleteInternal(IList<SchoolProgram> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
