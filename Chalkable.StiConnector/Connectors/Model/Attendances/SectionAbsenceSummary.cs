namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class SectionAbsenceSummary
    {
        public decimal? Absences { get; set; }
        public decimal? Presents { get; set; }
        public int SectionId { get; set; }
        public int? Tardies { get; set; }
    }
}
