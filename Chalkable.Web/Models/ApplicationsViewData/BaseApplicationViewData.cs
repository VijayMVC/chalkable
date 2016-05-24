using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.SchoolsViewData;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class BaseApplicationViewData
    {
        private const int SHORT_LENGHT = 270;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string VideoDemoUrl { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public Guid? SmallPictureId { get; set; }
        public Guid? BigPictureId { get; set; }
        public Guid? ExternalAttachPictureId { get; set; }
        public string MyAppsUrl { get; set; }
        public string SecretKey { get; set; }
        public string EncodedSecretKey { get; set; }
        public int State { get; set; }
        public Guid? DeveloperId { get; set; }
        public Guid? LiveAppId { get; set; }
        public bool IsInternal { get; set; }
        public bool IsAdvanced { get; set; }
        public int? InternalScore { get; set; }
        public string InternalDescription { get; set; }
        public bool? Ban { get; set; }
        public ApplicationAccessViewData ApplicationAccess { get; set; }
        public IList<Guid> Picturesid { get; set; }
        public BaseApplicationViewData LiveApplication { get; set; }
        public bool HasDistrictAdminSettings { get; set; }
        public bool? IsBannedForDistrict { get; set; }
        public bool? IsPartiallyBanned { get; set; }
        public bool? IsBannedForCurrentSchool { get; set; }
        
        protected BaseApplicationViewData(Application application)
        {
            Id = application.Id;
            Name = application.Name;
            VideoDemoUrl = application.VideoUrl;
            Url = application.Url;
            State = (int)application.State;
            DeveloperId = application.DeveloperRef;
            Description = application.Description;
            ShortDescription = StringTools.BuildShortText(application.ShortDescription, SHORT_LENGHT);
            SmallPictureId = application.SmallPictureRef;
            BigPictureId = application.BigPictureRef;
            ExternalAttachPictureId = application.ExternalAttachPictureRef;
            ApplicationAccess = ApplicationAccessViewData.Create(application);
            LiveAppId = application.OriginalRef;
            IsInternal = application.IsInternal;
            IsAdvanced = application.IsAdvanced;
            InternalScore = application.InternalScore;
            InternalDescription = application.InternalDescription;
            Picturesid = application.Pictures.Select(x => x.Id).ToList();
            if (application.LiveApplication != null)
                LiveApplication = Create(application.LiveApplication);
            Ban = application.Ban;
            HasDistrictAdminSettings = application.HasDistrictAdminSettings;
            MyAppsUrl = AppTools.BuildAppUrl(application, null, AppMode.MyView);
            IsBannedForCurrentSchool = null;
            IsBannedForDistrict = null;
            IsPartiallyBanned = null;
        }

        public static BaseApplicationViewData Create(Application application)
        {
            return new BaseApplicationViewData(application);
        }

        public static BaseApplicationViewData Create(Application application, ApplicationBanInfo appBan)
        {
            var model = new BaseApplicationViewData(application)
            {
                IsBannedForCurrentSchool = appBan?.BannedForCurrentSchool,
                IsBannedForDistrict = appBan?.UnBannedSchoolCount == 0,
                IsPartiallyBanned = appBan?.BannedSchoolCount != 0
            };
            return model;
        }

        public static IList<BaseApplicationViewData> Create(IList<Application> applications)
        {
            return applications.Select(Create).ToList();
        }

        public static IList<BaseApplicationViewData> Create(IList<Application> applications, IList<ApplicationBanInfo> appBans)
        {
            var res = applications.Select(x => Create(x, appBans.FirstOrDefault(y => x.Id == y.ApplicationId))).ToList();
            return res;
        }
    }

    public class ApplicationViewData : BaseApplicationViewData
    {
        public IList<ApplicationPersmissionsViewData> Permissions { get; set; }
        public IList<CategoryViewData> Categories { get; set; }
        public IList<int> GradeLevels { get; set; }
        public IList<RoleViewData> CanLaunchRoles { get; set; }
        public DeveloperViewData Developer { get; set; }
        public IList<Guid> Standards { get; set; } 

        protected ApplicationViewData(Application application, IList<Category> categories, bool canGetSecretKey, IList<Guid> standards)
            : base(application)
        {
            Developer = DeveloperViewData.Create(application.Developer);
            Permissions = ApplicationPersmissionsViewData.Create(application.Permissions);
            if (canGetSecretKey)
                SecretKey = application.SecretKey;
            categories = categories.Where(x => application.Categories.Any(y => y.CategoryRef == x.Id)).ToList();
            Categories = CategoryViewData.Create(categories);
            GradeLevels = application.GradeLevels.Select(x => x.GradeLevel).ToList();
            //if (application.ApplicationStandards != null && standards != null)
            //    Standards = CommonCoreStandardViewData.Create(
            //            standards.Where(x => application.ApplicationStandards.Any(y => y.StandardRef == x.Id))
            //                     .ToList());
        }
        public static ApplicationViewData Create(Application application, IList<Category> categories, IList<Guid> standards, bool canGetSecretKey = false)
        {
            return new ApplicationViewData(application, categories, canGetSecretKey, standards);
        }
        public static IList<ApplicationViewData> Create(IList<Application> applications, IList<CoreRole> roles, IList<Category> categories)
        {
            return applications.Select(x => Create(x, categories, null)).ToList();
        } 
    }
}