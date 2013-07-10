using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementReminderService
    {
        IList<AnnouncementReminder> GetReminders(Guid announcementId);
        AnnouncementReminder AddReminder(Guid announcementId, int? before);
        Announcement DeleteReminder(Guid reminderId);
        AnnouncementReminder EditReminder(Guid reminderId, int? before);// Append Method
        void ProcessReminders(int? count);
    }

    public class AnnouncementReminderService : SchoolServiceBase, IAnnouncementReminderService
    {
        public AnnouncementReminderService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        //TODO: security 
        //TODO: notification sending
        //TODO: tests

        public IList<AnnouncementReminder> GetReminders(Guid announcementId)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementReminderDataAccess(uow);
                return da.GetList(announcementId, Context.UserId);
            }
        }

        public AnnouncementReminder AddReminder(Guid announcementId, int? before)
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
                        Id = Guid.NewGuid(),
                        AnnouncementRef = ann.Id,
                        Before = before,
                        RemindDate = remiderDateTime,
                        Announcement = ann
                    };
                if (ann.PersonRef != Context.UserId)
                    reminder.PersonRef = Context.UserId;
                da.Insert(reminder);
                uow.Commit();
                return reminder;
            }
        }

        public Announcement DeleteReminder(Guid reminderId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementReminderDataAccess(uow);
                var reminder = da.GetById(reminderId, Context.UserId);
                if(!AnnouncementSecurity.IsReminderOwner(reminder, Context))
                    throw new ChalkableSecurityException();
                da.Delete(reminder);
                uow.Commit();
                return reminder.Announcement;
            }
         }

        public AnnouncementReminder EditReminder(Guid reminderId, int? before)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementReminderDataAccess(uow);
                var reminder = da.GetById(reminderId, Context.UserId);
                
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

        public void ProcessReminders(int? count)
        {
            throw new NotImplementedException();
        }
    }
}
