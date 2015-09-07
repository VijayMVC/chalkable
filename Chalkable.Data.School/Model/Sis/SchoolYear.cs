using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public  class SchoolYear
    {
        public const string ID_FIELD = "Id";
        public const string NAME_FIELD = "Name";
        public const string START_DATE_FIELD = "StartDate";
        public const string END_DATE_FIELD = "EndDate";
        public const string SCHOOL_REF_FIELD = "SchoolRef";
        public const string ARCHIVE_DATE = "ArchiveDate";


        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public int SchoolRef { get; set; }

        [NotDbFieldAttr]
        public bool IsActive
        {
            get { return !ArchiveDate.HasValue; }
        }
    }
}
