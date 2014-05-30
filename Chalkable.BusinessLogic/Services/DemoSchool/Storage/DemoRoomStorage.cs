using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoRoomStorage:BaseDemoIntStorage<Room>
    {
        public DemoRoomStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }
        
        public void AddRooms(IList<Room> rooms)
        {
            Add(rooms);
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
            var rooms = data.Select(x => x.Value).ToList().Skip(start).Take(count);
            return new PaginatedList<Room>(rooms, start / count, count, data.Count);
        }
    }
}
