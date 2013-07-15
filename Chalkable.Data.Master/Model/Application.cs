using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Enums;

namespace Chalkable.Data.Master.Model
{
    public class Application
    {
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
        public Guid DeveloperRef { get; set; }
        [DataEntityAttr]
        public Developer Developer { get; set; }
        public ApplicationStateEnum State { get; set; }
        public bool CanAttach { get; set; }
        public bool ShowInGradeView { get; set; }
        public bool HasStudentMyApps { get; set; }
        public bool HasTeacherMyApps { get; set; }
        public bool HasAdminMyApps { get; set; }
        public bool IsInternal { get; set; }
        public decimal? PricePerClass { get; set; }
        public decimal? PricePerSchool { get; set; }
        public Guid? OriginalRef { get; set; }
        [NotDbFieldAttr]
        public IList<ApplicationPicture> Pictures { get; set; }
    }

    public enum ApplicationStateEnum
    {
        Draft = 1,
        SubmitForApprove = 2,
        Approved = 3,
        Rejected = 4,
        Live = 5,
    }

    public class Developer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string WebSite { get; set; }
        [DataEntityAttr]
        public User User { get; set; }
    }

    public class ApplicationRating
    {
        public Guid Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public Guid UserRef { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
    }

    public class ApplicationPicture
    {
        public Guid Id { get; set; }
        public Guid ApplicationRef { get; set; }
    }

    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ApplicationCategory
    {
        public Guid Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public Guid CategoryRef { get; set; }
    }

    public class ApplicationPermission
    {
        public Guid Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public AppPermissionType Permission { get; set; }
    }
}