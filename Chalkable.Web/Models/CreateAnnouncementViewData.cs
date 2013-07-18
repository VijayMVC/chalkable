using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class CreateAnnouncementViewData
    {
        public AnnouncementViewData Announcement { get; set; }
        public IList<AnnouncementRecipientViewData> Recipients { get; set; }
        public bool IsDraft { get; set; }
    }

    public class AnnouncementRecipientViewData
    {
        public bool ToAll { get; set; }
        public Guid? GradeLevelId { get; set; }
        public IdNameViewData SchoolPerson { get; set; }
        public int? RoleId { get; set; }

        public static AnnouncementRecipientViewData Create(AnnouncementRecipient announcementRecipient)
        {
            return new AnnouncementRecipientViewData
            {
                ToAll = announcementRecipient.ToAll,
                GradeLevelId = announcementRecipient.GradeLevelRef,
                RoleId = announcementRecipient.RoleRef,
                //TODO : get person Data
                //SchoolPerson = !announcementRecipient.PersonRef.HasValue ? null
                //               : IdNameViewData.Create(announcementRecipient.PersonRef.Value, announcementRecipient.Person.DisplayName)
            };
        }
    }
}