﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;
using Chalkable.Data.School.Model.Chlk;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class AnnouncementApplicationViewData : BaseApplicationViewData
    {
        public int AnnouncementApplicationId { get; set; }
        public int AnnouncementId { get; set; }
        public int? CurrentPersonId { get; set; }
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
            Application application, IList<ApplicationInstall> installs, int? currentPersonId)
        {
            //Todo: think how to build urls for annAppViewData if there are no applicationInstalls
            var appInstallId = installs != null && installs.Any() ? installs.First().Id : (int?)null;
            var res = new AnnouncementApplicationViewData(application)
                {
                    AnnouncementApplicationId = announcementApplication.Id,
                    Active = announcementApplication.Active,
                    AnnouncementId = announcementApplication.AnnouncementRef,
                    EditUrl = AppTools.BuildAppUrl(application, announcementApplication.Id, appInstallId, AppMode.Edit),
                    ViewUrl = AppTools.BuildAppUrl(application, announcementApplication.Id, appInstallId, AppMode.View),
                    GradingViewUrl = AppTools.BuildAppUrl(application, announcementApplication.Id, appInstallId, AppMode.GradingView),
                    CurrentPersonId = currentPersonId,
                    Order = announcementApplication.Order,
                    IsInstalledForMe = installs != null && currentPersonId.HasValue &&
                                       installs.Any(x => x.OwnerRef == currentPersonId && x.Active)
                };
            return res;
        }

        public static IList<AnnouncementApplicationViewData> Create(IList<AnnouncementApplication> annApps
            , IList<Application> applications, IList<ApplicationInstall> installs, int? currentPersonId)
        {
            var res = new List<AnnouncementApplicationViewData>();
            foreach (var annApp in annApps)
            {
                var app = applications.FirstOrDefault(x => x.Id == annApp.ApplicationRef);
                if (app != null)
                {
                    var currentAppInstalls = installs.Where(x => x.ApplicationRef == app.Id).ToList();
                    res.Add(Create(annApp, app, currentAppInstalls, currentPersonId));    
                }
            }
            return res;
        }
        
    }
}