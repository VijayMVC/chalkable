﻿using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoClassRoomOptionStorage : BaseDemoIntStorage<ClassroomOption>
    {
        public DemoClassRoomOptionStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoClassroomOptionService : DemoSchoolServiceBase, IClassroomOptionService
    {
        private DemoClassRoomOptionStorage ClassRoomOptionStorage {get; set; }
        public DemoClassroomOptionService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            ClassRoomOptionStorage = new DemoClassRoomOptionStorage();
        }

        public void Add(IList<ClassroomOption> classroomOptions)
        {
            ClassRoomOptionStorage.Add(classroomOptions);
        }

        public void Edit(IList<ClassroomOption> classroomOptions)
        {
            ClassRoomOptionStorage.Update(classroomOptions);
        }

        public void Delete(IList<ClassroomOption> classroomOptions)
        {
            ClassRoomOptionStorage.Delete(classroomOptions);
        }

        public ClassroomOption GetClassOption(int classId)
        {
            return ClassRoomOptionStorage.GetById(classId);
        }
    }
}
