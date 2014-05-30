using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassAnnouncementTypeStorage:BaseDemoIntStorage<ClassAnnouncementType>
    {
        public DemoClassAnnouncementTypeStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<ClassAnnouncementType> GetAll(int classId)
        {
            return data.Where(x => x.Value.ClassRef == classId).Select(x => x.Value).ToList();
        }
    }
}
