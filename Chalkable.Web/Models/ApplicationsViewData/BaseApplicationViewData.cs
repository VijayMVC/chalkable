using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

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
        public string MyAppsUrl { get; set; }
        public string SecretKey { get; set; }
        public int State { get; set; }
        public Guid? DeveloperId { get; set; }
        public Guid? LiveAppId { get; set; }
        public bool IsInternal { get; set; }
        public bool IsAdvanced { get; set; }
        public int? InternalScore { get; set; }
        public string InternalDescription { get; set; }
        public bool? Ban { get; set; }
        public ApplicationAccessViewData ApplicationAccess { get; set; }
        public ApplicationPriceViewData ApplicationPrice { get; set; }
        public IList<Guid> Picturesid { get; set; }
        public BaseApplicationViewData LiveApplication { get; set; }
        
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
            ApplicationAccess = ApplicationAccessViewData.Create(application);
            ApplicationPrice = ApplicationPriceViewData.Create(application);
            LiveAppId = application.OriginalRef;
            IsInternal = application.IsInternal;
            IsAdvanced = application.IsAdvanced;
            InternalScore = application.InternalScore;
            InternalDescription = application.InternalDescription;
            Picturesid = application.Pictures.Select(x => x.Id).ToList();
            if (application.LiveApplication != null)
                LiveApplication = Create(application.LiveApplication);
            Ban = application.Ban;
        }

        public static BaseApplicationViewData Create(Application application)
        {
            return new BaseApplicationViewData(application);
        }

        public static IList<BaseApplicationViewData> Create(IList<Application> applications)
        {
            return applications.Select(Create).ToList();
        }
    }

    public class ApplicationViewData : BaseApplicationViewData
    {
        public IList<ApplicationPersmissionsViewData> Permissions { get; set; }
        public IList<CategoryViewData> Categories { get; set; }
        public IList<int> GradeLevels { get; set; }
        public IList<RoleViewData> CanLaunchRoles { get; set; }
        public DeveloperViewData Developer { get; set; }
        public IList<CommonCoreStandardViewData> Standards { get; set; } 

        protected ApplicationViewData(Application application, IList<Category> categories, bool canGetSecretKey, IList<CommonCoreStandard> standards)
            : base(application)
        {
            Developer = DeveloperViewData.Create(application.Developer);
            Permissions = ApplicationPersmissionsViewData.Create(application.Permissions);
            if (canGetSecretKey)
                SecretKey = application.SecretKey;
            categories = categories.Where(x => application.Categories.Any(y => y.CategoryRef == x.Id)).ToList();
            Categories = CategoryViewData.Create(categories);
            GradeLevels = application.GradeLevels.Select(x => x.GradeLevel).ToList();
            if (application.ApplicationStandards != null && standards != null)
                Standards = CommonCoreStandardViewData.Create(
                        standards.Where(x => application.ApplicationStandards.Any(y => y.StandardRef == x.Id))
                                 .ToList());
        }
        public static ApplicationViewData Create(Application application, IList<Category> categories, IList<CommonCoreStandard> standards, bool canGetSecretKey = false)
        {
            return new ApplicationViewData(application, categories, canGetSecretKey, standards);
        }
        public static IList<ApplicationViewData> Create(IList<Application> applications, IList<CoreRole> roles, IList<Category> categories)
        {
            return applications.Select(x => Create(x, categories, null)).ToList();
        } 
    }

    public class ApplicationDetailsViewData : ApplicationViewData
    {
        public ApplicationRatingViewData ApplicationRating { get; set; }
        public bool IsInstalledOnlyForMe { get; set; }
        public IList<InstalledForPersonsGroupViewData> InstalledForPersonsGroup { get; set; } 
        public IList<ApplicationInstallHistoryViewData> ApplicationInstallHistory { get; set; }

        protected ApplicationDetailsViewData(Application application,  IList<Category> categories, bool canGetSecretKey) 
            : base(application, categories, canGetSecretKey, null)
        {
        }
        public static ApplicationDetailsViewData Create(Application application, IList<CoreRole> roles, IList<Category> categories, IList<ApplicationRating> appRatings
            , IList<ApplicationInstallHistory> applicationInstallHistory)
        {
            var res = new ApplicationDetailsViewData(application, categories, false)
                {
                    ApplicationRating = ApplicationRatingViewData.Create(appRatings),
                };
            if (applicationInstallHistory != null)
                res.ApplicationInstallHistory = ApplicationInstallHistoryViewData.Create(applicationInstallHistory);
            return res;
        }
    }

    public class InstalledForPersonsGroupViewData
    {
        public enum GroupTypeEnum
        {
            All = 0,
            Class = 1,
            CurrentUser = 6
        }
        public bool IsInstalled { get; set; }
        public GroupTypeEnum GroupType { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public static InstalledForPersonsGroupViewData Create(GroupTypeEnum groupType, string id, string description, bool isInstalled)
        {
            return new InstalledForPersonsGroupViewData
            {
                Description = description,
                IsInstalled = isInstalled,
                GroupType = groupType,
                Id = id
            };
        }
    }

    public class ApplicationInstallHistoryViewData
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OwnerRoleId { get; set; }
        public DateTime InstallDate { get; set; }
        public int InstalledCount { get; set; }
        public decimal Price { get; set; }
        public decimal Remains { get; set; }

        public static IList<ApplicationInstallHistoryViewData> Create(IList<ApplicationInstallHistory> history)
        {
            return history.Select(x => new ApplicationInstallHistoryViewData
            {
                FirstName = x.FirstName,
                InstalledCount = x.InstalledCount,
                LastName = x.LastName,
                PersonId = x.PersonId,
                SchoolId = x.SchoolId,
                SchoolName = x.SchoolName,
                OwnerRoleId = x.OwnerRoleId,
                InstallDate = x.InstallDate
            }).ToList();
        }
    }
}