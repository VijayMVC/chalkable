using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    //TODO: implementation
    public class DemoClassroomOptionService : DemoSchoolServiceBase, IClassroomOptionService
    {
        public DemoClassroomOptionService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(IList<ClassroomOption> classroomOptions)
        {
            throw new NotImplementedException();
        }

        public void Edit(IList<ClassroomOption> classroomOptions)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<int> classroomOptionsIds)
        {
            throw new NotImplementedException();
        }

        public ClassroomOption GetClassOption(int classId)
        {
            throw new NotImplementedException();
        }
    }
}
