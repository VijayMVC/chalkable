using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementReminderService
    {
        IList<AnnouncementReminder> GetReminders(int announcementId);
        AnnouncementReminder AddReminder(int announcementId, int? before);
        Announcement DeleteReminder(int reminderId);
        AnnouncementReminder EditReminder(int reminderId, int? before);// Append Method
        void ProcessReminders(int count);
    }

    public class AnnouncementReminderService : SchoolServiceBase, IAnnouncementReminderService
    {
        public AnnouncementReminderService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        //TODO: security 
        //TODO: notification sending
        //TODO: tests

        public IList<AnnouncementReminder> GetReminders(int announcementId)
        {
            using (var uow = Read())
            {
                if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                    throw new UnassignedUserException();

                var da = new AnnouncementReminderDataAccess(uow);
                return da.GetList(announcementId, Context.UserLocalId.Value);
            }
        }

        public AnnouncementReminder AddReminder(int announcementId, int? before)
        {
            using (var uow = Update())
            {
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId); // security here 
                var da = new AnnouncementReminderDataAccess(uow);
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
                da.Insert(reminder);
                uow.Commit();
                return reminder;
            }
        }

        public Announcement DeleteReminder(int reminderId)
        {
            using (var uow = Update())
            {
                if(!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                    throw new UnassignedUserException();

                var da = new AnnouncementReminderDataAccess(uow);
                var reminder = da.GetById(reminderId, Context.UserLocalId.Value);
                if(!AnnouncementSecurity.IsReminderOwner(reminder, Context))
                    throw new ChalkableSecurityException();
                da.Delete(reminder);
                uow.Commit();
                return reminder.Announcement;
            }
         }

        public AnnouncementReminder EditReminder(int reminderId, int? before)
        {
            using (var uow = Update())
            {
                if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                    throw new UnassignedUserException();

                var da = new AnnouncementReminderDataAccess(uow);
                var reminder = da.GetById(reminderId, Context.UserLocalId.Value);
                
                if(!AnnouncementSecurity.IsReminderOwner(reminder, Context))
                    throw new ChalkableSecurityException();

                reminder.Before = before;
                var annExpires = reminder.Announcement.Expires;
                var nowLocalTime = Context.NowSchoolTime.Date;
                reminder.RemindDate = before.HasValue && annExpires >= nowLocalTime ? annExpires.AddDays(-before.Value) : nowLocalTime;
                da.Update(reminder);
                uow.Commit();
                return reminder;
            }
        }

        public void ProcessReminders(int count)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            var now = DateTime.UtcNow;
            Debug.WriteLine(now);

            using (var uow = Update())
            {
                var da = new AnnouncementReminderDataAccess(uow);
                var toProcess = da.GetRemindersToProcess(Context.NowSchoolTime, count);

                var toProcessList = toProcess.ToList();
                foreach (var announcementReminder in toProcessList)
                {
                    var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementReminder.AnnouncementRef);
                    ServiceLocator.NotificationService.AddAnnouncementReminderNotification(announcementReminder, ann);
                    announcementReminder.Processed = true;
                    da.Update(announcementReminder);
                }
                uow.Commit();
            }
        }
    }
}
