using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public  class SchoolYear
    {
        public const string ID_FIELD = nameof(Id);
        public const string NAME_FIELD = nameof(Name);
        public const string START_DATE_FIELD = nameof(StartDate);
        public const string END_DATE_FIELD = nameof(EndDate);
        public const string SCHOOL_REF_FIELD = nameof(SchoolRef);
        public const string ARCHIVE_DATE = nameof(ArchiveDate);
        public const string ACAD_YEAR_FIELD = nameof(AcadYear);

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public int SchoolRef { get; set; }
        public int AcadYear { get; set; }

        [NotDbFieldAttr]
        public bool IsActive => !ArchiveDate.HasValue;
    }
}
