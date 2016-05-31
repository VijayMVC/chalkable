using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class BaseApplicationInfo
    {
        public ShortApplicationInfo ShortApplicationInfo { get; set; }
        public ApplicationPricesInfo ApplicationPrices { get; set; }
        public ApplicationAccessInfo ApplicationAccessInfo { get; set; }
        public IList<Guid> Categories { get; set; }
        public IList<Guid> PicturesId { get; set; }
        public IList<int> GradeLevels { get; set; }
        public Guid DeveloperId { get; set; }
        public IList<AppPermissionType> PermissionIds { get; set; }
        public IList<Guid> StandardsIds { get; set; }

        protected BaseApplicationInfo()
        {
            ShortApplicationInfo = new ShortApplicationInfo();
            ApplicationAccessInfo = new ApplicationAccessInfo();
            ApplicationPrices = new ApplicationPricesInfo();
        }

        public static BaseApplicationInfo Create(ShortApplicationInfo shortApplicationInfo, Guid developerId
            , IList<AppPermissionType> permissionIds = null,  IList<Guid> picturesId = null
            , ApplicationPricesInfo applicationPricesInfo = null, IList<Guid> categories = null
            , ApplicationAccessInfo applicationAccess = null, IList<int> gradeLevels = null, IList<Guid> standardsIds = null)
        {
            return new BaseApplicationInfo
            {
                ShortApplicationInfo = shortApplicationInfo ?? new ShortApplicationInfo(),
                ApplicationPrices = applicationPricesInfo ?? new ApplicationPricesInfo(),
                Categories = categories,
                PicturesId = picturesId,
                ApplicationAccessInfo = applicationAccess ?? new ApplicationAccessInfo(),
                GradeLevels = gradeLevels,
                DeveloperId = developerId,
                PermissionIds = permissionIds ?? new List<AppPermissionType>(),
                StandardsIds = standardsIds ?? new List<Guid>()
            };
        }

        public static BaseApplicationInfo Create(Application application)
        {
            return new BaseApplicationInfo
            {
                ShortApplicationInfo = ShortApplicationInfo.Create(application),
                ApplicationAccessInfo = ApplicationAccessInfo.Create(application),
                ApplicationPrices = ApplicationPricesInfo.Create(application),
                DeveloperId = application.DeveloperRef,
                Categories = application.Categories.Select(x => x.CategoryRef).ToList(),
                PicturesId = application.Pictures.Select(x => x.Id).ToList(),
                GradeLevels = application.GradeLevels.Select(x => x.GradeLevel).ToList(),
                PermissionIds = application.Permissions.Select(x=>x.Permission).ToList(),
                StandardsIds = application.ApplicationStandards.Select(x=>x.StandardRef).ToList()
            };
        }
    }

    public class ShortApplicationInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string VideoDemoUrl { get; set; }
        public Guid? SmallPictureId { get; set; }
        public Guid? BigPictureId { get; set; }
        public Guid? ExternalAttachPictureId { get; set; }
        public bool AdvancedApp { get; set; }

        public static ShortApplicationInfo Create(string name, string url, string shortDescription
            , string description, string videoDemoUrl, bool advancedApp, Guid? smallPictureId, Guid? bigPictureId, Guid? externalAttachPictureId)
        {
            return new ShortApplicationInfo
            {
                Name = name,
                Url = url,
                ShortDescription = shortDescription,
                Description = description,
                VideoDemoUrl = videoDemoUrl,
                SmallPictureId = smallPictureId,
                BigPictureId = bigPictureId,
                ExternalAttachPictureId = externalAttachPictureId,
                AdvancedApp = advancedApp
            };
        }


        public static ShortApplicationInfo Create(Application application)
        {
            return new ShortApplicationInfo
            {
                Name = application.Name,
                ExternalAttachPictureId = application.ExternalAttachPictureRef,
                BigPictureId = application.BigPictureRef,
                Description = application.Description,
                ShortDescription = application.ShortDescription,
                SmallPictureId = application.SmallPictureRef,
                VideoDemoUrl = application.VideoUrl,
                Url = application.Url,
                AdvancedApp = application.IsAdvanced
            };
        }
    }

    public class ApplicationPricesInfo
    {
        public decimal Price { get; set; }
        public decimal? PricePerClass { get; set; }
        public decimal? PricePerSchool { get; set; }

        public static ApplicationPricesInfo Create(double price, double? pricePerClass, double? pricePerSchool)
        {
            return new ApplicationPricesInfo
            {
                PricePerClass = (decimal?)pricePerClass,
                Price = (decimal)price,
                PricePerSchool = (decimal?)pricePerSchool,
            };
        }
        public static ApplicationPricesInfo Create(Application application)
        {
            return new ApplicationPricesInfo
            {
                Price = application.Price,
                PricePerClass = application.PricePerClass,
                PricePerSchool = application.PricePerSchool
            };
        }
    }

    public class ApplicationAccessInfo
    {
        public bool HasStudentMyApps { get; set; }
        public bool HasTeacherMyApps { get; set; }
        public bool HasAdminMyApps { get; set; }
        public bool HasParentMyApps { get; set; }
        public bool HasTeacherExternalAttach { get; set; }
        public bool HasStudentExternalAttach { get; set; }
        public bool HasAdminExternalAttach { get; set; }
        public bool HasSysAdminSettings { get; set; }
        public bool HasDistrictAdminSettings { get; set; }
        public bool HasStudentProfile { get; set; }
        public bool CanAttach { get; set; }
        public bool ShowInGradeView { get; set; }
        public bool ProvidesRecommendedContent { get; set; }
        public static ApplicationAccessInfo Create(bool hasStudentMyApps, bool hasTeacherMyApps, bool hasAdminMyApps, bool hasParentMyApps
            , bool canAttach, bool showInGradeView, bool hasTeacherExternalAttach, bool hasStudentExternalAttach, bool hasAdminExternalAttach,
            bool hasSysAdminSettings, bool hasDistrictAdminSettings, bool hasStudentProfile, bool providesRecommendedContent)
        {
            return new ApplicationAccessInfo
            {
                HasStudentMyApps = hasStudentMyApps,
                HasTeacherMyApps = hasTeacherMyApps,
                HasAdminMyApps = hasAdminMyApps,
                HasParentMyApps = hasParentMyApps,
                HasAdminExternalAttach = hasAdminExternalAttach,
                HasTeacherExternalAttach = hasTeacherExternalAttach,
                HasStudentExternalAttach = hasStudentExternalAttach,
                HasSysAdminSettings = hasSysAdminSettings,
                HasDistrictAdminSettings = hasDistrictAdminSettings,
                HasStudentProfile = hasStudentProfile,
                CanAttach = canAttach,
                ShowInGradeView = showInGradeView,
                ProvidesRecommendedContent =  providesRecommendedContent
            };
        }

        public static ApplicationAccessInfo Create(Application application)
        {
            return new ApplicationAccessInfo
            {
                CanAttach = application.CanAttach,
                HasAdminMyApps = application.HasAdminMyApps,
                HasStudentMyApps = application.HasStudentMyApps,
                HasTeacherMyApps = application.HasTeacherMyApps,
                HasParentMyApps = application.HasParentMyApps,
                HasAdminExternalAttach = application.HasAdminExternalAttach,
                HasTeacherExternalAttach = application.HasTeacherExternalAttach,
                HasStudentExternalAttach = application.HasStudentExternalAttach,
                HasSysAdminSettings = application.HasSysAdminSettings,
                HasDistrictAdminSettings = application.HasDistrictAdminSettings,
                HasStudentProfile = application.HasStudentProfile,
                ShowInGradeView = application.ShowInGradeView,
                ProvidesRecommendedContent = application.ProvidesRecommendedContent
            };
        }

    }
}
