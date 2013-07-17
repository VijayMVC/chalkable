using System;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonSummaryViewData : ShortPersonViewData
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        //public IList<PrivateNoteViewData> Notes { get; set; }
        private const string NO_CLASS_SCHEDULED = "No Class Scheduled";

        protected PersonSummaryViewData(Person person, Room room): base(person)
        {
            PrepareRoomInfo(room);
        }
        private void PrepareRoomInfo(Room room)
        {
            RoomId = room != null ? room.Id : Guid.Empty;
            RoomName = room != null ? room.Description : NO_CLASS_SCHEDULED;
        }
    }
}