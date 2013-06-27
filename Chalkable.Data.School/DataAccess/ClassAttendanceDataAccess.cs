using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassAttendanceDataAccess : DataAccessBase<ClassAttendance>
    {
        public ClassAttendanceDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

    }

    public class ClassAttendanceQuery
    {
        public Guid? Id { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? SchoolYearId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public AttendanceTypeEnum? Type { get; set; }
        public int? FromTime { get; set; }
        public int? ToTime { get; set; }
        public bool NeedAllData { get; set; }
    }
}
