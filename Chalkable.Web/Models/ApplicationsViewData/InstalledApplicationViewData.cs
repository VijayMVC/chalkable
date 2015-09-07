using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.Web.Models.ApplicationsViewData
{

    public class ApplicationForAttachViewData : BaseApplicationViewData
    {
        public int NotInstalledStudentsCount { get; set; }
        protected ApplicationForAttachViewData(Application application) : base(application)
        {
        }
        
        public static ApplicationForAttachViewData Create(Application application, int notInstalledStudentsCount)
        {
            return new ApplicationForAttachViewData(application) {NotInstalledStudentsCount = notInstalledStudentsCount};
        }
        public static IList<ApplicationForAttachViewData> Create(IList<Application> applications, IDictionary<Guid, int> notInstalledStCountPerApp)
        {
            return applications.Select(app => Create(app, notInstalledStCountPerApp[app.Id])).ToList();
        } 
    }

    public class InstalledApplicationViewData : BaseApplicationViewData
    {
        public bool HasMyApp { get; set; }
        public bool Installed { get; set; }
        public IList<ApplicationInstallViewData> ApplicationInstalls { get; set; }

        protected InstalledApplicationViewData(Application application) : base(application)
        {
        }

        public static IList<InstalledApplicationViewData> Create(IList<ApplicationInstall> installedApp, int? personId,
                                                         IList<Application> applications, IDictionary<Guid, bool> hasMyAppsDic = null)
        {
            var res = new List<InstalledApplicationViewData>();
            foreach (var app in applications)
            {
                var hasMyApps = hasMyAppsDic != null && hasMyAppsDic.ContainsKey(app.Id) && hasMyAppsDic[app.Id];
                var item = Create(installedApp, personId, app, hasMyApps);
                res.Add(item);
            }
            return res;
        }

        public static InstalledApplicationViewData Create(IList<ApplicationInstall> installedApp, int? personId, Application app, bool hasMyApps)
        {
            var res = new InstalledApplicationViewData(app)
                {
                    HasMyApp = hasMyApps,
                };
            var appInstalls = installedApp.Where(x => x.ApplicationRef == app.Id).ToList();
            res.ApplicationInstalls = ApplicationInstallViewData.Create(appInstalls, personId);
            if (appInstalls.Count > 0)
            {
                res.MyAppsUrl = AppTools.BuildAppUrl(app, null, appInstalls.First().Id, AppMode.MyView);
                res.Installed = res.ApplicationInstalls.Any(x => personId.HasValue && x.PersonId == personId.Value);
            }
            return res;
        }

        public enum InstallationType
        {
            Owner = 1,
            Personal = 2,
        }

    }

    public class ApplicationInstallViewData
    {
        public int ApplicationInstallId { get; set; }
        public int InstallationOwnerId { get; set; }
        public int PersonId { get; set; }
        public bool IsOwner { get; set; }

        public static ApplicationInstallViewData Create(ApplicationInstall applicationInstall, int? personId)
        {
            return new ApplicationInstallViewData
            {
                ApplicationInstallId = applicationInstall.Id,
                InstallationOwnerId = applicationInstall.OwnerRef,
                PersonId = applicationInstall.PersonRef,
                IsOwner = personId.HasValue && personId.Value == applicationInstall.OwnerRef,
            };
        }
        public static IList<ApplicationInstallViewData> Create(IList<ApplicationInstall> applicationInstalls, int? personId)
        {
            return applicationInstalls.Select(x => Create(x, personId)).ToList();
        }
    }
}