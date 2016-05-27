using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Enums;

namespace Chalkable.Data.Master.Model
{
    public class Application
    {


        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string Url { get; set; }
        public string SecretKey { get; set; }
        public string ShortDescription { get; set; }
        public string AdditionalInfo { get; set; }
        public decimal Price { get; set; }
        public string VideoUrl { get; set; }
        public Guid? SmallPictureRef { get; set; }
        public Guid? BigPictureRef { get; set; }
        public Guid? ExternalAttachPictureRef { get; set; }
        
        public Guid DeveloperRef { get; set; }
        public ApplicationStateEnum State { get; set; }
        public bool CanAttach { get; set; }
        public bool ShowInGradeView { get; set; }
        public bool HasStudentMyApps { get; set; }
        public bool HasTeacherMyApps { get; set; }
        public bool HasAdminMyApps { get; set; }
        public bool HasParentMyApps { get; set; }
        public bool IsInternal { get; set; }
        public bool IsAdvanced { get; set; }
        public decimal? PricePerClass { get; set; }
        public decimal? PricePerSchool { get; set; }
        public Guid? OriginalRef { get; set; }
        public int? InternalScore { get; set; }
        public string InternalDescription { get; set; }
        public bool HasTeacherExternalAttach { get; set; }
        public bool HasStudentExternalAttach { get; set; }
        public bool HasAdminExternalAttach { get; set; }
        public bool HasSysAdminSettings { get; set; }
        public bool HasDistrictAdminSettings { get; set; }
        public bool HasStudentProfile { get; set; }
        public bool ProvidesRecommendedContent { get; set; }

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
        [NotDbFieldAttr]
        public IList<ApplicationStandard> ApplicationStandards { get; set; }
        [NotDbFieldAttr]
        public Application LiveApplication { get; set; }
        [NotDbFieldAttr]
        public bool IsLive => State == ApplicationStateEnum.Live;
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

    public class ApplicationPicture
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
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
        public Guid ApplicationRef { get; set; }
        public Guid CategoryRef { get; set; }
    }

    public class ApplicationPermission
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public AppPermissionType Permission { get; set; }
    }

    public class ApplicationGradeLevel
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public int GradeLevel { get; set; }
    }

    public class ApplicationStandard
    {
        [PrimaryKeyFieldAttr]
        public Guid ApplicationRef { get; set; }
        [PrimaryKeyFieldAttr]
        public Guid StandardRef { get; set; }
    }
}