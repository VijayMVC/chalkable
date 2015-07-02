﻿using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Announcements
{
    public class ClassAnnouncement : Announcement
    {
        public const string VW_CLASS_ANNOUNCEMENT_NAME = "vwClassAnnouncement";
        public const string VW_CLASS_ANNOUNCEMENT_COMPLEX = "vwClassAnnouncementComplex";

        public const string EXPIRES_FIELD = "Expires";
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string CLASS_ANNOUNCEMENT_TYPE_REF_FIELD = "ClassAnnouncementTypeRef";
        public const string SIS_ACTIVITY_ID_FIELD = "SisActivityId";
        public const string PRIMARY_TEACHER_REF_FIELD = "PrimaryTeacherRef";
        public const string SCHOOL_REF_FIELD = "SchoolRef";

        public const string VISIBLE_FOR_STUDENT = "VisibleForStudent";

        //todo remove this later

        public DateTime Expires { get; set; }
        public int? ClassAnnouncementTypeRef { get; set; }
        public int ClassRef { get; set; }
        public int SchoolRef { get; set; }
        public int Order { get; set; }
        public bool Dropped { get; set; }
        
        public int? SisActivityId { get; set; }
        public decimal? MaxScore { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool VisibleForStudent { get; set; }
        public bool MayBeDropped { get; set; }

        
        [NotDbFieldAttr]
        public override bool IsSubmitted
        {
            get { return SisActivityId.HasValue && base.IsSubmitted; }
        }

        public override int OwnereId
        {
            get { return PrimaryTeacherRef; }
        }

        [NotDbFieldAttr]
        public override string AnnouncementTypeName
        {
            get { return ClassAnnouncementTypeName; }
        }
        [NotDbFieldAttr]
        public override AnnouncementType Type
        {
            get { return AnnouncementType.Class; }
        }
        [NotDbFieldAttr]
        public bool MayBeExempt { get; set; }
        [NotDbFieldAttr]
        public bool IsScored { get; set; }
        [NotDbFieldAttr]
        public int PrimaryTeacherRef { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherGender { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherName { get; set; }

        [NotDbFieldAttr]
        public string ClassAnnouncementTypeName { get; set; }
        [NotDbFieldAttr]
        public int? ChalkableAnnouncementType { get; set; }
        
        [NotDbFieldAttr]
        public string ClassName { get; set; }
        [NotDbFieldAttr]
        public string FullClassName { get; set; }
        [NotDbFieldAttr]
        public Guid? DepartmentId { get; set; }

        [NotDbFieldAttr]
        public override string Title
        {
            get { return !string.IsNullOrEmpty(base.Title) ? base.Title : DefaultTitle; }
            set { base.Title = value; }
        }

        [NotDbFieldAttr]
        public string DefaultTitle
        {
            get
            {
                return !string.IsNullOrEmpty(ClassAnnouncementTypeName)
                           ? string.Format("{0} {1}", ClassAnnouncementTypeName, Order)
                           : null;
            }
        }

    }
}
