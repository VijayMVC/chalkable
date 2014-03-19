using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoRoomStorage
    {
        private Dictionary<int ,Room> roomData = new Dictionary<int, Room>(); 
        public void Add(Room room)
        {
            if (!roomData.ContainsKey(room.Id))
                roomData.Add(room.Id, room);
        }

        public void AddRooms(IList<Room> rooms)
        {
            foreach (var room in rooms)
            {
                Add(room);
            }
        }

        public Room GetById(int id)
        {
             return roomData.ContainsKey(id) ? roomData[id] : null;
        }


        public Room Edit(int id, string roomNumber, string description, string size, int? capacity, string phoneNumber)
        {
            var room = GetById(id);
            room.RoomNumber = roomNumber;
            room.Description = description;
            room.Capacity = capacity;
            room.Size = size;
            room.PhoneNumber = phoneNumber;
            return room;
        }

        public void Delete(int id)
        {
            roomData.Remove(id);
        }

        public PaginatedList<Room> GetAll(int start, int count)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(IList<int> ids)
        {
            foreach (var item in ids)
            {
                Delete(item);
            }
        }

        public void Update(IList<Room> rooms)
        {
            throw new System.NotImplementedException();
        }
    }
}
