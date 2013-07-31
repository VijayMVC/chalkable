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
    public class InstalledApplicationViewData : BaseApplicationViewData
    {
        public IList<ApplicationInstallViewData> ApplicationInstalls { get; set; }

        protected InstalledApplicationViewData(Application application) : base(application)
        {
        }

        public static IList<InstalledApplicationViewData> Create(IList<ApplicationInstall> installedApp, Person person,
                                                         IList<Application> applications, IDictionary<int, bool> hasMyAppsDic = null)
        {
            var res = new List<InstalledApplicationViewData>();
            var dicApp = installedApp.GroupBy(x => x.ApplicationRef).ToDictionary(x => x.Key, x => x.ToList());
            var personId = person != null ? person.Id : default(Guid?);
            foreach (var appKey in dicApp.Keys)
            {
                var appInstalls = dicApp[appKey];
                var app = applications.First(x => x.Id == appKey);
                res.Add(new InstalledApplicationViewData(app)
                    {
                        ApplicationInstalls = ApplicationInstallViewData.Create(appInstalls, personId),
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
        public Guid ApplicationInstallId { get; set; }
        public Guid InstallationOwnerId { get; set; }
        public Guid PersonId { get; set; }
        public bool IsOwner { get; set; }

        public static ApplicationInstallViewData Create(ApplicationInstall applicationInstall, Guid? personId)
        {
            return new ApplicationInstallViewData
            {
                ApplicationInstallId = applicationInstall.Id,
                InstallationOwnerId = applicationInstall.OwnerRef,
                PersonId = applicationInstall.OwnerRef,
                IsOwner = personId.HasValue && personId.Value == applicationInstall.OwnerRef,
            };
        }
        public static IList<ApplicationInstallViewData> Create(IList<ApplicationInstall> applicationInstalls, Guid? personId)
        {
            return applicationInstalls.Select(x => Create(x, personId)).ToList();
        }
    }
}