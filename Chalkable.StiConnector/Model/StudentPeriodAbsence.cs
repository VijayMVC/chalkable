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
    
    public partial class StudentPeriodAbsence
    {
        public int StudentPeriodAbsenceID { get; set; }
        public int AcadSessionID { get; set; }
        public int StudentID { get; set; }
        public System.DateTime Date { get; set; }
        public int TimeSlotID { get; set; }
        public int SectionID { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonID { get; set; }
        public decimal AbsenceValue { get; set; }
        public string AbsenceCategory { get; set; }
        public string Note { get; set; }
        public Nullable<int> OccurrenceID { get; set; }
        public bool IsSystemGenerated { get; set; }
        public System.Guid RowVersion { get; set; }
        public System.Guid DistrictGuid { get; set; }
    
        public virtual AbsenceReason AbsenceReason { get; set; }
        public virtual AcadSession AcadSession { get; set; }
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }
    }
}
