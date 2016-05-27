namespace Chalkable.API.Models.StudentAttendance
{
    public class CheckInCheckOut
    {
        public bool IsCheckIn { get; set; }
        public int? CheckInReasonId { get; set; }
        public int? CheckInTime { get; set; }
    }
}
