using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoCourseTypeService : DemoSchoolServiceBase, ICourseTypeService
    {
        public DemoCourseTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<CourseType> courseTypes)
        {
            throw new NotImplementedException();
        }

        public void Edit(IList<CourseType> courseTypes)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<CourseType> courseTypes)
        {
            throw new NotImplementedException();
        }

        public IList<CourseType> GetList(bool activeOnly, string filter = null)
        {
            throw new NotImplementedException();
        }
    }
}
