using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class AnnouncementReminderViewData
    {
        public DateTime? RemindDate { get; set; }
        public Guid Id { get; set; }
        public int? Before { get; set; }
        public bool IsOwner { get; set; }

        public static AnnouncementReminderViewData Create(AnnouncementReminder reminder, Guid personId, Guid annOwnerId)
        {
            return new AnnouncementReminderViewData
            {
                Before = reminder.Before,
                Id = reminder.Id,
                IsOwner = reminder.PersonRef == personId || (!reminder.PersonRef.HasValue && annOwnerId == personId),
                RemindDate = reminder.RemindDate
            };
        }

        public static IList<AnnouncementReminderViewData> Create(IList<AnnouncementReminder> reminders, Guid personId,
                                                                 Guid annOwnerId)
        {
            return reminders.Select(x => Create(x, personId, annOwnerId)).ToList();
        }
    }
}