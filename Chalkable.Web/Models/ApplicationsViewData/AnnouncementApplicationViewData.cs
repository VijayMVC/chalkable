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
    public class AnnouncementApplicationViewData
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid AnnouncementId { get; set; }
        public Guid? CurrentPersonId { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string ViewUrl { get; set; }
        public string EditUrl { get; set; }
        public string GradingViewUrl { get; set; }
        public BaseApplicationViewData Application { get; set; }
        public int Order { get; set; }
        public bool IsInstalledForMe { get; set; }

        public static AnnouncementApplicationViewData Create(AnnouncementApplication announcementApplication, 
            Application application, IList<ApplicationInstall> installs, Guid? currentPersonId)
        {
            var res = new AnnouncementApplicationViewData
                {
                    Id = announcementApplication.Id,
                    Active = announcementApplication.Active,
                    AnnouncementId = announcementApplication.AnnouncementRef,
                    ApplicationId = announcementApplication.ApplicationRef,
                    EditUrl = AppTools.BuildAppUrl(application, null, installs.First().Id, AppMode.Edit),
                    ViewUrl = AppTools.BuildAppUrl(application, null, installs.First().Id, AppMode.View),
                    GradingViewUrl = AppTools.BuildAppUrl(application, null, installs.First().Id, AppMode.GradingView),
                    Application = BaseApplicationViewData.Create(application),
                    CurrentPersonId = currentPersonId,
                };
            res.IsInstalledForMe = installs != null && currentPersonId.HasValue &&
                                   installs.Any(x => x.OwnerRef == currentPersonId && x.Active);
            return res;
        }
        
    }
}