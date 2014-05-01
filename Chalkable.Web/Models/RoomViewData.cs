using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class RoomViewData
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public string Building { get; set; }
        public string HomeRoomNumber { get; set; }
        public string Size { get; set; }
        public int? Capacity { get; set; }
        public string PhoneNumber { get; set; }
        public string RoomTypeDescription { get; set; }
        public int RoomTypeId { get; set; }

        private RoomViewData() { }

        public static RoomViewData Create(Room room)
        {
            return new RoomViewData
                {
                    Id = room.Id,
                    RoomNumber = room.RoomNumber,
                    Description = room.Description,
                    Size = room.Size,
                    Capacity = room.Capacity,
                    PhoneNumber = room.PhoneNumber
                };
        }
    }
}