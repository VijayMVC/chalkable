using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class MiniQuizAppInfoViewData
    {
        public BaseApplicationViewData ApplicationInfo { get; set; }
        public IList<BaseApplicationViewData> Applications { get; set; }
        public IList<BaseApplicationViewData> RecommendedApplications { get; set; }
        public string Token { get; set; }

        public static MiniQuizAppInfoViewData Create(Application miniQuizApp, IList<BaseApplicationViewData> suggestedApps, IList<BaseApplicationViewData> application, IDictionary<Guid, bool> hasMyAppsDic, int? personId, string token)
        {
            var res = new MiniQuizAppInfoViewData
            {
                ApplicationInfo = BaseApplicationViewData.Create(miniQuizApp),
                Token = token,
                Applications = application.Where(a => hasMyAppsDic.ContainsKey(a.Id)).ToList(),
                RecommendedApplications = suggestedApps.Where(a => hasMyAppsDic.ContainsKey(a.Id)).ToList()
            };

            return res;
        }
    }
}