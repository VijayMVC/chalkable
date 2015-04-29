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

        public void Delete(IList<ClassroomOption> classroomOptions)
        {
            Storage.ClassRoomOptionStorage.Delete(classroomOptions);
        }

        public ClassroomOption SetUpClassroomOption(ClassroomOption classroomOption)
        {
            Edit(new List<ClassroomOption> {classroomOption});
            return GetClassOption(classroomOption.Id);
        }

        public ClassroomOption GetClassOption(int classId, bool useInowApi = false)
        {
            return Storage.ClassRoomOptionStorage.GetById(classId);
        }
    }
}
