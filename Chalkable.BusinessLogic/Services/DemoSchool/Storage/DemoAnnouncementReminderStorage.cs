using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public void Delete(AnnouncementReminder reminder)
        {
            throw new NotImplementedException();
        }

        public AnnouncementReminder GetById(int reminderId, int value)
        {
            throw new NotImplementedException();
        }

        public void Update(AnnouncementReminder reminder)
        {
            throw new NotImplementedException();
        }

        public IList<AnnouncementReminder> GetRemindersToProcess(DateTime nowSchoolTime, int count)
        {
            throw new NotImplementedException();
        }
    }
}
