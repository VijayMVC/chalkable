using System;
using System.Linq;
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

        public ClassroomOption GetByClassId(int classId)
        {
            throw new NotImplementedException();
        }
    }
}
