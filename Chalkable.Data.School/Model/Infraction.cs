using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Infraction
    {
        public const string IS_ACTIVE_FIELD = "IsActive";
        public const string VISIBLE_IN_CLASSROOM_FIELD = "VisibleInClassroom";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
	    public string Description { get; set; }
	    public byte Demerits { get; set; }
	    public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }        
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public bool VisibleInClassroom { get; set; }
    }

    public class DisciplineTotalPerType
    {
        public Guid PersonId { get; set; }
        public int Total { get; set; }
        [DataEntityAttr]
        public Infraction DisciplineType { get; set; }
    }
}
