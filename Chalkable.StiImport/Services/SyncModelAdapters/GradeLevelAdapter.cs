using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class GradeLevelAdapter : SyncModelAdapter<GradeLevel>
    {
        public GradeLevelAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<GradeLevel> entities)
        {
            var gradeLevels = entities.Select(x => new Data.School.Model.GradeLevel
            {
                Id = x.GradeLevelID,
                Description = x.Description,
                Name = x.Name,
                Number = x.Sequence
            }).ToList();
            ServiceLocatorSchool.GradeLevelService.Add(gradeLevels);
        }

        protected override void UpdateInternal(IList<GradeLevel> entities)
        {
            var gradeLevels = entities
                .Select(x => new Data.School.Model.GradeLevel
                {
                    Id = x.GradeLevelID,
                    Description = x.Description,
                    Name = x.Name,
                    Number = x.Sequence
                }).ToList();
            ServiceLocatorSchool.GradeLevelService.Edit(gradeLevels);
        }

        protected override void DeleteInternal(IList<GradeLevel> entities)
        {
            var ids = entities.Select(x => new Data.School.Model.GradeLevel { Id = (int)x.GradeLevelID }).ToList();
            ServiceLocatorSchool.GradeLevelService.Delete(ids);
        }
    }
}