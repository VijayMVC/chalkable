using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StaffSchoolAdapter : SyncModelAdapter<StaffSchool>
    {
        public StaffSchoolAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.StaffSchool Selector(StaffSchool x)
        {
            return new Data.School.Model.StaffSchool
            {
                SchoolRef = x.SchoolID,
                StaffRef = x.StaffID
            };
        }

        protected override void InsertInternal(IList<StaffSchool> entities)
        {
            var staffSchool = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StaffService.AddStaffSchools(staffSchool);
        }

        protected override void UpdateInternal(IList<StaffSchool> entities)
        {
            //Nothing here
        }

        protected override void DeleteInternal(IList<StaffSchool> entities)
        {
            var ss = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StaffService.DeleteStaffSchools(ss);
        }

        protected override void PrepareToDeleteInternal(IList<StaffSchool> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}