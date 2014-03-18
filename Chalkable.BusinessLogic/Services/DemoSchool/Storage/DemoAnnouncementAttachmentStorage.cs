using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementAttachmentStorage
    {
        public void Add(AnnouncementAttachment annAtt)
        {
            throw new NotImplementedException();
        }

        public IList<AnnouncementAttachment> GetList(int value, int id, string name)
        {
            throw new NotImplementedException();
        }

        public AnnouncementAttachment GetAttachmentById(int announcementAttachmentId)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<AnnouncementAttachment> GetPaginatedList(int announcementId, int i, int id, int start, int count, bool needsAllAttachments)
        {
            throw new NotImplementedException();
        }
    }
}
