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

        protected override void InsertInternal(IList<UserSchool> entities)
        {
            Trace.Assert(ServiceLocatorSchool.Context.DistrictId.HasValue);
            var masterUserSchool = entities.Select(x => new SchoolUser
            {
                DistrictRef = ServiceLocatorSchool.Context.DistrictId.Value,
                SchoolRef = x.SchoolID,
                UserRef = x.UserID
            }).ToList();
            ServiceLocatorMaster.UserService.AddSchoolUsers(masterUserSchool);
            var districtUserSchool = entities.Select(x => new Data.School.Model.UserSchool
            {
                SchoolRef = x.SchoolID,
                UserRef = x.UserID
            }).ToList();
            ServiceLocatorSchool.UserSchoolService.Add(districtUserSchool);
        }

        protected override void UpdateInternal(IList<UserSchool> entities)
        {
            //Nothing here
        }

        protected override void DeleteInternal(IList<UserSchool> entities)
        {
            Trace.Assert(ServiceLocatorSchool.Context.DistrictId.HasValue);
            var masterSchoolUsers = entities.Select(x => new SchoolUser
            {
                SchoolRef = x.SchoolID,
                UserRef = x.UserID,
                DistrictRef = ServiceLocatorSchool.Context.DistrictId.Value
            }).ToList();
            ServiceLocatorMaster.UserService.DeleteSchoolUsers(masterSchoolUsers);

            var districtUserSchool = entities.Select(x => new Data.School.Model.UserSchool
            {
                SchoolRef = x.SchoolID,
                UserRef = x.UserID
            }).ToList();
            ServiceLocatorSchool.UserSchoolService.Delete(districtUserSchool);
        }
    }
}