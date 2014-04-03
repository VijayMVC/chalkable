using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
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

        public IList<AnnouncementAttachment> GetList(int value, int id, string name)
        {
            throw new NotImplementedException();
        }
        
        public PaginatedList<AnnouncementAttachment> GetPaginatedList(int announcementId, int i, int id, int start, int count, bool needsAllAttachments)
        {
            throw new NotImplementedException();
        }

        public AnnouncementAttachment GetById(int announcementAttachmentId, int i, int id)
        {
            throw new NotImplementedException();
        }
    }
}
