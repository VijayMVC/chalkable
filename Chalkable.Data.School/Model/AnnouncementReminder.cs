﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementReminder
    {
        public const string ID_FIELD = "Id";
        public const string REMIND_DATE_FIELD = "RemindDate";
        public const string PROCESSED_FIELD = "Processed";
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";


        [IdentityFieldAttr]
        public int Id { get; set; }
        public DateTime? RemindDate { get; set; }
        public bool Processed { get; set; }
        public int AnnouncementRef { get; set; }
        public int? Before { get; set; }
        public int? PersonRef { get; set; }
        
        [NotDbFieldAttr]
        public Announcement Announcement { get; set; }
    }
}
