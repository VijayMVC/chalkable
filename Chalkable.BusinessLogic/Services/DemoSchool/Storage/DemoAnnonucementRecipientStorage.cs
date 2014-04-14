using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementRecipientStorage:BaseDemoStorage<int ,AnnouncementRecipient>
    {
        public DemoAnnouncementRecipientStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(List<AnnouncementRecipient> annRecipients)
        {
            foreach (var announcementRecipient in annRecipients)
            {
                if (!data.ContainsKey(announcementRecipient.Id))
                    data[announcementRecipient.Id] = announcementRecipient;
            }
            
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

      
    }
}
