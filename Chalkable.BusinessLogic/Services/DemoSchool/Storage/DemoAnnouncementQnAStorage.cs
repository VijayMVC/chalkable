using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementQnAStorage:BaseDemoIntStorage<AnnouncementQnAComplex>
    {
        public DemoAnnouncementQnAStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }

        public AnnouncementQnAQueryResult GetAnnouncementQnA(AnnouncementQnAQuery announcementQnAQuery)
        {
            var qnas = data.Select(x => x.Value);

            if (announcementQnAQuery.Id.HasValue)
                qnas = qnas.Where(x => x.Id == announcementQnAQuery.Id);
            if (announcementQnAQuery.AskerId.HasValue)
                qnas = qnas.Where(x => x.PersonRef == announcementQnAQuery.AskerId);


            if (announcementQnAQuery.AnnouncementId.HasValue)
                qnas = qnas.Where(x => x.AnnouncementRef == announcementQnAQuery.AnnouncementId);

            if (announcementQnAQuery.AnswererId.HasValue)
            {
                var announcementQnAComplexs = qnas as IList<AnnouncementQnAComplex> ?? qnas.ToList();
                var announcementIds = announcementQnAComplexs.Select(x => x.AnnouncementRef);
                var personIds = announcementIds.Select(annId => Storage.AnnouncementStorage.GetById(annId)).Select(announcement => announcement.PersonRef).ToList();
                qnas = announcementQnAComplexs.Where(x => personIds.Contains(x.PersonRef));
            }

            return new AnnouncementQnAQueryResult
            {
                AnnouncementQnAs = qnas.ToList(),
                Query = announcementQnAQuery
            };
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
