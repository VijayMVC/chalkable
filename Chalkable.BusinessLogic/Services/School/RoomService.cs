using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IRoomService
    {
        Room AddRoom(string roomNumber, string description, string size, int? capacity, string phoneNumber, int? sisId = null);
        Room EditRoom(Guid id, string roomNumber, string description, string size, int? capacity, string phoneNumber);
        void DeleteRoom(Guid id);
        PaginatedList<Room> GetRooms(int start = 0, int count = int.MaxValue);
        Room WhereIsPerson(Guid personId, DateTime dateTime);
        Room GetRoomById(Guid id);
    }

    //TODO: needs tests 

    public class RoomService : SchoolServiceBase, IRoomService
    {
        public RoomService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Room AddRoom(string roomNumber, string description, string size, int? capacity, string phoneNumber, int? sisId = null)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new RoomDataAccess(uow);
                var room = new Room
                    {
                        Id = Guid.NewGuid(),
                        Capacity = capacity,
                        Description = description,
                        PhoneNumber = phoneNumber,
                        RoomNumber = roomNumber,
                        Size = size, 
                        SisId = sisId
                    };
                da.Insert(room);
                uow.Commit();
                return room;
            }
        }

        public Room EditRoom(Guid id, string roomNumber, string description, string size, int? capacity, string phoneNumber)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new RoomDataAccess(uow);
                var room = da.GetById(id);
                room.RoomNumber = roomNumber;
                room.Description = description;
                room.Capacity = capacity;
                room.Size = size;
                room.PhoneNumber = phoneNumber;
                da.Update(room);
                uow.Commit();
                return room;
            }
        }

        public void DeleteRoom(Guid id)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
           
            using (var uow = Update())
            {
                var cpDa = new ClassPeriodDataAccess(uow);
                if (cpDa.Exists(new ClassPeriodQuery{RoomId = id}))
                    throw new ChalkableException(ChlkResources.ERR_ROOM_CANT_DELETE_ROOM_TYPE_ASSIGNED_TO_CLASSPERIOD);
                
                new RoomDataAccess(uow).Delete(id);
                uow.Commit();
            }
        }

        public PaginatedList<Room> GetRooms(int start = 0, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                return new RoomDataAccess(uow).GetPage(start, count);
            }
        }

        public Room WhereIsPerson(Guid personId, DateTime dateTime)
        {
            var classPeriod = ServiceLocator.ClassPeriodService.GetClassPeriodForSchoolPersonByDate(personId, dateTime);
            return classPeriod != null ? GetRoomById(classPeriod.RoomRef) : null;
        }

        public Room GetRoomById(Guid id)
        {
            using (var uow = Read())
            {
                return new RoomDataAccess(uow).GetById(id);
            }
        }
    }
}
