using System.Collections.Generic;
using Chalkable.BusinessLogic.Model.PanoramaSettings;

namespace Chalkable.Web.Models.Settings
{
    public class BasePanoramaSettingsViewData
    {
        public IList<int> AcadYears { get; set; }
        public IList<StandardizedTestFilterViewData> StandardizedTestFilters { get; set; }

        protected BasePanoramaSettingsViewData(IList<int> academicYears, IList<StandardizedTestFilter> filters)
        {
            AcadYears = academicYears;
            StandardizedTestFilters = filters != null ? StandardizedTestFilterViewData.Create(filters) : null;
        }
    }
}