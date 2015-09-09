using System;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonSummaryViewData : ShortPersonViewData
    {
        public int? RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomNumber { get; set; }
        //public IList<PrivateNoteViewData> Notes { get; set; }
        
        protected PersonSummaryViewData(Person person, Room room): base(person)
        {
            PrepareRoomInfo(room);
        }
        private void PrepareRoomInfo(Room room)
        {
            //RoomName = "";  
            if (room != null)
            {
                RoomId = room.Id;
                RoomNumber = room.RoomNumber;
                RoomName = room.Description;
            }
        }
    }
}