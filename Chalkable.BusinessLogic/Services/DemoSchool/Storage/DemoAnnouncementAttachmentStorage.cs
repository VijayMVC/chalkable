using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementAttachmentStorage:BaseDemoStorage<int, AnnouncementAttachment>
    {
        public DemoAnnouncementAttachmentStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(AnnouncementAttachment annAtt)
        {
            if (!data.ContainsKey(annAtt.Id))
                data[annAtt.Id] = annAtt;
        }

        public IList<AnnouncementAttachment> GetList(int userId, int roleId, string name)
        {

            //todo filter by role id
            return data.Where(x => x.Value.PersonRef == userId && x.Value.Name == name).Select(x => x.Value).ToList();
        }
        
        public PaginatedList<AnnouncementAttachment> GetPaginatedList(int announcementId, int i, int id, int start, int count, bool needsAllAttachments)
        {
            throw new NotImplementedException();
        }

        public AnnouncementAttachment GetById(int announcementAttachmentId, int userId, int roleId)
        {
            //todo filter by role
            return
                data.First(x => x.Value.Id == announcementAttachmentId && x.Value.PersonRef == userId).Value;

        }

        public IList<AnnouncementAttachment> GetAll(int announcementId)
        {
            return data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value).ToList();
        }
    }
}
