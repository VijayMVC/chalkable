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
        public ApplicationAccessViewData ApplicationAccess { get; set; }
        public ApplicationPriceViewData ApplicationPrice { get; set; }
        public IList<Guid> Picturesid { get; set; }
       

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
            Picturesid = application.Pictures.Select(x => x.Id).ToList();
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
        public BaseApplicationViewData LiveApplication { get; set; }
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
                        standards.Where(x => application.ApplicationStandards.Any(y => y.StandardCode == x.Code))
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

        protected ApplicationDetailsViewData(Application application,  IList<Category> categories, bool canGetSecretKey) 
            : base(application, categories, canGetSecretKey, null)
        {
        }
        public static ApplicationDetailsViewData Create(Application application, IList<CoreRole> roles, IList<Category> categories
            , IList<ApplicationRating> appRatings, IList<Person> persons)
        {
            var res = new ApplicationDetailsViewData(application, categories, false);
            res.ApplicationRating = ApplicationRatingViewData.Create(appRatings, persons);

            return res;
        }
    }

    public class InstalledForPersonsGroupViewData
    {
        public enum GroupTypeEnum
        {
            Class = 1,
            GradeLevel,
            Department,
            Role,
            All,
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
}