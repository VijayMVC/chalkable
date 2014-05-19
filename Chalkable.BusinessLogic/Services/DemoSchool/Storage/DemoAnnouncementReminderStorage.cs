using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementReminderStorage:BaseDemoIntStorage<AnnouncementReminder>
    {
        public DemoAnnouncementReminderStorage(DemoStorage storage)
            : base(storage, x => x.Id)
        {
        }

        public IList<AnnouncementReminder> GetList(int announcementId, int userId)
        {
            return
                data.Where(x => x.Value.AnnouncementRef == announcementId && x.Value.PersonRef == userId)
                    .Select(x => x.Value)
                    .ToList();
        }

        public AnnouncementReminder GetById(int reminderId, int userId)
        {
            return data.Where(x => x.Value.Id == reminderId && x.Value.PersonRef == userId).Select(x => x.Value).First();
        }

        public IList<AnnouncementReminder> GetRemindersToProcess(DateTime nowSchoolTime, int count)
        {
            throw new NotImplementedException();
        }

        public void DeleteByAnnouncementId(int id)
        {
            var items = data.Where(x => x.Value.AnnouncementRef == id).Select(x => x.Key).ToList();
            Delete(items);
        }

        public override void Setup()
        {
        }
    }
}
