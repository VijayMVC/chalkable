using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class AbsenceLevelReasonAdapter : SyncModelAdapter<AbsenceLevelReason>
    {
        public AbsenceLevelReasonAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private AttendanceLevelReason Selector(AbsenceLevelReason x)
        {
            return new AttendanceLevelReason
            {
                Id = x.AbsenceLevelReasonID,
                AttendanceReasonRef = x.AbsenceReasonID,
                IsDefault = x.IsDefaultReason,
                Level = x.AbsenceLevel
            };
        }

        protected override void InsertInternal(IList<AbsenceLevelReason> entities)
        {
            var absenceLevelReasons = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AttendanceReasonService.AddAttendanceLevelReasons(absenceLevelReasons);
        }

        protected override void UpdateInternal(IList<AbsenceLevelReason> entities)
        {
            var absenceLevelReasons = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AttendanceReasonService.EditAttendanceLevelReasons(absenceLevelReasons);
        }

        protected override void DeleteInternal(IList<AbsenceLevelReason> entities)
        {
            var ids = entities.Select(x => x.AbsenceLevelReasonID).ToList();
            ServiceLocatorSchool.AttendanceReasonService.DeleteAttendanceLevelReasons(ids);
        }

        protected override void PrepareToDeleteInternal(IList<AbsenceLevelReason> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}