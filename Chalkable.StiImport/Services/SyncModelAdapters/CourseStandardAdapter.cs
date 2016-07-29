using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class CourseStandardAdapter : SyncModelAdapter<CourseStandard>
    {
        public CourseStandardAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private ClassStandard Selector(CourseStandard x)
        {
            return new ClassStandard
            {
                ClassRef = x.CourseID,
                StandardRef = x.StandardID
            };
        }

        protected override void InsertInternal(IList<CourseStandard> entities)
        {
            var cs = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardService.AddClassStandards(cs);
        }

        protected override void UpdateInternal(IList<CourseStandard> entities)
        {
            //Nothing here
        }

        protected override void DeleteInternal(IList<CourseStandard> entities)
        {
            var classStandards = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardService.DeleteClassStandards(classStandards);
        }
    }
}