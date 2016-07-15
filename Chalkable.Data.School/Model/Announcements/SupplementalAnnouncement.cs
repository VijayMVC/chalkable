using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Announcements
{
    public class SupplementalAnnouncement : Announcement
    {
        public const string CLASS_REF_FIELD = nameof(ClassRef);
        public const string SCHOOL_SCHOOLYEAR_REF_FIELD = nameof(SchoolYearRef);
        public const string END_DATE_FIELD = nameof(Expires);
        public const string VISIBLE_FOR_STUDENT_FIELD = nameof(VisibleForStudent);
        public const string VW_SUPPLEMENTAL_ANNOUNCEMENT = "vwSupplementalAnnouncement";

        public DateTime Expires { get; set; }
        public bool VisibleForStudent { get; set; }
        public int ClassRef { get; set; }
        public int? ClassAnnouncementTypeRef { get; set; }
        public int SchoolYearRef { get; set; }
        
        [NotDbFieldAttr]
        public IList<Person> Recipients { get; set; }
        [NotDbFieldAttr]
        public int PrimaryTeacherRef { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherGender { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherName { get; set; }

        [NotDbFieldAttr]
        public string ClassName { get; set; }
        [NotDbFieldAttr]
        public string FullClassName { get; set; }
        
        [NotDbFieldAttr]
        public string ClassAnnouncementTypeName { get; set; }
        [NotDbFieldAttr]
        public int? ChalkableAnnouncementType { get; set; }

        [NotDbFieldAttr]
        public override string AnnouncementTypeName => ClassAnnouncementTypeName;

        [NotDbFieldAttr]
        public override AnnouncementTypeEnum Type => AnnouncementTypeEnum.Supplemental;
        [NotDbFieldAttr]
        public override int? OwnereId => PrimaryTeacherRef;
    }
}
