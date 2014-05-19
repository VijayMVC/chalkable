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
            Storage.ClassRoomOptionStorage.Add(classroomOptions);
        }

        public void Edit(IList<ClassroomOption> classroomOptions)
        {
            Storage.ClassRoomOptionStorage.Update(classroomOptions);
        }

        public void Delete(IList<int> classroomOptionsIds)
        {
            Storage.ClassRoomOptionStorage.Delete(classroomOptionsIds);
        }

        public ClassroomOption GetClassOption(int classId)
        {
            return Storage.ClassRoomOptionStorage.GetByClassId(classId);
        }
    }
}
