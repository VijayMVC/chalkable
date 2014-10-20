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
            return applications.Select(x => Create(x, notInstalledStCountPerApp[x.Id])).ToList();
        } 
    }

    public class InstalledApplicationViewData : BaseApplicationViewData
    {
        public bool HasMyApp { get; set; }
        public IList<ApplicationInstallViewData> ApplicationInstalls { get; set; }

        protected InstalledApplicationViewData(Application application) : base(application)
        {
        }

        public static IList<InstalledApplicationViewData> Create(IList<ApplicationInstall> installedApp, int? personId,
                                                         IList<Application> applications, IDictionary<Guid, bool> hasMyAppsDic = null)
        {
            var res = new List<InstalledApplicationViewData>();
            var dicApp = installedApp.GroupBy(x => x.ApplicationRef).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var appKey in dicApp.Keys)
            {
                var appInstalls = dicApp[appKey];
                var app = applications.FirstOrDefault(x => x.Id == appKey);
                if (app != null)
                    res.Add(new InstalledApplicationViewData(app)
                        {
                            ApplicationInstalls = ApplicationInstallViewData.Create(appInstalls, personId),
                            HasMyApp = hasMyAppsDic != null && hasMyAppsDic.ContainsKey(appKey) && hasMyAppsDic[appKey],
                            MyAppsUrl = AppTools.BuildAppUrl(app, null, appInstalls.First().Id, AppMode.MyView)
                        });
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