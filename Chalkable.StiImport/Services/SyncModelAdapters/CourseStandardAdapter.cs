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

        protected override void InsertInternal(IList<CourseStandard> entities)
        {
            var cs = entities.Select(x => new ClassStandard
            {
                ClassRef = x.CourseID,
                StandardRef = x.StandardID
            }).ToList();
            ServiceLocatorSchool.StandardService.AddClassStandards(cs);
        }

        protected override void UpdateInternal(IList<CourseStandard> entities)
        {
            //Nothing here
        }

        protected override void DeleteInternal(IList<CourseStandard> entities)
        {
            var classStandards = entities.Select(x => new ClassStandard
            {
                ClassRef = x.CourseID,
                StandardRef = x.StandardID
            }).ToList();
            ServiceLocatorSchool.StandardService.DeleteClassStandards(classStandards);
        }
    }
}