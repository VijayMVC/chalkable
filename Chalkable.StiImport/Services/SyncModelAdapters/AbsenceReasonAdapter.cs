using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class AbsenceReasonAdapter : SyncModelAdapter<AbsenceReason>
    {
        public AbsenceReasonAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<AbsenceReason> entities)
        {
            var rs = entities.Select(x => new AttendanceReason
            {
                Category = x.AbsenceCategory,
                Code = x.Code,
                Description = x.Description,
                Id = x.AbsenceReasonID,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name
            }).ToList();
            ServiceLocatorSchool.AttendanceReasonService.Add(rs);
        }

        protected override void UpdateInternal(IList<AbsenceReason> entities)
        {
            var rs = entities.Select(x => new AttendanceReason
            {
                Category = x.AbsenceCategory,
                Code = x.Code,
                Description = x.Description,
                Id = x.AbsenceReasonID,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name
            }).ToList();
            ServiceLocatorSchool.AttendanceReasonService.Edit(rs);
        }

        protected override void DeleteInternal(IList<AbsenceReason> entities)
        {
            var ids = entities.Select(x => (int)x.AbsenceReasonID).ToList();
            ServiceLocatorSchool.AttendanceReasonService.Delete(ids);
        }
    }
}