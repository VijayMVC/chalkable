using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementRecipientStorage:BaseDemoIntStorage<AnnouncementRecipient>
    {
        public DemoAnnouncementRecipientStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

  
        public void DeleteByAnnouncementId(int id)
        {
            var annRep = data.Where(x => x.Value.AnnouncementRef == id).Select(x => x.Key).ToList();
            Delete(annRep);
        }

        public IList<AnnouncementRecipient> GetList(int announcementId)
        {
            return data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value).ToList();
        }


        public override void Setup()
        {
        }
    }
}
