using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class MiniQuizAppInfoViewData
    {
        public BaseApplicationViewData ApplicationInfo { get; set; }
        public IList<InstalledApplicationViewData> InstalledApplications { get; set; }
        public IList<InstalledApplicationViewData> RecommendedApplications { get; set; }
        public string AuthorizationCode { get; set; }

        public static MiniQuizAppInfoViewData Create(Application miniQuizApp, IList<Application> suggestedApps, IList<ApplicationInstall> applicationsInstalls, IDictionary<Guid, bool> hasMyAppsDic, int? personId, string authorizationCode)
        {
            var res = new MiniQuizAppInfoViewData
                {
                    ApplicationInfo = BaseApplicationViewData.Create(miniQuizApp),
                    AuthorizationCode = authorizationCode
                };
            var apps = InstalledApplicationViewData.Create(applicationsInstalls, personId, suggestedApps, hasMyAppsDic);
            res.InstalledApplications = apps.Where(a => a.HasMyApp && a.Installed).ToList();
            res.RecommendedApplications = apps.Where(a => !a.Installed && a.HasMyApp).ToList();
            return res;
        }
    }
}