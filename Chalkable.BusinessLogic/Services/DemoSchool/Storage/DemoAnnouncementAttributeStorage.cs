using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementAttributeStorage : BaseDemoIntStorage<AnnouncementAttribute>
    {
        public DemoAnnouncementAttributeStorage(DemoStorage storage) : base(storage, x=> x.Id, true)
        {
        }

    }
}
