using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class AlphaGradeAdapter : SyncModelAdapter<AlphaGrade>
    {
        public AlphaGradeAdapter(AdapterLocator locator) : base(locator)
        {
        }
        private Data.School.Model.AlphaGrade Selector(AlphaGrade x)
        {
            return new Data.School.Model.AlphaGrade
            {
                Id = x.AlphaGradeID,
                Description = x.Description,
                Name = x.Name,
                SchoolRef = x.SchoolID
            };
        }

        protected override void InsertInternal(IList<AlphaGrade> entities)
        {
            var alphaGrades = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AlphaGradeService.AddAlphaGrades(alphaGrades);
        }
        
        protected override void UpdateInternal(IList<AlphaGrade> entities)
        {
            var alphaGrades = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AlphaGradeService.EditAlphaGrades(alphaGrades);
        }

        protected override void DeleteInternal(IList<AlphaGrade> entities)
        {
            var alphaGrades = entities.Select(x => new Data.School.Model.AlphaGrade { Id = x.AlphaGradeID }).ToList();
            ServiceLocatorSchool.AlphaGradeService.Delete(alphaGrades);
        }
    }
}