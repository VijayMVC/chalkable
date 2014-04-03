using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementQnAStorage:BaseDemoStorage<int, AnnouncementQnA>
    {
        public DemoAnnouncementQnAStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(AnnouncementQnAComplex annQnA)
        {
            throw new NotImplementedException();
        }

        public AnnouncementQnAQueryResult GetAnnouncementQnA(AnnouncementQnAQuery announcementQnAQuery)
        {
            throw new NotImplementedException();
        }

        public void Update(AnnouncementQnAComplex annQnA)
        {
            throw new NotImplementedException();
        }
    }
}
