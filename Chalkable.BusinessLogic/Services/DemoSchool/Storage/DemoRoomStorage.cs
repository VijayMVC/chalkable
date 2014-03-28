using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoRoomStorage:BaseDemoStorage<int, Room>
    {
        public DemoRoomStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(Room room)
        {
            if (!data.ContainsKey(room.Id))
                data.Add(room.Id, room);
        }

        public void AddRooms(IList<Room> rooms)
        {
            foreach (var room in rooms)
            {
                Add(room);
            }
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

        public PaginatedList<Room> GetAll(int start, int count)
        {
            throw new System.NotImplementedException();
        }

        public void Update(IList<Room> rooms)
        {
            foreach (var room in rooms.Where(room => data.ContainsKey(room.Id)))
            {
                data[room.Id] = room;
            }
        }
    }
}
