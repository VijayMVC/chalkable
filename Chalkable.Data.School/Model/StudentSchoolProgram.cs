using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentSchoolProgram
    {
        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AcadSessionId { get; set; }
        public int SchoolProgramId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? DecimalValue { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
