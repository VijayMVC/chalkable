using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementReminderStorage:BaseDemoStorage<int, AnnouncementReminder>
    {
        public DemoAnnouncementReminderStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public IList<AnnouncementReminder> GetList(int announcementId, int value)
        {
            throw new NotImplementedException();
        }

        public void Add(AnnouncementReminder reminder)
        {
            if (!data.ContainsKey(reminder.Id))
                data[reminder.Id] = reminder;
        }

        public void Delete(AnnouncementReminder reminder)
        {
            Delete(reminder.Id);
        }

        public AnnouncementReminder GetById(int reminderId, int userId)
        {
            return data.Where(x => x.Value.Id == reminderId && x.Value.PersonRef == userId).Select(x => x.Value).First();
        }

        public void Update(AnnouncementReminder reminder)
        {
            if (data.ContainsKey(reminder.Id))
                data[reminder.Id] = reminder;
        }

        public IList<AnnouncementReminder> GetRemindersToProcess(DateTime nowSchoolTime, int count)
        {
            throw new NotImplementedException();
        }

        public void Update(IList<AnnouncementReminder> annReminders)
        {
            foreach (var annReminder in annReminders)
            {
                Update(annReminder);
            }
        }

        public void DeleteByAnnouncementId(int id)
        {
            var items = data.Where(x => x.Value.AnnouncementRef == id).Select(x => x.Key).ToList();
            Delete(items);
        }
    }
}
