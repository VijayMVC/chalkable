﻿using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Enums;

namespace Chalkable.Data.Master.Model
{
    public class Application
    {
        public const string ID_FIELD = "Id";
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public const string NAME_FIELD = "Name";
        public string Name { get; set; }
        public string Description { get; set; }
        public const string CREATE_DATE_TIME_FIELD = "CreateDateTime";
        public DateTime CreateDateTime { get; set; }
        public const string URL_FIELD = "Url";
        public string Url { get; set; }
        public string SecretKey { get; set; }
        public string ShortDescription { get; set; }
        public string AdditionalInfo { get; set; }
        public const string PRICE_FIELD = "Price";
        public decimal Price { get; set; }
        public string VideoUrl { get; set; }
        public Guid? SmallPictureRef { get; set; }
        public Guid? BigPictureRef { get; set; }
        public Guid? ExternalAttachPictureRef { get; set; }

        public const string DEVELOPER_REF_FIELD = "DeveloperRef";
        public Guid DeveloperRef { get; set; }
        public const string STATE_FIELD = "State";
        public ApplicationStateEnum State { get; set; }
        public const string CAN_ATTACH_FIELD = "CanAttach";
        public bool CanAttach { get; set; }
        public const string SHOW_IN_GRADE_VIEW_FIELD = "ShowInGradeView";
        public bool ShowInGradeView { get; set; }
        public const string HAS_STUDENT_MY_APPS_FIELD = "HasStudentMyApps";
        public bool HasStudentMyApps { get; set; }
        public const string HAS_TEACHER_MY_APPS_FIELD = "HasTeacherMyApps";
        public bool HasTeacherMyApps { get; set; }
        public const string HAS_ADMIN_MY_APPS_FIELD = "HasAdminMyApps";
        public bool HasAdminMyApps { get; set; }
        public bool HasParentMyApps { get; set; }
        public const string IS_INTERNAL_FIELD = "IsInternal";
        public bool IsInternal { get; set; }

        public const string IS_ADVANCED_FIELD = "IsAdvanced";
        public bool IsAdvanced { get; set; }

        public decimal? PricePerClass { get; set; }
        public decimal? PricePerSchool { get; set; }
        public const string ORIGINAL_REF_FIELD = "OriginalRef";
        public Guid? OriginalRef { get; set; }
        public int? InternalScore { get; set; }
        public string InternalDescription { get; set; }

        public const string HAS_TEACHER_EXTERNAL_ATTACH_FIELD = "HasTeacherExternalAttach";
        public bool HasTeacherExternalAttach { get; set; }

        public const string HAS_STUDENT_EXTERNAL_ATTACH_FIELD = "HasStudentExternalAttach";
        public bool HasStudentExternalAttach { get; set; }

        public const string HAS_ADMIN_EXTERNAL_ATTACH_FIELD = "HasAdminExternalAttach";
        public bool HasAdminExternalAttach { get; set; }

        public const string HAS_SYS_ADMIN_SETTINGS_FIELD = "HasSysAdminSettings";
        public bool HasSysAdminSettings { get; set; }

        public const string HAS_DISTRICT_ADMIN_SETTINGS_FIELD = "HasDistricAdminSettings";
        public bool HasDistricAdminSettings { get; set; }

        public const string HAS_STUDENT_PROFILE_FIELD = "HasStudentProfile";
        public bool HasStudentProfile { get; set; }

        public const string PROVIDES_RECOMENDED_CONTENT = "ProvidesRecomendedContent";
        public bool ProvidesRecomendedContent { get; set; }

        [NotDbFieldAttr]
        public bool? Ban { get; set; }
        [NotDbFieldAttr]
        public IList<ApplicationPicture> Pictures { get; set; }
        [NotDbFieldAttr]
        public Developer Developer { get; set; }
        [NotDbFieldAttr]
        public IList<ApplicationPermission> Permissions { get; set; }
        [NotDbFieldAttr]
        public IList<ApplicationCategory> Categories { get; set; }
        [NotDbFieldAttr]
        public IList<ApplicationGradeLevel> GradeLevels { get; set; }
        public const string AVG_FIELD = "Avg";
        [NotDbFieldAttr]
        public IList<ApplicationStandard> ApplicationStandards { get; set; }
        [NotDbFieldAttr]
        public Application LiveApplication { get; set; }
        [NotDbFieldAttr]
        public bool IsLive { get { return State == ApplicationStateEnum.Live; } }
        
    }

    public enum ApplicationStateEnum
    {
        Draft = 1,
        SubmitForApprove = 2,
        Approved = 3,
        Rejected = 4,
        Live = 5,
    }

    public enum AppMode
    {
        MyView,
        View,
        Edit,
        GradingView,
    }


    public class ApplicationRating
    {
        public const string ID_FIELD = "Id";
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public const string USER_REF_FIELD = "UserRef";

        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public Guid UserRef { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }

        [DataEntityAttr]
        public User User { get; set; }
    }

    public class ApplicationPicture
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public Guid ApplicationRef { get; set; }
    }

    public class Category
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ApplicationCategory
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public Guid ApplicationRef { get; set; }
        public Guid CategoryRef { get; set; }
    }

    public class ApplicationPermission
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public Guid ApplicationRef { get; set; }
        public AppPermissionType Permission { get; set; }
    }

    public class ApplicationGradeLevel
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public Guid ApplicationRef { get; set; }
        public int GradeLevel { get; set; }
    }

    public class ApplicationStandard
    {
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        [PrimaryKeyFieldAttr]
        public Guid ApplicationRef { get; set; }
        [PrimaryKeyFieldAttr]
        public Guid StandardRef { get; set; }
    }
}