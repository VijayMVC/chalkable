using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class AttendanceReasonViewData
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        protected AttendanceReasonViewData(AttendanceReason reason)
        {
            Id = reason.Id;
            Name = reason.Name;
            Code = reason.Code;
            Description = reason.Description;
        }

        public static AttendanceReasonViewData Create(AttendanceReason reason)
        {
            return new AttendanceReasonViewData(reason);
        }
        public static IList<AttendanceReasonViewData> Create(IList<AttendanceReason> reasons)
        {
            return reasons.Select(Create).ToList();
        }
    }

    public class AttendanceReasonDetailsViewData : AttendanceReasonViewData
    {
        public IList<AttendanceLevelReasonViewData> AttendanceLevelReason { get; set; }
       
        protected AttendanceReasonDetailsViewData(AttendanceReason reason) : base(reason)
        {
            if(reason.AttendanceLevelReasons != null)
                AttendanceLevelReason = reason.AttendanceLevelReasons.Select(AttendanceLevelReasonViewData.Create).ToList();
        }
        public static new IList<AttendanceReasonDetailsViewData> Create(IList<AttendanceReason> reasons)
        {
            return reasons.Select(x => new AttendanceReasonDetailsViewData(x)).ToList();
        }
    }

    public class AttendanceLevelReasonViewData
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public int AttendanceReasonId { get; set; }
        public bool IsDefault { get; set; }

        public static AttendanceLevelReasonViewData Create(AttendanceLevelReason attendanceLevelReason)
        {
            return new AttendanceLevelReasonViewData
                {
                    Id = attendanceLevelReason.Id,
                    Level = attendanceLevelReason.Level,
                    AttendanceReasonId = attendanceLevelReason.AttendanceReasonRef,
                    IsDefault = attendanceLevelReason.IsDefault
                };
        }
    }
}