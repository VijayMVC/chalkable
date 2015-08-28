using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StaffAdapter : SyncModelAdapter<Staff>
    {
        public StaffAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<Staff> entities)
        {
            var staff = entities.Select(x => new Data.School.Model.Staff
            {
                BirthDate = x.DateOfBirth,
                Id = x.StaffID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Gender = x.GenderID.HasValue ? Locator.GetnderMapping[x.GenderID.Value] : "U",
                UserId = x.UserID
            }).ToList();
            ServiceLocatorSchool.StaffService.Add(staff);
        }

        protected override void UpdateInternal(IList<Staff> entities)
        {
            var staff = entities.Select(x => new Data.School.Model.Staff
            {
                BirthDate = x.DateOfBirth,
                Id = x.StaffID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Gender = x.GenderID.HasValue ? Locator.GetnderMapping[x.GenderID.Value] : "U",
                UserId = x.UserID
            }).ToList();
            ServiceLocatorSchool.StaffService.Edit(staff);
        }

        protected override void DeleteInternal(IList<Staff> entities)
        {
            var staff = entities.Select(x => new Data.School.Model.Staff { Id = x.StaffID }).ToList();
            ServiceLocatorSchool.StaffService.Delete(staff);
        }
    }
}