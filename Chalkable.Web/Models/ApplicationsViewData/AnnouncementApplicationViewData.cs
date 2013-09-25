using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class AnnouncementApplicationViewData : BaseApplicationViewData
    {
        public Guid AnnouncementApplicationId { get; set; }
        public Guid AnnouncementId { get; set; }
        public Guid? CurrentPersonId { get; set; }
        public bool Active { get; set; }
        public string ViewUrl { get; set; }
        public string EditUrl { get; set; }
        public string GradingViewUrl { get; set; }
        public int Order { get; set; }
        public bool IsInstalledForMe { get; set; }

        protected AnnouncementApplicationViewData(Application application) : base(application)
        {
        }

        public static AnnouncementApplicationViewData Create(AnnouncementApplication announcementApplication, 
            Application application, IList<ApplicationInstall> installs, Guid? currentPersonId)
        {
            var res = new AnnouncementApplicationViewData(application)
                {
                    AnnouncementApplicationId = announcementApplication.Id,
                    Active = announcementApplication.Active,
                    AnnouncementId = announcementApplication.AnnouncementRef,
                    EditUrl = AppTools.BuildAppUrl(application, announcementApplication.Id, installs.First().Id, AppMode.Edit),
                    ViewUrl = AppTools.BuildAppUrl(application, announcementApplication.Id, installs.First().Id, AppMode.View),
                    GradingViewUrl = AppTools.BuildAppUrl(application, announcementApplication.Id, installs.First().Id, AppMode.GradingView),
                    CurrentPersonId = currentPersonId,
                    Order = announcementApplication.Order
                };
            res.IsInstalledForMe = installs != null && currentPersonId.HasValue &&
                                   installs.Any(x => x.OwnerRef == currentPersonId && x.Active);
            return res;
        }

        public static IList<AnnouncementApplicationViewData> Create(IList<AnnouncementApplication> annApps
            , IList<Application> applications, IList<ApplicationInstall> installs, Guid? currentPersonId)
        {
            var res = new List<AnnouncementApplicationViewData>();
            foreach (var annApp in annApps)
            {
                var app = applications.First(x => x.Id == annApp.ApplicationRef);
                res.Add(Create(annApp, app, installs, currentPersonId));
            }
            return res;
        }
        
    }
}