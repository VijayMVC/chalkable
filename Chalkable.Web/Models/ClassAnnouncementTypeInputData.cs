namespace Chalkable.Web.Models
{
    public class ClassAnnouncementTypeInputData
    {
        public int ClassId { get; set; }
        public bool IsSystem { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Percentage { get; set; }
        public byte HighScoresToDrop { get; set; }
        public byte LowScoresToDrop { get; set; }
    }
}