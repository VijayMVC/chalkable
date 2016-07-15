using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class UserSchoolAdapter : SyncModelAdapter<UserSchool>
    {
        public UserSchoolAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.UserSchool SchoolSelector(UserSchool x)
        {
            return new Data.School.Model.UserSchool
            {
                SchoolRef = x.SchoolID,
                UserRef = x.UserID
            };
        }

        private SchoolUser MasterSelector(UserSchool x)
        {
            Trace.Assert(ServiceLocatorSchool.Context.DistrictId.HasValue);
            return new SchoolUser
            {
                DistrictRef = ServiceLocatorSchool.Context.DistrictId.Value,
                SchoolRef = x.SchoolID,
                UserRef = x.UserID
            };
        }

        protected override void InsertInternal(IList<UserSchool> entities)
        {
            var masterUserSchool = entities.Select(MasterSelector).ToList();
            ServiceLocatorMaster.UserService.AddSchoolUsers(masterUserSchool);
            var districtUserSchool = entities.Select(SchoolSelector).ToList();
            ServiceLocatorSchool.UserSchoolService.Add(districtUserSchool);
        }

        protected override void UpdateInternal(IList<UserSchool> entities)
        {
            //Nothing here
        }

        protected override void DeleteInternal(IList<UserSchool> entities)
        {
            var masterSchoolUsers = entities.Select(MasterSelector).ToList();
            ServiceLocatorMaster.UserService.DeleteSchoolUsers(masterSchoolUsers);
            var districtUserSchool = entities.Select(SchoolSelector).ToList();
            ServiceLocatorSchool.UserSchoolService.Delete(districtUserSchool);
        }
    }
}