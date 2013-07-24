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
        public bool HasStudentMyApps { get; set; }
        public bool HasTeacherMyApps { get; set; }
        public bool HasAdminMyApps { get; set; }
        public bool HasParentMyApps { get; set; }
        public bool HasMyAppsView { get; set; }
        public bool CanAttach { get; set; }
        public bool ShowInGradeView { get; set; }
        public bool IsInternal { get; set; }
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
            ApplicationPrice = ApplicationPriceViewData.Create(application);
            HasAdminMyApps = application.HasAdminMyApps;
            HasStudentMyApps = application.HasStudentMyApps;
            HasTeacherMyApps = application.HasTeacherMyApps;
            HasParentMyApps = application.HasParentMyApps;
            CanAttach = application.CanAttach;
            ShowInGradeView = application.ShowInGradeView;
            LiveAppId = application.OriginalRef;
            IsInternal = application.IsInternal;
            Picturesid = application.Pictures.Select(x => x.Id).ToList();
    
        }

        public static IList<BaseApplicationViewData> Create(IList<Application> applications)
        {
            return applications.Select(x=>new BaseApplicationViewData(x)).ToList();

        }
    }


    public class ApplicationViewData : BaseApplicationViewData
    {
        public IList<ApplicationPersmissionsViewData> Permissions { get; set; }
        //public ApplicationRatingViewData ApplicationRating { get; set; }
        public IList<CategoryViewData> Categories { get; set; }
        public IList<GradeLevelViewData> GradelLevels { get; set; }
        public IList<RoleViewData> CanLaunchRoles { get; set; }
        public DeveloperViewData Developer { get; set; }
        public BaseApplicationViewData LiveApplication { get; set; }

        protected ApplicationViewData(Application application): base(application)
        {
            Developer = DeveloperViewData.Create(application.Developer);
            Permissions = ApplicationPersmissionsViewData.Create(application.Permissions);
        }

        public static ApplicationViewData Create(Application application, IList<CoreRole> roles, IList<Category> categories)
        {
            var res = new ApplicationViewData(application);
            categories = categories.Where(x => application.Categories.Any(y => y.CategoryRef == x.Id)).ToList();
            res.Categories = CategoryViewData.Create(categories);
            return res;
        }

        public static IList<ApplicationViewData> Create(IList<Application> applications, IList<CoreRole> roles, IList<Category> categories)
        {
            return applications.Select(x => Create(x, roles, categories)).ToList();
        } 
    }
}