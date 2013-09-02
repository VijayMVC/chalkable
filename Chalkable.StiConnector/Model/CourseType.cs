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
    
    public partial class CourseType
    {
        public CourseType()
        {
            this.Courses = new HashSet<Course>();
        }
    
        public short CourseTypeID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public System.Guid RowVersion { get; set; }
        public System.Guid DistrictGuid { get; set; }
    
        public virtual ICollection<Course> Courses { get; set; }
    }
}
