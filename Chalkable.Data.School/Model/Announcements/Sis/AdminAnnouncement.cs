﻿using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Announcements.Sis
{
    public class AdminAnnouncement : Announcement
    {
        private const string ADMIN_ANNOUNCEMENT_TYPE_NAME = "Admin Announcement";
        
        public const string VW_ADMIN_ANNOUNCEMENT_NAME = "Chlk.vwAdminAnnouncement";

        public const string ADMIN_REF_FIELD = "AdminRef";
        public const string EXPIRES_FIELD = "Expires";
        public const string ADMIN_NAME_FIELD = "AdminName";

        public DateTime Expires { get; set; }
        public int AdminRef { get; set; }

        [NotDbFieldAttr]
        public string AdminName { get; set; }
        [NotDbFieldAttr]
        public string AdminGender { get; set; }
        [NotDbFieldAttr]
        public override string AnnouncementTypeName
        {
            get { return ADMIN_ANNOUNCEMENT_TYPE_NAME; }
        }
        [NotDbFieldAttr]
        public override AnnouncementType Type
        {
            get { return AnnouncementType.Admin; }
        }
        [NotDbFieldAttr]
        public override int OwnereId
        {
            get { return AdminRef; }
        }
    }
}
