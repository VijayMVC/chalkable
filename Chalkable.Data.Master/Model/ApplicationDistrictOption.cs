using System;

namespace Chalkable.Data.Master.Model
{
    public class ApplicationDistrictOption
    {
        public const string DISTRICT_REF_FIELD = "DistrictRef";
        public const string BAN_FIELD = "Ban";
        public Guid ApplicationRef { get; set; }
	    public Guid DistrictRef { get; set; }
        public bool Ban { get; set; }
    }
}