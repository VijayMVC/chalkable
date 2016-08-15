using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class CourseTypeAdapter : SyncModelAdapter<CourseType>
    {
        public CourseTypeAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.CourseType Selector(CourseType courseType)
        {
            return new Data.School.Model.CourseType
            {
                Id = courseType.CourseTypeID,
                Name = courseType.Name,
                Description = courseType.Description,
                Code = courseType.Code,
                IsActive = courseType.IsActive,
                IsSystem = courseType.IsSystem,
                NCESCode = courseType.NCESCode,
                SIFCode = courseType.SIFCode,
                StateCode = courseType.StateCode
            };
        }

        protected override void InsertInternal(IList<CourseType> entities)
        {
            var chalkableCourseTypes = entities.Select(Selector).ToList();
            ServiceLocatorSchool.CourseTypeService.Add(chalkableCourseTypes);
        }

        protected override void UpdateInternal(IList<CourseType> entities)
        {
            var chalkableCourseTypes = entities.Select(Selector).ToList();
            ServiceLocatorSchool.CourseTypeService.Edit(chalkableCourseTypes);
        }

        protected override void DeleteInternal(IList<CourseType> entities)
        {
            var courseTypes = entities.Select(x => new Data.School.Model.CourseType { Id = x.CourseTypeID }).ToList();
            ServiceLocatorSchool.CourseTypeService.Delete(courseTypes);
        }

        protected override void PrepareToDeleteInternal(IList<CourseType> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}