//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chalkable.StiConnector.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class StandardSubject
    {
        public StandardSubject()
        {
            this.Standards = new HashSet<Standard>();
        }
    
        public int StandardSubjectID { get; set; }
        public Nullable<int> AssessmentSubjectID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<short> AdoptionYear { get; set; }
        public bool IsActive { get; set; }
        public System.Guid RowVersion { get; set; }
        public System.Guid DistrictGuid { get; set; }
    
        public virtual ICollection<Standard> Standards { get; set; }
    }
}
