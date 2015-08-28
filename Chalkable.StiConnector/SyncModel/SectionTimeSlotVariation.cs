namespace Chalkable.StiConnector.SyncModel
{
    public class SectionTimeSlotVariation : SyncModel
    {
        public int SectionID { get; set; }
        public int TimeSlotVariationID { get; set; }
        public override int DefaultOrder => 33;
    }
}