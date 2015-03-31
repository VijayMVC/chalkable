using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementStandardStorage:BaseDemoIntStorage<AnnouncementStandard>
    {
        public DemoAnnouncementStandardStorage() : base(null, true)
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

        public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        {
            return GetAll(announcementId).Select(x => new AnnouncementStandardDetails
            {
                AnnouncementRef = x.AnnouncementRef,
                StandardRef = x.StandardRef,
                Standard = StorageLocator.StandardStorage.GetById(x.StandardRef)
            }).ToList();
        }
    }
}
