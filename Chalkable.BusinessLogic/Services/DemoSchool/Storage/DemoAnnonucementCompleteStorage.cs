using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Models;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementCompleteStorage:BaseDemoIntStorage<AnnouncementComplete>
    {
        public DemoAnnouncementCompleteStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }

        public void SetComplete(AnnouncementComplete complete)
        {
            if (data.Count(x => x.Value.AnnouncementId == complete.AnnouncementId && x.Value.UserId == complete.UserId) == 0)
            {
                data.Add(GetNextFreeId(), complete);
            }
            var item = data.First(x => x.Value.AnnouncementId == complete.AnnouncementId && x.Value.UserId == complete.UserId).Key;
            data[item] = complete;
        }

        public bool? GetComplete(int announcementId, int userId)
        {
            if (data.Count(x => x.Value.AnnouncementId == announcementId && x.Value.UserId == userId) == 0)
            {
                return (bool?)false;
            }
            return data.First(x => x.Value.AnnouncementId == announcementId && x.Value.UserId == userId).Value.Complete;
        }
    }
}
