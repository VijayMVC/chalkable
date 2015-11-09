using System;
using System.Collections.Generic;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models
{
    public class AttachSettingsViewData
    {
        public Guid? AssessmentApplicationId { get; set; }
        public bool IsStandardEnabled { get; set; }
        public bool IsAppsEnabled { get; set; }
        public bool IsFileCabinetEnabled { get; set; }
        public IList<BaseApplicationViewData> ExternalAttachApps { get; set; }
        
        public static AttachSettingsViewData Create(Guid? assessmentId, bool isStandardEnabled, bool isAppsEnabled
            , bool isFileCabinetEnabled, IList<Application> externalAttachApps)
        {
            return new AttachSettingsViewData
            {
                AssessmentApplicationId = assessmentId,
                IsStandardEnabled = isStandardEnabled,
                IsAppsEnabled = isAppsEnabled,
                IsFileCabinetEnabled = isFileCabinetEnabled,
                ExternalAttachApps = BaseApplicationViewData.Create(externalAttachApps) 
            };
        }
    }
}