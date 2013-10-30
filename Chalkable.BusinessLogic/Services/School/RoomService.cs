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
        Room AddRoom(int id, int schoolId, string roomNumber, string description, string size, int? capacity, string phoneNumber);
        Room EditRoom(int id, int schoolId, string roomNumber, string description, string size, int? capacity, string phoneNumber);
        void DeleteRoom(int id);
        PaginatedList<Room> GetRooms(int start = 0, int count = int.MaxValue);
        Room WhereIsPerson(int personId, DateTime dateTime);
        Room GetRoomById(int id);
    }

    //TODO: needs tests 

    public class RoomService : SchoolServiceBase, IRoomService
    {
        public RoomService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Room AddRoom(int id, int schoolId, string roomNumber, string description, string size, int? capacity, string phoneNumber)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new RoomDataAccess(uow, Context.SchoolLocalId);
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
                da.Insert(room);
                uow.Commit();
                return room;
            }
        }

        public Room EditRoom(int id, int schoolId, string roomNumber, string description, string size, int? capacity, string phoneNumber)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new RoomDataAccess(uow, Context.SchoolLocalId);
                var room = da.GetById(id);
                room.RoomNumber = roomNumber;
                room.Description = description;
                room.Capacity = capacity;
                room.Size = size;
                room.SchoolRef = schoolId;
                room.PhoneNumber = phoneNumber;
                da.Update(room);
                uow.Commit();
                return room;
            }
        }

        public void DeleteRoom(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
           
            using (var uow = Update())
            {
                var cpDa = new ClassPeriodDataAccess(uow, Context.SchoolLocalId);
                if (cpDa.Exists(new ClassPeriodQuery{RoomId = id}))
                    throw new ChalkableException(ChlkResources.ERR_ROOM_CANT_DELETE_ROOM_TYPE_ASSIGNED_TO_CLASSPERIOD);

                new RoomDataAccess(uow, Context.SchoolLocalId).Delete(id);
                uow.Commit();
            }
        }

        public PaginatedList<Room> GetRooms(int start = 0, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                return new RoomDataAccess(uow, Context.SchoolLocalId).GetPage(start, count);
            }
        }

        public Room WhereIsPerson(int personId, DateTime dateTime)
        {
            var classPeriod = ServiceLocator.ClassPeriodService.GetClassPeriodForSchoolPersonByDate(personId, dateTime);
            return classPeriod != null && classPeriod.RoomRef.HasValue ? GetRoomById(classPeriod.RoomRef.Value) : null;
        }

        public Room GetRoomById(int id)
        {
            using (var uow = Read())
            {
                return new RoomDataAccess(uow, Context.SchoolLocalId).GetById(id);
            }
        }
    }
}
