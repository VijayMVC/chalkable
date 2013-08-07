using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class AttendanceReasonViewData
    {
        public Guid Id { get; set; }
        public int AttendanceType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static AttendanceReasonViewData Create(AttendanceReason reason)
        {
            return new AttendanceReasonViewData
                {
                    Id = reason.Id,
                    AttendanceType = (int) reason.AttendanceType,
                    Code = reason.Code,
                    Description = reason.Description
                };
        }
        public static IList<AttendanceReasonViewData> Create(IList<AttendanceReason> reasons)
        {
            return reasons.Select(Create).ToList();
        }
    }
}