﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementApplication
    {
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public const string ACTIVE_FIELD = "Active";
    
        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public Guid ApplicationRef { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
    }
}
