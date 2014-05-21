using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementStandardStorage:BaseDemoIntStorage<AnnouncementStandard>
    {
        public DemoAnnouncementStandardStorage(DemoStorage storage) : base(storage, null, true)
        {
        }

        public void Delete(int announcementId, int standardId)
        {

            var annStandarts =
                data.Where(x => x.Value.AnnouncementRef == announcementId && x.Value.StandardRef == standardId)
                    .Select(x => x.Key)
                    .ToList();

            Delete(annStandarts);
        }

        public IList<AnnouncementStandard> GetAll(int announcementId)
        {
            return data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value).ToList();
        }

        public override void Setup()
        {
            throw new System.NotImplementedException();
        }
    }
}
