using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class ReportProcessingTaskData
    {
        public Guid DistrictId { get; set; }
        //public int ToPersonId { get; set; }
        //public int ToRoleId { get; set; }
        //public int SchoolId { get; set; }
        //public int SchoolYearId { get; set; }
        public string ContentUrl { get; set; }
        public ReportCardsInputModel ReportInputModel { get; set; }

        //public string UserContextStr { get; private set; }
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
            //ToPersonId = toPersonId;
            //ToRoleId = toRoleId;
            //SchoolId = schoolId;
            //SchoolYearId = schoolYearId;
            ContentUrl = contentUrl;
        }

        public ReportProcessingTaskData(string str)
        {
            var o = JsonConvert.DeserializeObject<ReportProcessingTaskData>(str);
            DistrictId = o.DistrictId;
            ReportInputModel = o.ReportInputModel;
            //ToPersonId = o.ToPersonId;
            //ToRoleId = o.ToRoleId;
            //SchoolId = o.SchoolId;
            //SchoolYearId = o.SchoolYearId;
            //UserContextStr = o.UserContextStr;
            //UserContext userContext;
            //UserContext.TryConvertFromString(UserContextStr, out userContext);
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
