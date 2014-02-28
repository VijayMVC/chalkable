﻿using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class CreateAnnouncementViewData
    {
        public AnnouncementViewData Announcement { get; set; }
        public IList<AnnouncementRecipientViewData> Recipients { get; set; }
        public bool IsDraft { get; set; }
        public bool CanAddStandard { get; set; }

        //public static CreateAnnouncementViewData Create(AnnouncementViewData announcement, bool isDra)
    }

    public class AnnouncementRecipientViewData
    {
        public bool ToAll { get; set; }
        public int? GradeLevelId { get; set; }
        public IdNameViewData<int> Person { get; set; }
        public int? RoleId { get; set; }

        public static AnnouncementRecipientViewData Create(AnnouncementRecipient announcementRecipient)
        {
            return new AnnouncementRecipientViewData
            {
                ToAll = announcementRecipient.ToAll,
                GradeLevelId = announcementRecipient.GradeLevelRef,
                RoleId = announcementRecipient.RoleRef,
                Person = !announcementRecipient.PersonRef.HasValue ? null
                              : IdNameViewData<int>.Create(announcementRecipient.PersonRef.Value, announcementRecipient.Person.FullName)
            };
        }
    }
}