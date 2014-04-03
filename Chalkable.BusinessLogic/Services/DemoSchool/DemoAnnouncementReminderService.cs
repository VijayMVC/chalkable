using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementReminderService : DemoSchoolServiceBase, IAnnouncementReminderService
    {
        public DemoAnnouncementReminderService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<AnnouncementReminder> GetReminders(int announcementId)
        {
            if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                throw new UnassignedUserException();

            return Storage.AnnouncementReminderStorage.GetList(announcementId, Context.UserLocalId.Value);
        }

        public AnnouncementReminder AddReminder(int announcementId, int? before)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId); // security here 

            var nowDate = Context.NowSchoolTime.Date;
            var remiderDateTime = before.HasValue && ann.Expires >= nowDate
                                 ? ann.Expires.AddDays(-before.Value) : nowDate;

            var reminder = new AnnouncementReminder
            {
                AnnouncementRef = ann.Id,
                Before = before,
                RemindDate = remiderDateTime,
                Announcement = ann
            };
            if (ann.PersonRef != Context.UserLocalId)
                reminder.PersonRef = Context.UserLocalId;
            Storage.AnnouncementReminderStorage.Add(reminder);
            return reminder;


        }

        public Announcement DeleteReminder(int reminderId)
        {
            if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                throw new UnassignedUserException();

            var reminder = Storage.AnnouncementReminderStorage.GetById(reminderId, Context.UserLocalId.Value);
            if (!AnnouncementSecurity.IsReminderOwner(reminder, Context))
                throw new ChalkableSecurityException();
            Storage.AnnouncementReminderStorage.Delete(reminder);
            return reminder.Announcement;
         }

        public AnnouncementReminder EditReminder(int reminderId, int? before)
        {
            if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                throw new UnassignedUserException();

            var reminder = Storage.AnnouncementReminderStorage.GetById(reminderId, Context.UserLocalId.Value);

            if (!AnnouncementSecurity.IsReminderOwner(reminder, Context))
                throw new ChalkableSecurityException();

            reminder.Before = before;
            var annExpires = reminder.Announcement.Expires;
            var nowLocalTime = Context.NowSchoolTime.Date;
            reminder.RemindDate = before.HasValue && annExpires >= nowLocalTime ? annExpires.AddDays(-before.Value) : nowLocalTime;
            Storage.AnnouncementReminderStorage.Update(reminder);
            return reminder;
        }

        public void ProcessReminders(int count)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            var now = DateTime.UtcNow;
            Debug.WriteLine(now);

            var toProcess = Storage.AnnouncementReminderStorage.GetRemindersToProcess(Context.NowSchoolTime, count);

            var toProcessList = toProcess.ToList();
            foreach (var announcementReminder in toProcessList)
            {
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementReminder.AnnouncementRef);
                ServiceLocator.NotificationService.AddAnnouncementReminderNotification(announcementReminder, ann);
                announcementReminder.Processed = true;
                Storage.AnnouncementReminderStorage.Update(announcementReminder);
            }
        }
    }
}
