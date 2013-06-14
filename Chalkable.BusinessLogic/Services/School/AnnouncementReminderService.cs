using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementReminderService
    {
        IList<AnnouncementReminder> GetReminders(Guid announcementId);
        Announcement AddReminder(Guid announcementId, int? before);
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
            throw new NotImplementedException();
        }

        public Announcement AddReminder(Guid announcementId, int? before)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementReminderDataAccess(uow);
                var reminder = new AnnouncementReminder
                    {
                        Id = Guid.NewGuid(),
                        AnnouncementRef = announcementId,
                        PersonRef = Context.UserId,
                        Before = before,

                    };
                da.Create(reminder);
                uow.Commit();
                throw new NotImplementedException();
            }
           
        }

        public Announcement DeleteReminder(Guid reminderId)
        {
            throw new NotImplementedException();
        }

        public AnnouncementReminder EditReminder(Guid reminderId, int? before)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementReminderDataAccess(uow);
                var reminder = da.GetById(reminderId);
                
                var annExpires = reminder.Announcement.Expires;
                var nowLocalTime = Context.NowSchoolTime;
                reminder.RemindDate = before.HasValue && annExpires >= nowLocalTime ? annExpires.AddDays(-before.Value) : nowLocalTime;
                da.Update(reminder);
                uow.Commit();
                throw new NotImplementedException();
            }
        }

        public void ProcessReminders(int? count)
        {
            throw new NotImplementedException();
        }
    }
}
