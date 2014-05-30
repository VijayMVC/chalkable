using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    
    public class DemoRoomService : DemoSchoolServiceBase, IRoomService
    {
        public DemoRoomService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public Room AddRoom(int id, int schoolId, string roomNumber, string description, string size, int? capacity, string phoneNumber)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            var room = new Room
            {
                Id = id,
                SchoolRef = schoolId,
                Capacity = capacity,
                Description = description,
                PhoneNumber = phoneNumber,
                RoomNumber = roomNumber,
                Size = size,
            };
            Storage.RoomStorage.Add(room);
            return room;

        }

        public void AddRooms(IList<Room> rooms)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.RoomStorage.AddRooms(rooms);
        }

        public Room EditRoom(int id, string roomNumber, string description, string size, int? capacity, string phoneNumber)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Storage.RoomStorage.Edit(id, roomNumber, description, size, capacity, phoneNumber);

        }

        public void EditRooms(IList<Room> rooms)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.RoomStorage.Update(rooms);
        }

        public void DeleteRoom(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();


            if (Storage.ClassPeriodStorage.Exists(new ClassPeriodQuery { RoomId = id }))
                throw new ChalkableException(ChlkResources.ERR_ROOM_CANT_DELETE_ROOM_TYPE_ASSIGNED_TO_CLASSPERIOD);
            Storage.RoomStorage.Delete(id);
        }

        public void DeleteRooms(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.RoomStorage.Delete(ids);
        }

        public PaginatedList<Room> GetRooms(int start = 0, int count = Int32.MaxValue)
        {
            return Storage.RoomStorage.GetAll(start, count);
            
        }

        public Room WhereIsPerson(int personId, DateTime dateTime)
        {
            var classPeriod = ServiceLocator.ClassPeriodService.GetClassPeriodForSchoolPersonByDate(personId, dateTime);
            if (classPeriod == null) return null;
            var c = ServiceLocator.ClassService.GetClassDetailsById(classPeriod.ClassRef);
            return c.RoomRef.HasValue ? GetRoomById(c.RoomRef.Value) : null;
        }

        public Room GetRoomById(int id)
        {
            return Storage.RoomStorage.GetById(id);
        }
    }
}
