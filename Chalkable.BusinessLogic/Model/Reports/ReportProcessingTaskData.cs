using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class ReportProcessingTaskData
    {
        public Guid DistrictId { get; set; }
        public string ContentUrl { get; set; }
        public ReportCardsInputModel ReportInputModel { get; set; }
        public UserContext UserContext { get; set; }

        public ReportProcessingTaskData()
        {
        }
        public ReportProcessingTaskData(UserContext userContext,
            ReportCardsInputModel reportInput, string contentUrl)
        {
            DistrictId = userContext.DistrictId.Value;
            UserContext = userContext;
            ReportInputModel = reportInput;
            ContentUrl = contentUrl;
        }

        public ReportProcessingTaskData(string str)
        {
            var o = JsonConvert.DeserializeObject<ReportProcessingTaskData>(str);
            DistrictId = o.DistrictId;
            ReportInputModel = o.ReportInputModel;
            UserContext = o.UserContext;
            UserContext.Role = CoreRoles.GetById(UserContext.RoleId);
            ContentUrl = o.ContentUrl;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
