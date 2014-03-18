using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoRoomStorage
    {
        public Room Add(Room room)
        {
            throw new System.NotImplementedException();
        }

        public void AddRooms(IList<Room> rooms)
        {
            throw new System.NotImplementedException();
        }

        public Room GetById(int id)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public PaginatedList<Room> GetAll(int start, int count)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(IList<int> ids)
        {
            throw new System.NotImplementedException();
        }

        public void Update(IList<Room> rooms)
        {
            throw new System.NotImplementedException();
        }
    }
}
