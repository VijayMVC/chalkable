using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassRoomOptionStorage:BaseDemoIntStorage<ClassroomOption>
    {
        public DemoClassRoomOptionStorage(DemoStorage storage)
            : base(storage, x => x.Id)
        {
        }
    
        public override void Setup()
        {
           
        }
    }
}
