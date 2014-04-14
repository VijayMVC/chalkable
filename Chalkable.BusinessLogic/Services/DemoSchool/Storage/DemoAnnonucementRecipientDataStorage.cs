using System;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementRecipientDataStorage:BaseDemoStorage<int, AnnouncementRecipientData>
    {
        public DemoAnnouncementRecipientDataStorage(DemoStorage storage)
            : base(storage)
        {

        }

        public void Update(int announcementId, int userId, bool starred, int? starredAutomatically, DateTime modifDateTime)
        {
            var exists = data.Count(x => x.Value.AnnouncementRef == announcementId && x.Value.PersonRef == userId) == 1;

            if (exists)
            {
                var annRecipient =
                    data.First(x => x.Value.AnnouncementRef == announcementId && x.Value.PersonRef == userId).Value;
                annRecipient.LastModifiedDate = modifDateTime;
                annRecipient.Starred = starred;

            }
            else
            {
                var annRecipient = new AnnouncementRecipientData()
                {
                    AnnouncementRef = announcementId,
                    PersonRef = userId,
                    Id = GetNextFreeId(),
                    LastModifiedDate = modifDateTime,
                    Starred = starred,
                    StarredAutomatically = starredAutomatically.HasValue && starredAutomatically.Value == 1,

                };
                data.Add(annRecipient.Id, annRecipient);
            }
        }
   
    }
}
