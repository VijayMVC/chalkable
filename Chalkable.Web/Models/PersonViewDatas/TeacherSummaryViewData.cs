using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class TeacherSummaryViewData : StaffViewData
    {
        public int? RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomNumber { get; set; }

        protected TeacherSummaryViewData(Staff person, Room room) : base(person)
        {
            if (room != null)
            {
                RoomId = room.Id;
                RoomName = room.Description;
                RoomNumber = room.RoomNumber;
            }
        }
        public static TeacherSummaryViewData Create(Staff person, Room room)
        {
            return new TeacherSummaryViewData(person, room);
        }
    }
}